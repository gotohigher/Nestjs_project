/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

package com.microsoft.azure.iiot.opc.history.models;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * A registered callback.
 */
public class CallbackApiModel {
    /**
     * Uri to call - should use https scheme in which
     * case security is enforced.
     */
    @JsonProperty(value = "uri")
    private String uri;

    /**
     * Http Method to use for callback. Possible values include: 'Get', 'Post',
     * 'Put', 'Delete'.
     */
    @JsonProperty(value = "method")
    private CallbackMethodType method;

    /**
     * Authentication header to add or null if not needed.
     */
    @JsonProperty(value = "authenticationHeader")
    private String authenticationHeader;

    /**
     * Get uri to call - should use https scheme in which
     case security is enforced.
     *
     * @return the uri value
     */
    public String uri() {
        return this.uri;
    }

    /**
     * Set uri to call - should use https scheme in which
     case security is enforced.
     *
     * @param uri the uri value to set
     * @return the CallbackApiModel object itself.
     */
    public CallbackApiModel withUri(String uri) {
        this.uri = uri;
        return this;
    }

    /**
     * Get http Method to use for callback. Possible values include: 'Get', 'Post', 'Put', 'Delete'.
     *
     * @return the method value
     */
    public CallbackMethodType method() {
        return this.method;
    }

    /**
     * Set http Method to use for callback. Possible values include: 'Get', 'Post', 'Put', 'Delete'.
     *
     * @param method the method value to set
     * @return the CallbackApiModel object itself.
     */
    public CallbackApiModel withMethod(CallbackMethodType method) {
        this.method = method;
        return this;
    }

    /**
     * Get authentication header to add or null if not needed.
     *
     * @return the authenticationHeader value
     */
    public String authenticationHeader() {
        return this.authenticationHeader;
    }

    /**
     * Set authentication header to add or null if not needed.
     *
     * @param authenticationHeader the authenticationHeader value to set
     * @return the CallbackApiModel object itself.
     */
    public CallbackApiModel withAuthenticationHeader(String authenticationHeader) {
        this.authenticationHeader = authenticationHeader;
        return this;
    }

}
