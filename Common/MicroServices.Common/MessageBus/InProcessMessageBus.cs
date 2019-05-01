using System;
using System.Collections.Generic;
using System.Threading;
using MicroServices.Common.General;

namespace MicroServices.Common.MessageBus
{
    public class InProcessMessageBus : IMessageBus
    {
        /// <summary>
        /// The command handlers.
        /// </summary>
        private readonly Dictionary<Type, List<Action<IMessage>>> commandHandlers = new Dictionary<Type, List<Action<IMessage>>>();

        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void RegisterHandler<T>(Action<T> handler) where T : IMessage
        {
            List<Action<IMessage>> handlers;

            if (!commandHandlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<IMessage>>();
                commandHandlers.Add(typeof(T), handlers);
            }
            handlers.Add((msg => handler((T)msg)));
        }


        /// <summary>
        /// Publish the specified event.
        /// </summary>
        /// <param name="event">Event.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Publish<T>(T @event) where T : Event
        {
            List<Action<IMessage>> handlers;
            if (!commandHandlers.TryGetValue(@event.GetType(), out handlers)) return;

            foreach (var item in handlers)
            {
                var handler = item;
                ThreadPool.QueueUserWorkItem(work => handler(@event));
            }
        }

        /// <summary>
        /// Send the specified command.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Send<T>(T command) where T : class, ICommand
        {

            List<Action<IMessage>> handler;

            if (!commandHandlers.TryGetValue(typeof(T), out handler))
            {
                if (handler.Count != 1) throw new InvalidOperationException("Cannot send command to more than one hanlder");
                handler[0](command);
            }else
            {
                throw new InvalidOperationException("No command habler registered");
            }
        }
    }
}
