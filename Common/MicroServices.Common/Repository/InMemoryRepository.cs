using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServices.Common.General;
using MicroServices.Common.General.Exceptions;
using MicroServices.Common.MessageBus;
using Newtonsoft.Json;

namespace MicroServices.Common.Repository
{
    public class InMemoryRepository : Repository
    {
        
        private readonly IMessageBus bus;
        private readonly List<Event> latestEventes = new List<Event>();
        private readonly JsonSerializerSettings jsonSerilizerSettings;

        public Dictionary<Guid,List<string>> EventStore { get; private set; }

        public List<Event> LatestEventes => latestEventes;

        public InMemoryRepository(IMessageBus bus)
        {
            this.bus = bus;
            this.EventStore = new Dictionary<Guid, List<string>>();
            jsonSerilizerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public override TAggregate GetById<TAggregate>(Guid id)
        {
            if (EventStore.ContainsKey(id))
            {
                var events = EventStore[id];
                var deserilizedEvents = events.Select(e => JsonConvert.DeserializeObject(e, jsonSerilizerSettings) as Event);
                return BuildAggregate<TAggregate>(deserilizedEvents);
            }

            throw new AggregateNotFoundException(id, typeof(TAggregate));
        }

        /// <summary>
        /// Save the specified aggregate and publish the events through the bus.
        /// </summary>
        /// <param name="aggregate">Aggregate.</param>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        public override void Save<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.GetUncommittedEvents().ToList();
            var eventsSerialized = eventsToSave.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);

            if (expectedVersion < 0)
            {
                EventStore.Add(aggregate.Id, eventsSerialized);
            }else
            {
                var existingEvents = EventStore[aggregate.Id];
                var currentVersion = existingEvents.Count - 1;
                if (currentVersion != expectedVersion)
                {
                    throw new AggregateVersionException(aggregate.Id, typeof(TAggregate), currentVersion, expectedVersion);
                }
                existingEvents.AddRange(eventsSerialized);
            }
            latestEventes.AddRange(eventsToSave);

            PublishEvents(latestEventes);
            aggregate.MarkEventsAsCommitted();
        }

        private void PublishEvents(List<Event> events)
        {
            if (bus == null) return;
            foreach (var e in events)
            {
                bus.Publish(e);
            }
        }

        public string Serialize(Event arg)
        {
            return JsonConvert.SerializeObject(arg, jsonSerilizerSettings);
        }

        public override Task<TAggregate> GetByIdAsync<TAggregate>(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task SaveAsync<TAggregate>(TAggregate aggregate)
        {
            throw new NotImplementedException();
        }

        public void AddEvents(Dictionary<Guid, IEnumerable<Event>> eventsForAggregates)
        {
            foreach (var eventsForAggregate in eventsForAggregates)
            {
                EventStore.Add(eventsForAggregate.Key, eventsForAggregate.Value.Select(Serialize).ToList());
            }
        }
    }
}
