/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

package com.microsoft.azure.iiot.opc.registry.models;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Application information.
 */
public class ApplicationRegistrationQueryApiModel {
    /**
     * Possible values include: 'Server', 'Client', 'ClientAndServer',
     * 'DiscoveryServer'.
     */
    @JsonProperty(value = "applicationType")
    private ApplicationType applicationType;

    /**
     * Application uri.
     */
    @JsonProperty(value = "applicationUri")
    private String applicationUri;

    /**
     * Product uri.
     */
    @JsonProperty(value = "productUri")
    private String productUri;

    /**
     * Name of application.
     */
    @JsonProperty(value = "applicationName")
    private String applicationName;

    /**
     * Locale of application name - default is "en".
     */
    @JsonProperty(value = "locale")
    private String locale;

    /**
     * Application capability to query with.
     */
    @JsonProperty(value = "capability")
    private String capability;

    /**
     * Discovery profile uri.
     */
    @JsonProperty(value = "discoveryProfileUri")
    private String discoveryProfileUri;

    /**
     * Gateway server uri.
     */
    @JsonProperty(value = "gatewayServerUri")
    private String gatewayServerUri;

    /**
     * Supervisor or site the application belongs to.
     */
    @JsonProperty(value = "siteOrGatewayId")
    private String siteOrGatewayId;

    /**
     * Whether to include apps that were soft deleted.
     */
    @JsonProperty(value = "includeNotSeenSince")
    private Boolean includeNotSeenSince;

    /**
     * Get possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'.
     *
     * @return the applicationType value
     */
    public ApplicationType applicationType() {
        return this.applicationType;
    }

    /**
     * Set possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'.
     *
     * @param applicationType the applicationType value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withApplicationType(ApplicationType applicationType) {
        this.applicationType = applicationType;
        return this;
    }

    /**
     * Get application uri.
     *
     * @return the applicationUri value
     */
    public String applicationUri() {
        return this.applicationUri;
    }

    /**
     * Set application uri.
     *
     * @param applicationUri the applicationUri value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withApplicationUri(String applicationUri) {
        this.applicationUri = applicationUri;
        return this;
    }

    /**
     * Get product uri.
     *
     * @return the productUri value
     */
    public String productUri() {
        return this.productUri;
    }

    /**
     * Set product uri.
     *
     * @param productUri the productUri value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withProductUri(String productUri) {
        this.productUri = productUri;
        return this;
    }

    /**
     * Get name of application.
     *
     * @return the applicationName value
     */
    public String applicationName() {
        return this.applicationName;
    }

    /**
     * Set name of application.
     *
     * @param applicationName the applicationName value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withApplicationName(String applicationName) {
        this.applicationName = applicationName;
        return this;
    }

    /**
     * Get locale of application name - default is "en".
     *
     * @return the locale value
     */
    public String locale() {
        return this.locale;
    }

    /**
     * Set locale of application name - default is "en".
     *
     * @param locale the locale value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withLocale(String locale) {
        this.locale = locale;
        return this;
    }

    /**
     * Get application capability to query with.
     *
     * @return the capability value
     */
    public String capability() {
        return this.capability;
    }

    /**
     * Set application capability to query with.
     *
     * @param capability the capability value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withCapability(String capability) {
        this.capability = capability;
        return this;
    }

    /**
     * Get discovery profile uri.
     *
     * @return the discoveryProfileUri value
     */
    public String discoveryProfileUri() {
        return this.discoveryProfileUri;
    }

    /**
     * Set discovery profile uri.
     *
     * @param discoveryProfileUri the discoveryProfileUri value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withDiscoveryProfileUri(String discoveryProfileUri) {
        this.discoveryProfileUri = discoveryProfileUri;
        return this;
    }

    /**
     * Get gateway server uri.
     *
     * @return the gatewayServerUri value
     */
    public String gatewayServerUri() {
        return this.gatewayServerUri;
    }

    /**
     * Set gateway server uri.
     *
     * @param gatewayServerUri the gatewayServerUri value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withGatewayServerUri(String gatewayServerUri) {
        this.gatewayServerUri = gatewayServerUri;
        return this;
    }

    /**
     * Get supervisor or site the application belongs to.
     *
     * @return the siteOrGatewayId value
     */
    public String siteOrGatewayId() {
        return this.siteOrGatewayId;
    }

    /**
     * Set supervisor or site the application belongs to.
     *
     * @param siteOrGatewayId the siteOrGatewayId value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withSiteOrGatewayId(String siteOrGatewayId) {
        this.siteOrGatewayId = siteOrGatewayId;
        return this;
    }

    /**
     * Get whether to include apps that were soft deleted.
     *
     * @return the includeNotSeenSince value
     */
    public Boolean includeNotSeenSince() {
        return this.includeNotSeenSince;
    }

    /**
     * Set whether to include apps that were soft deleted.
     *
     * @param includeNotSeenSince the includeNotSeenSince value to set
     * @return the ApplicationRegistrationQueryApiModel object itself.
     */
    public ApplicationRegistrationQueryApiModel withIncludeNotSeenSince(Boolean includeNotSeenSince) {
        this.includeNotSeenSince = includeNotSeenSince;
        return this;
    }

}
