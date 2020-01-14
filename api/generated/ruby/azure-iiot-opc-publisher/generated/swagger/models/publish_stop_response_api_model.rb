# encoding: utf-8
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.

module azure.iiot.opc.publisher
  module Models
    #
    # Result of unpublish request
    #
    class PublishStopResponseApiModel
      # @return [ServiceResultApiModel]
      attr_accessor :error_info


      #
      # Mapper for PublishStopResponseApiModel class as Ruby Hash.
      # This will be used for serialization/deserialization.
      #
      def self.mapper()
        {
          client_side_validation: true,
          required: false,
          serialized_name: 'PublishStopResponseApiModel',
          type: {
            name: 'Composite',
            class_name: 'PublishStopResponseApiModel',
            model_properties: {
              error_info: {
                client_side_validation: true,
                required: false,
                serialized_name: 'errorInfo',
                type: {
                  name: 'Composite',
                  class_name: 'ServiceResultApiModel'
                }
              }
            }
          }
        }
      end
    end
  end
end
