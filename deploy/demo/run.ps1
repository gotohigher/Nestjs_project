<#
 .SYNOPSIS
    Deploys to Azure

 .DESCRIPTION
    Deploys an Azure Resource Manager template of choice

 .PARAMETER applicationName
    The name of the application. 

 .PARAMETER resourceGroupName
    The resource group where the template will be deployed. 

 .PARAMETER containerRegistryPrefix
    Optional, a container registry prefix from which to pull the micro services containers to deploy.

 .PARAMETER aadConfig
    The AAD configuration the template will be configured with.

 .PARAMETER interactive
    Whether to run in interactive mode
#>

param(
    [Parameter(Mandatory=$True)] [string] $applicationName,
    [Parameter(Mandatory=$True)] [string] $resourceGroupName,
    $aadConfig = $null,
    $containerRegistryPrefix = $null,
    $interactive = $true
)

#******************************************************************************
# Get env file content from deployment
#******************************************************************************
Function GetEnvironmentVariables() {
    Param(
        $deployment
    )

    $IOTHUB_NAME = $deployment.Outputs["iothub-name"].Value
    $IOTHUB_ENDPOINT = $deployment.Outputs["iothub-endpoint"].Value
    $IOTHUB_CONNSTRING = $deployment.Outputs["iothub-connstring"].Value
    $AZURE_WEBSITE = $deployment.Outputs["azureWebsite"].Value

    Write-Output `
        "_HUB_CS=$IOTHUB_CONNSTRING"
    Write-Output `
        "PCS_IOTHUB_CONNSTRING=$IOTHUB_CONNSTRING"
    Write-Output `
        "PCS_IOTHUBREACT_ACCESS_CONNSTRING=$IOTHUB_CONNSTRING"
    Write-Output `
        "PCS_IOTHUBREACT_HUB_NAME=$IOTHUB_NAME"
    Write-Output `
        "PCS_IOTHUBREACT_HUB_ENDPOINT=$IOTHUB_ENDPOINT"
        
    Write-Output `
        "PCS_TWIN_REGISTRY_URL=$AZURE_WEBSITE/registry"
    Write-Output `
        "PCS_TWIN_SERVICE_URL=$AZURE_WEBSITE/twin"
    Write-Output `
        "REACT_APP_PCS_TWIN_REGISTRY_URL=$AZURE_WEBSITE/registry"
    Write-Output `
        "REACT_APP_PCS_TWIN_SERVICE_URL=$AZURE_WEBSITE/twin"

    if (!$aadConfig) {
    Write-Output `
        "PCS_AUTH_HTTPSREDIRECTPORT=443"
    Write-Output `
        "PCS_AUTH_REQUIRED=false"
    Write-Output `
        "REACT_APP_PCS_AUTH_REQUIRED=false"
        return;
    }

    $AUTH_AUDIENCE = $aadConfig.Audience
    $AUTH_AAD_APPID = $aadConfig.ClientId
    $AUTH_AAD_TENANT = $aadConfig.TenantId
    $AUTH_AAD_AUTHORITY = $aadConfig.Instance

    Write-Output `
        "PCS_AUTH_HTTPSREDIRECTPORT=443"
    Write-Output `
        "PCS_AUTH_REQUIRED=true"
    Write-Output `
        "PCS_AUTH_AUDIENCE=$AUTH_AUDIENCE"
    Write-Output `
        "PCS_AUTH_ISSUER=https://sts.windows.net/$AUTH_AAD_TENANT/"
    Write-Output `
        "PCS_WEBUI_AUTH_AAD_APPID=$AUTH_AAD_APPID"
    Write-Output `
        "PCS_WEBUI_AUTH_AAD_AUTHORITY=$AUTH_AAD_AUTHORITY"
    Write-Output `
        "PCS_WEBUI_AUTH_AAD_TENANT=$AUTH_AAD_TENANT"

    Write-Output `
        "REACT_APP_PCS_AUTH_REQUIRED=true"
    Write-Output `
        "REACT_APP_PCS_AUTH_AUDIENCE=$AUTH_AUDIENCE"
    Write-Output `
        "REACT_APP_PCS_AUTH_ISSUER=https://sts.windows.net/$AUTH_AAD_TENANT/"
    Write-Output `
        "REACT_APP_PCS_WEBUI_AUTH_AAD_APPID=$AUTH_AAD_APPID"
    Write-Output `
        "REACT_APP_PCS_WEBUI_AUTH_AAD_AUTHORITY=$AUTH_AAD_AUTHORITY"
    Write-Output `
        "REACT_APP_PCS_WEBUI_AUTH_AAD_TENANT=$AUTH_AAD_TENANT"
}

#******************************************************************************
# Generate a random password
#******************************************************************************
Function CreateRandomPassword() {
    param(
        $length = 15
    ) 
    $punc = 46..46
    $digits = 48..57
    $lcLetters = 65..90
    $ucLetters = 97..122

    $password = `
        [char](Get-Random -Count 1 -InputObject ($lcLetters)) + `
        [char](Get-Random -Count 1 -InputObject ($ucLetters)) + `
        [char](Get-Random -Count 1 -InputObject ($digits)) + `
        [char](Get-Random -Count 1 -InputObject ($punc))
    $password += get-random -Count ($length -4) `
        -InputObject ($punc + $digits + $lcLetters + $ucLetters) |`
         % -begin { $aa = $null } -process {$aa += [char]$_} -end {$aa}

    return $password
}

#******************************************************************************
# find the top most folder with docker-compose.yml in it
#******************************************************************************
Function GetRootFolder() {
    param(
        $startDir
    ) 
    $cur = $startDir
    while ($True) {
        if ([string]::IsNullOrEmpty($cur)) {
            break
        }
        $test = Join-Path $cur "docker-compose.yml"
        if (Test-Path $test) {
            $found = $cur
        }
        elseif (![string]::IsNullOrEmpty($found)) {
            break
        }
        $cur = Split-Path $cur
    }
    if (![string]::IsNullOrEmpty($found)) {
        return $found
    }
    return $startDir
}

#******************************************************************************
# Script body
#******************************************************************************
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path

# Register RPs
Register-AzureRmResourceProvider -ProviderNamespace "microsoft.web" | Out-Null
Register-AzureRmResourceProvider -ProviderNamespace "microsoft.compute" | Out-Null

# Set admin user and password
$adminPassword = CreateRandomPassword
$adminUser = "azureuser"
$templateParameters = @{ 
    adminPassword = $adminPassword
    adminUsername = $adminUser
    azureWebsiteName = $script:applicationName
}

try {
    # Try set branch name as current branch
    $output = cmd /c "git rev-parse --abbrev-ref HEAD" 2`>`&1
    $branchName = ("{0}" -f $output);
    Write-Host "VM deployment will use configuration from '$branchName' branch."
    $templateParameters.Add("branchName", $branchName)
}
catch {
    $templateParameters.Add("branchName", "master")
}

# Configure auth
if ($aadConfig) {
    if (![string]::IsNullOrEmpty($aadConfig.Audience)) { 
        $templateParameters.Add("authAudience", $aadConfig.Audience)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ClientId)) { 
        $templateParameters.Add("aadClientId", $aadConfig.ClientId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.TenantId)) { 
        $templateParameters.Add("aadTenantId", $aadConfig.TenantId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.Instance)) { 
        $templateParameters.Add("aadInstance", $aadConfig.Instance)
    }
}

if (![string]::IsNullOrEmpty($script:containerRegistryPrefix)) {
    $templateParameters.Add("containerRegistryPrefix", $script:containerRegistryPrefix)
}

# Create ssl cert 
$cert = New-SelfSignedCertificate -DnsName "opctwin.services.net" `
    -KeyExportPolicy Exportable -CertStoreLocation "Cert:\CurrentUser\My"
if ($cert) {
    $thumbprint = $cert.Thumbprint;
    if (![string]::IsNullOrEmpty($thumbprint)) { 
        $templateParameters.Add("remoteEndpointSSLThumbprint", $thumbprint)
    }
    $certificate = [Convert]::ToBase64String(`
        $cert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pfx, `
        $adminPassword))
    if (![string]::IsNullOrEmpty($certificate)) { 
        $templateParameters.Add("remoteEndpointCertificate", $certificate)
    }
}

Write-Host "Starting deployment..."
Write-Host 
Write-Host "To trouble shoot use the following User and Password to log onto your VM:"
Write-Host 
Write-Host $adminUser
Write-Host $adminPassword
Write-Host 

# Start the deployment
$templateFilePath = Join-Path $ScriptDir "template.json"
$deployment = New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName `
    -TemplateFile $templateFilePath -TemplateParameterObject $templateParameters

$website = $deployment.Outputs["azureWebsite"].Value
$iothub = $deployment.Outputs["iothub-connstring"].Value

if ($aadConfig -and $aadConfig.ClientObjectId) {
    # 
    # Update client application to add reply urls 
    #
    $replyUrls = New-Object System.Collections.Generic.List[System.String]
    $replyUrls.Add($website)
    $replyUrls.Add($website + "/twin/oauth2-redirect.html")
    $replyUrls.Add($website + "/registry/oauth2-redirect.html")
    $replyUrls.Add($website + "/history/oauth2-redirect.html")
    $replyUrls.Add($website + "/vault/oauth2-redirect.html")
    # still connected
    Set-AzureADApplication -ObjectId $aadConfig.ClientObjectId -ReplyUrls $replyUrls
}

# find the top most folder with docker-compose.yml in it
$rootDir = GetRootFolder $ScriptDir

$writeFile = $false
if ($script:interactive) {
    $ENVVARS = Join-Path $rootDir ".env"
    $prompt = "Save environment as $ENVVARS for local development? [y/n]"
    $reply = Read-Host -Prompt $prompt
    if ($reply -match "[yY]") {
        $writeFile = $true
    }
    if ($writeFile) {
        if (Test-Path $ENVVARS) {
            $prompt = "Overwrite existing .env file in $rootDir? [y/n]"
            if ( $reply -match "[yY]" ) {
                Remove-Item $ENVVARS -Force
            }
            else {
                $writeFile = $false
            }
        }
    }
}
if ($writeFile) {
    GetEnvironmentVariables $deployment | Out-File -Encoding ascii `
        -FilePath $ENVVARS

    Write-Host
    Write-Host ".env file created in $rootDir."
    Write-Host
    Write-Warning "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
    Write-Warning "!The file contains security keys to your Azure resources!"
    Write-Warning "! Safeguard the contents of this file, or delete it now !"
    Write-Warning "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
    Write-Host
}

Write-Host
Write-Host "In your webbrowser go to:"
Write-Host $website
Write-Host 
