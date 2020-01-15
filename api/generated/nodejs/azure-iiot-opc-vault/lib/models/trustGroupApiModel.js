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
 * Trust group model
 *
 */
class TrustGroupApiModel {
  /**
   * Create a TrustGroupApiModel.
   * @property {string} [name] The name of the trust group.
   * @property {string} [parentId] The identifer of the parent trust group.
   * @property {string} [type] Possible values include:
   * 'ApplicationInstanceCertificate', 'HttpsCertificate',
   * 'UserCredentialCertificate'
   * @property {string} [subjectName] The subject name of the group as
   * distinguished name.
   * @property {string} [lifetime] The lifetime of the trust group certificate.
   * @property {number} [keySize] The trust group certificate key size in bits.
   * @property {string} [signatureAlgorithm] Possible values include: 'Rsa256',
   * 'Rsa384', 'Rsa512', 'Rsa256Pss', 'Rsa384Pss', 'Rsa512Pss'
   * @property {string} [issuedLifetime] The issued certificate lifetime in
   * months.
   * @property {number} [issuedKeySize] The issued certificate key size in
   * bits.
   * @property {string} [issuedSignatureAlgorithm] Possible values include:
   * 'Rsa256', 'Rsa384', 'Rsa512', 'Rsa256Pss', 'Rsa384Pss', 'Rsa512Pss'
   */
  constructor() {
  }

  /**
   * Defines the metadata of TrustGroupApiModel
   *
   * @returns {object} metadata of TrustGroupApiModel
   *
   */
  mapper() {
    return {
      required: false,
      serializedName: 'TrustGroupApiModel',
      type: {
        name: 'Composite',
        className: 'TrustGroupApiModel',
        modelProperties: {
          name: {
            required: false,
            serializedName: 'name',
            type: {
              name: 'String'
            }
          },
          parentId: {
            required: false,
            serializedName: 'parentId',
            type: {
              name: 'String'
            }
          },
          type: {
            required: false,
            serializedName: 'type',
            type: {
              name: 'Enum',
              allowedValues: [ 'ApplicationInstanceCertificate', 'HttpsCertificate', 'UserCredentialCertificate' ]
            }
          },
          subjectName: {
            required: false,
            serializedName: 'subjectName',
            type: {
              name: 'String'
            }
          },
          lifetime: {
            required: false,
            serializedName: 'lifetime',
            type: {
              name: 'String'
            }
          },
          keySize: {
            required: false,
            serializedName: 'keySize',
            type: {
              name: 'Number'
            }
          },
          signatureAlgorithm: {
            required: false,
            serializedName: 'signatureAlgorithm',
            type: {
              name: 'Enum',
              allowedValues: [ 'Rsa256', 'Rsa384', 'Rsa512', 'Rsa256Pss', 'Rsa384Pss', 'Rsa512Pss' ]
            }
          },
          issuedLifetime: {
            required: false,
            serializedName: 'issuedLifetime',
            type: {
              name: 'String'
            }
          },
          issuedKeySize: {
            required: false,
            serializedName: 'issuedKeySize',
            type: {
              name: 'Number'
            }
          },
          issuedSignatureAlgorithm: {
            required: false,
            serializedName: 'issuedSignatureAlgorithm',
            type: {
              name: 'Enum',
              allowedValues: [ 'Rsa256', 'Rsa384', 'Rsa512', 'Rsa256Pss', 'Rsa384Pss', 'Rsa512Pss' ]
            }
          }
        }
      }
    };
  }
}

module.exports = TrustGroupApiModel;
