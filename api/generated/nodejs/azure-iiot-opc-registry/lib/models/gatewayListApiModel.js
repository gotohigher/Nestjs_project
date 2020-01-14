/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

'use strict';

/**
 * Gateway registration list
 *
 */
class GatewayListApiModel {
  /**
   * Create a GatewayListApiModel.
   * @property {array} [items] Registrations
   * @property {string} [continuationToken] Continuation or null if final
   */
  constructor() {
  }

  /**
   * Defines the metadata of GatewayListApiModel
   *
   * @returns {object} metadata of GatewayListApiModel
   *
   */
  mapper() {
    return {
      required: false,
      serializedName: 'GatewayListApiModel',
      type: {
        name: 'Composite',
        className: 'GatewayListApiModel',
        modelProperties: {
          items: {
            required: false,
            serializedName: 'items',
            type: {
              name: 'Sequence',
              element: {
                  required: false,
                  serializedName: 'GatewayApiModelElementType',
                  type: {
                    name: 'Composite',
                    className: 'GatewayApiModel'
                  }
              }
            }
          },
          continuationToken: {
            required: false,
            serializedName: 'continuationToken',
            type: {
              name: 'String'
            }
          }
        }
      }
    };
  }
}

module.exports = GatewayListApiModel;
