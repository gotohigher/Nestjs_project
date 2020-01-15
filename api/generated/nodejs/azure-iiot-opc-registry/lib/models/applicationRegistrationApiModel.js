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
 * Application with optional list of endpoints
 *
 */
class ApplicationRegistrationApiModel {
  /**
   * Create a ApplicationRegistrationApiModel.
   * @property {object} application
   * @property {string} [application.applicationId] Unique application id
   * @property {string} [application.applicationType] Possible values include:
   * 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'
   * @property {string} [application.applicationUri] Unique application uri
   * @property {string} [application.productUri] Product uri
   * @property {string} [application.applicationName] Default name of
   * application
   * @property {string} [application.locale] Locale of default name - defaults
   * to "en"
   * @property {object} [application.localizedNames] Localized Names of
   * application keyed on locale
   * @property {buffer} [application.certificate] Application public cert
   * @property {array} [application.capabilities] The capabilities advertised
   * by the server.
   * @property {array} [application.discoveryUrls] Discovery urls of the server
   * @property {string} [application.discoveryProfileUri] Discovery profile uri
   * @property {string} [application.gatewayServerUri] Gateway server uri
   * @property {array} [application.hostAddresses] Host addresses of server
   * application or null
   * @property {string} [application.siteId] Site of the application
   * @property {string} [application.discovererId] Discoverer that registered
   * the application
   * @property {date} [application.notSeenSince] Last time application was seen
   * @property {object} [application.created]
   * @property {string} [application.created.authorityId] Operation User
   * @property {date} [application.created.time] Operation time
   * @property {object} [application.updated]
   * @property {string} [application.updated.authorityId] Operation User
   * @property {date} [application.updated.time] Operation time
   * @property {array} [endpoints] List of endpoint twins
   * @property {string} [securityAssessment] Possible values include: 'Low',
   * 'Medium', 'High'
   */
  constructor() {
  }

  /**
   * Defines the metadata of ApplicationRegistrationApiModel
   *
   * @returns {object} metadata of ApplicationRegistrationApiModel
   *
   */
  mapper() {
    return {
      required: false,
      serializedName: 'ApplicationRegistrationApiModel',
      type: {
        name: 'Composite',
        className: 'ApplicationRegistrationApiModel',
        modelProperties: {
          application: {
            required: true,
            serializedName: 'application',
            type: {
              name: 'Composite',
              className: 'ApplicationInfoApiModel'
            }
          },
          endpoints: {
            required: false,
            serializedName: 'endpoints',
            type: {
              name: 'Sequence',
              element: {
                  required: false,
                  serializedName: 'EndpointRegistrationApiModelElementType',
                  type: {
                    name: 'Composite',
                    className: 'EndpointRegistrationApiModel'
                  }
              }
            }
          },
          securityAssessment: {
            required: false,
            serializedName: 'securityAssessment',
            type: {
              name: 'Enum',
              allowedValues: [ 'Low', 'Medium', 'High' ]
            }
          }
        }
      }
    };
  }
}

module.exports = ApplicationRegistrationApiModel;
