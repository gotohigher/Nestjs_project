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
 * History update results
 *
 */
class HistoryUpdateResponseApiModel {
  /**
   * Create a HistoryUpdateResponseApiModel.
   * @property {array} [results] List of results from the update operation
   * @property {object} [errorInfo]
   * @property {number} [errorInfo.statusCode] Error code - if null operation
   * succeeded.
   * @property {string} [errorInfo.errorMessage] Error message in case of error
   * or null.
   * @property {object} [errorInfo.diagnostics] Additional diagnostics
   * information
   */
  constructor() {
  }

  /**
   * Defines the metadata of HistoryUpdateResponseApiModel
   *
   * @returns {object} metadata of HistoryUpdateResponseApiModel
   *
   */
  mapper() {
    return {
      required: false,
      serializedName: 'HistoryUpdateResponseApiModel',
      type: {
        name: 'Composite',
        className: 'HistoryUpdateResponseApiModel',
        modelProperties: {
          results: {
            required: false,
            serializedName: 'results',
            type: {
              name: 'Sequence',
              element: {
                  required: false,
                  serializedName: 'ServiceResultApiModelElementType',
                  type: {
                    name: 'Composite',
                    className: 'ServiceResultApiModel'
                  }
              }
            }
          },
          errorInfo: {
            required: false,
            serializedName: 'errorInfo',
            type: {
              name: 'Composite',
              className: 'ServiceResultApiModel'
            }
          }
        }
      }
    };
  }
}

module.exports = HistoryUpdateResponseApiModel;
