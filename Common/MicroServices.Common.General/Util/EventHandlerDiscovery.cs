using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroServices.Common.General.Util
{
    public class EventHandlerDiscovery
    {
        public Dictionary<Type,Aggregate> Handlers { get; private set; }

        public EventHandlerDiscovery()
        {
            Handlers = new Dictionary<Type, Aggregate>();
        }

        public EventHandlerDiscovery Scan(Aggregate aggregate)
        {
            var handlerInterface = typeof(IHandler<>);
            var aggType = aggregate.GetType();

            var instances = from i in aggType.GetInterfaces()
                            where (i.IsGenericType && handlerInterface.IsAssignableFrom(i.GetGenericTypeDefinition()))
                            select i.GenericTypeArguments[0];

            foreach (var i in instances)
            {
                Handlers.Add(i, aggregate);
            }

            return this;
        }
    }
}
