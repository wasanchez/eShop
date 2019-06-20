using System;
using EasyNetQ;
using MicroServices.Common.General;
using Newtonsoft.Json;

namespace MicroServices.Common.MessageBus
{
    /// <summary>
    /// Rubbit mq bus.
    /// </summary>
    public class RabbitMqBus : IMessageBus
    {
        private readonly IBus bus;

        /// <summary>
        /// Gets the bus.
        /// </summary>
        /// <value>The bus.</value>
        public IBus Bus { get { return bus; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MicroServices.Common.MessageBus.RubbitMqBus"/> class.
        /// </summary>
        /// <param name="bus">Bus.</param>
        public RabbitMqBus(IBus bus)
        {
            if (bus == null) throw new NullReferenceException("bus");
            this.bus = bus;
        }

        /// <summary>
        /// Publish the specified event.
        /// </summary>
        /// <param name="event">Event.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Publish<T>(T @event) where T : Event
        {
            if (bus == null) throw new ApplicationException("Rubbit MQ is not intialized yet.");

            var innerMessage = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType();
            var typeName = eventType.ToString();
            var topicName = typeName.Substring(0, typeName.IndexOf(".", StringComparison.CurrentCulture));

            var message = new PublishedMessage { MessageTypeName = eventType.AssemblyQualifiedName, SerialisedMessage = innerMessage };

            bus.PublishAsync(message, topicName).Wait();

        }

        void IMessageBus.Send<T>(T command)
        {
            throw new NotImplementedException();
        }
    }
}
