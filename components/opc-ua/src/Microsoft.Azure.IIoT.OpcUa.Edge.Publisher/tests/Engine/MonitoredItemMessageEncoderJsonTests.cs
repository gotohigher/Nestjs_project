﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Tests.Engine {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Engine;
    using Microsoft.Azure.IIoT.OpcUa.Protocol.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Runtime;
    using Moq;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class MonitoredItemMessageEncoderJsonTests {

        /// <summary>
        /// Create compliant encoder
        /// </summary>
        /// <returns></returns>
        private static NetworkMessageEncoder GetEncoder() {
            var loggerMock = new Mock<ILogger>();
            return new NetworkMessageEncoder(loggerMock.Object, new WriterGroupJobConfig());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EmptyMessagesTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = new List<SubscriptionNotificationModel>();

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            Assert.Empty(networkMessages);
            Assert.Equal((uint)0, encoder.NotificationsProcessedCount);
            Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
            Assert.Equal((uint)0, encoder.MessagesProcessedCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EmptyDataSetMessageModelTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = new[] { new SubscriptionNotificationModel() };

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            Assert.Empty(networkMessages);
            Assert.Equal((uint)0, encoder.NotificationsProcessedCount);
            Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
            Assert.Equal((uint)0, encoder.MessagesProcessedCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeTooBigMessageTest(bool encodeBatchFlag) {
            var maxMessageSize = 100;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(3, false, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            Assert.Empty(networkMessages);
            Assert.Equal((uint)0, encoder.NotificationsProcessedCount);
            Assert.Equal((uint)6, encoder.NotificationsDroppedCount);
            Assert.Equal((uint)0, encoder.MessagesProcessedCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeAsUadpNotSupportedTest(bool encodeBatchFlag) {
            var maxMessageSize = 100;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(3, false,
                encoding: MessageEncoding.Uadp, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            Assert.Empty(networkMessages);
            Assert.Equal((uint)0, encoder.NotificationsProcessedCount);
            Assert.Equal((uint)6, encoder.NotificationsDroppedCount);
            Assert.Equal((uint)0, encoder.MessagesProcessedCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeDataTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(20, false, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            if (encodeBatchFlag) {
                Assert.Equal(1, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)20, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)1, encoder.MessagesProcessedCount);
                Assert.Equal(20, encoder.AvgNotificationsPerMessage);
            }
            else {
                Assert.Equal(210, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)20, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)210, encoder.MessagesProcessedCount);
                Assert.Equal(0.10, Math.Round(encoder.AvgNotificationsPerMessage, 2));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeDataWithMultipleNotificationsTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(20, false, isSampleMode: true);
            var notifications = messages.SelectMany(n => n.Notifications).ToList();
            messages[0].Notifications = notifications;
            messages = new List<SubscriptionNotificationModel> { messages[0] };

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            if (encodeBatchFlag) {
                Assert.Equal(1, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)1, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)1, encoder.MessagesProcessedCount);
                Assert.Equal(1, encoder.AvgNotificationsPerMessage);
            }
            else {
                Assert.Equal(210, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)1, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)210, encoder.MessagesProcessedCount);
                Assert.Equal(0.005, Math.Round(encoder.AvgNotificationsPerMessage, 3));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EncodeChunkTest(bool encodeBatchFlag) {
            var maxMessageSize = 8 * 1024;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(50, false, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            var count = networkMessages.Sum(m => m.Buffers.Count);
            Assert.All(networkMessages, m => Assert.All(m.Buffers, m => Assert.True((m?.Length ?? 0) <= maxMessageSize, m?.Length.ToString())));
            if (encodeBatchFlag) {
                Assert.InRange(count, 31, 33);
                Assert.Equal((uint)50, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)count, encoder.MessagesProcessedCount);
                Assert.Equal(1.56, Math.Round(encoder.AvgNotificationsPerMessage, 2));
            }
            else {
                Assert.InRange(count, 1270, 1280);
                Assert.Equal((uint)50, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)count, encoder.MessagesProcessedCount);
                Assert.Equal(0.04, Math.Round(encoder.AvgNotificationsPerMessage, 2));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeEventTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(1, true, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            Assert.Equal(1, networkMessages.Sum(m => m.Buffers.Count));
            Assert.Equal(1u, encoder.NotificationsProcessedCount);
            Assert.Equal(0u, encoder.NotificationsDroppedCount);
            Assert.Equal(1u, encoder.MessagesProcessedCount);
            Assert.Equal(1, encoder.AvgNotificationsPerMessage);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EncodeEventsTest(bool encodeBatchFlag) {
            var maxMessageSize = 256 * 1024;
            var messages = NetworkMessage.GenerateSampleSubscriptionNotifications(20, true, isSampleMode: true);

            var encoder = GetEncoder();
            var networkMessages = encoder.Encode(NetworkMessage.Create, messages, maxMessageSize, encodeBatchFlag);

            if (encodeBatchFlag) {
                Assert.Equal(1, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)20, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)1, encoder.MessagesProcessedCount);
                Assert.Equal(20, encoder.AvgNotificationsPerMessage);
            }
            else {
                Assert.Equal(210, networkMessages.Sum(m => m.Buffers.Count));
                Assert.Equal((uint)20, encoder.NotificationsProcessedCount);
                Assert.Equal((uint)0, encoder.NotificationsDroppedCount);
                Assert.Equal((uint)210, encoder.MessagesProcessedCount);
                Assert.Equal(0.10, Math.Round(encoder.AvgNotificationsPerMessage, 2));
            }
        }
    }
}
