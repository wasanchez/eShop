using System;
using EasyNetQ;

namespace MicroServices.Common.MessageBus
{
    /// <summary>
    /// Transient subscriber.
    /// </summary>
    public class TransientSubscriber : IDisposable
    {

        private ISubscriptionResult subcription;

        private readonly Action handler;
        private readonly string topic;
        private readonly string listenerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/> class.
        /// </summary>
        /// <param name="listenerName">Listener name.</param>
        /// <param name="topic">Topic.</param>
        /// <param name="handler">Handler.</param>
        public TransientSubscriber(string listenerName, string topic, Action handler)
        {
            this.listenerName = listenerName;
            this.topic = topic;
            this.handler = handler;
        }

        /// <summary>
        /// Initializes the bus.
        /// </summary>
        private void InitializeBus()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            subcription = bus.Subscribe<PublishedMessage>(listenerName, m => handler(), q => q.WithTopic(topic));
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/>. The <see cref="Dispose"/> method leaves
        /// the <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/> so the garbage collector can reclaim the
        /// memory that the <see cref="T:MicroServices.Common.MessageBus.TransientSubscriber"/> was occupying.</remarks>
        public void Dispose()
        {
            subcription.Dispose();
        }
    }
}
