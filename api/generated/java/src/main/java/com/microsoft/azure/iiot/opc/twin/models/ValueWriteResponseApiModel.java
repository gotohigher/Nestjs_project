/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

package com.microsoft.azure.iiot.opc.twin.models;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Value write response model.
 */
public class ValueWriteResponseApiModel {
    /**
     * The errorInfo property.
     */
    @JsonProperty(value = "errorInfo")
    private ServiceResultApiModel errorInfo;

    /**
     * Get the errorInfo value.
     *
     * @return the errorInfo value
     */
    public ServiceResultApiModel errorInfo() {
        return this.errorInfo;
    }

    /**
     * Set the errorInfo value.
     *
     * @param errorInfo the errorInfo value to set
     * @return the ValueWriteResponseApiModel object itself.
     */
    public ValueWriteResponseApiModel withErrorInfo(ServiceResultApiModel errorInfo) {
        this.errorInfo = errorInfo;
        return this;
    }

}
