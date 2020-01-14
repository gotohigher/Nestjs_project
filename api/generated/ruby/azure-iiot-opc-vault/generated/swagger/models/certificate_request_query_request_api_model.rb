# encoding: utf-8
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.

module azure.iiot.opc.vault
  module Models
    #
    # Certificate request query model
    #
    class CertificateRequestQueryRequestApiModel
      # @return [String] The entity id to filter with
      attr_accessor :entity_id

      # @return [CertificateRequestState] Possible values include: 'New',
      # 'Approved', 'Rejected', 'Failure', 'Completed', 'Accepted'
      attr_accessor :state


      #
      # Mapper for CertificateRequestQueryRequestApiModel class as Ruby Hash.
      # This will be used for serialization/deserialization.
      #
      def self.mapper()
        {
          client_side_validation: true,
          required: false,
          serialized_name: 'CertificateRequestQueryRequestApiModel',
          type: {
            name: 'Composite',
            class_name: 'CertificateRequestQueryRequestApiModel',
            model_properties: {
              entity_id: {
                client_side_validation: true,
                required: false,
                serialized_name: 'entityId',
                type: {
                  name: 'String'
                }
              },
              state: {
                client_side_validation: true,
                required: false,
                serialized_name: 'state',
                type: {
                  name: 'Enum',
                  module: 'CertificateRequestState'
                }
              }
            }
          }
        }
      end
    end
  end
end
