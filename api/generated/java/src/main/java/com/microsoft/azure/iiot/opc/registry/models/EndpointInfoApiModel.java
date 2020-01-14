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

import org.joda.time.DateTime;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Endpoint registration model.
 */
public class EndpointInfoApiModel {
    /**
     * The registration property.
     */
    @JsonProperty(value = "registration", required = true)
    private EndpointRegistrationApiModel registration;

    /**
     * Application id endpoint is registered under.
     */
    @JsonProperty(value = "applicationId", required = true)
    private String applicationId;

    /**
     * Possible values include: 'Deactivated', 'Activated',
     * 'ActivatedAndConnected'.
     */
    @JsonProperty(value = "activationState")
    private EndpointActivationState activationState;

    /**
     * Possible values include: 'Connecting', 'NotReachable', 'Busy',
     * 'NoTrust', 'CertificateInvalid', 'Ready', 'Error'.
     */
    @JsonProperty(value = "endpointState")
    private EndpointConnectivityState endpointState;

    /**
     * Whether the registration is out of sync.
     */
    @JsonProperty(value = "outOfSync")
    private Boolean outOfSync;

    /**
     * Last time endpoint was seen.
     */
    @JsonProperty(value = "notSeenSince")
    private DateTime notSeenSince;

    /**
     * Get the registration value.
     *
     * @return the registration value
     */
    public EndpointRegistrationApiModel registration() {
        return this.registration;
    }

    /**
     * Set the registration value.
     *
     * @param registration the registration value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withRegistration(EndpointRegistrationApiModel registration) {
        this.registration = registration;
        return this;
    }

    /**
     * Get application id endpoint is registered under.
     *
     * @return the applicationId value
     */
    public String applicationId() {
        return this.applicationId;
    }

    /**
     * Set application id endpoint is registered under.
     *
     * @param applicationId the applicationId value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withApplicationId(String applicationId) {
        this.applicationId = applicationId;
        return this;
    }

    /**
     * Get possible values include: 'Deactivated', 'Activated', 'ActivatedAndConnected'.
     *
     * @return the activationState value
     */
    public EndpointActivationState activationState() {
        return this.activationState;
    }

    /**
     * Set possible values include: 'Deactivated', 'Activated', 'ActivatedAndConnected'.
     *
     * @param activationState the activationState value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withActivationState(EndpointActivationState activationState) {
        this.activationState = activationState;
        return this;
    }

    /**
     * Get possible values include: 'Connecting', 'NotReachable', 'Busy', 'NoTrust', 'CertificateInvalid', 'Ready', 'Error'.
     *
     * @return the endpointState value
     */
    public EndpointConnectivityState endpointState() {
        return this.endpointState;
    }

    /**
     * Set possible values include: 'Connecting', 'NotReachable', 'Busy', 'NoTrust', 'CertificateInvalid', 'Ready', 'Error'.
     *
     * @param endpointState the endpointState value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withEndpointState(EndpointConnectivityState endpointState) {
        this.endpointState = endpointState;
        return this;
    }

    /**
     * Get whether the registration is out of sync.
     *
     * @return the outOfSync value
     */
    public Boolean outOfSync() {
        return this.outOfSync;
    }

    /**
     * Set whether the registration is out of sync.
     *
     * @param outOfSync the outOfSync value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withOutOfSync(Boolean outOfSync) {
        this.outOfSync = outOfSync;
        return this;
    }

    /**
     * Get last time endpoint was seen.
     *
     * @return the notSeenSince value
     */
    public DateTime notSeenSince() {
        return this.notSeenSince;
    }

    /**
     * Set last time endpoint was seen.
     *
     * @param notSeenSince the notSeenSince value to set
     * @return the EndpointInfoApiModel object itself.
     */
    public EndpointInfoApiModel withNotSeenSince(DateTime notSeenSince) {
        this.notSeenSince = notSeenSince;
        return this;
    }

}
