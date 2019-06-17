using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using MicroServices.Common.General;
using MicroServices.Common.General.Exceptions;
using MicroServices.Common.MessageBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicroServices.Common.Repository
{
    public class EventStoreRepository : IRepository
    {
        private const string EventClrTypeHeader = "EventClrTypeHeader";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private static readonly JsonSerializerSettings serializationSettings;
        private readonly IEventStoreConnection eventStoreConnection;
        private readonly IMessageBus bus;

        /// <summary>
        /// Initializes the <see cref="T:MicroServices.Common.Repository.EventStoreRepository"/> class.
        /// </summary>
        static EventStoreRepository()
        {
            serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MicroServices.Common.Repository.EventStoreRepository"/> class.
        /// </summary>
        /// <param name="eventStoreConnection">Event store connection.</param>
        /// <param name="bus">Bus.</param>
        public EventStoreRepository(IEventStoreConnection eventStoreConnection, IMessageBus bus)
        {
            this.eventStoreConnection = eventStoreConnection;
            this.bus = bus;

        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <returns>The by identifier.</returns>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : Aggregate
        {
            return GetByIdAsync<TAggregate>(id).Result;
        }

        /// <summary>
        /// Gets the Aggregate by identifier async.
        /// </summary>
        /// <returns>The by identifier async.</returns>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate
        {
            return await GetByIdAsync<TAggregate>(id, int.MinValue);
        }

        /// <summary>
        /// Gets an Aggregate by identifier async.
        /// </summary>
        /// <returns>The Aggregate by identifier async.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="version">Version.</param>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id, int version) where TAggregate : Aggregate
        {
            if (version <= 0)
                throw new InvalidOperationException("Cannot get version <= 0");

            var streamName = AggregateIdToStreamName(typeof(TAggregate), id);
            var aggregate = ConstructorAggregate<TAggregate>();


            int sliceStart = 0;
            StreamEventsSlice currentSlice;

            do
            {
                int sliceCount = sliceStart + ReadPageSize <= version ? ReadPageSize : version - sliceStart + 1;

                currentSlice = await eventStoreConnection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, true);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                    throw new AggregateNotFoundException(id, typeof(TAggregate));
                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                    throw new AggregateDeletedException(id, typeof(TAggregate));

                sliceStart = (int) currentSlice.NextEventNumber;

                var history = new List<Event>();
                foreach (var item in currentSlice.Events)
                {
                    history.Add(DeserailizeEvent(item.Event.Metadata, item.Event.Data));
                }
                aggregate.LoadStateFromHistory(history);

            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);

            if (aggregate.Version != version && version < Int32.MaxValue)
                throw new AggregateVersionException(id, typeof(TAggregate), aggregate.Version, version);

            return aggregate;
        }

        public void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            SaveAsync<TAggregate>(aggregate).Wait();
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            var commitHeaders = new Dictionary<string, object>
           {
               {CommitIdHeader, aggregate.Id},
               {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName }
           };

            var streamName = AggregateIdToStreamName(aggregate.GetType(), aggregate.Id);
            var eventsToPublish = aggregate.GetUncommittedEvents();
            var newEvents = eventsToPublish.Cast<object>().ToList();
            var originalVersion = aggregate.Version - newEvents.Count();
            var expectedVersion = originalVersion == -1 ? ExpectedVersion.NoStream : originalVersion;

            var eventsToSave = newEvents.Select(e => ToEventData(aggregate.Id, e, commitHeaders)).ToList();

            if (eventsToSave.Count() < WritePageSize)
            {
                await eventStoreConnection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
            }

        }

        private string AggregateIdToStreamName(Type type, Guid id)
        {
            //javascript name convention
            return string.Format("{0}-{1}", char.ToLower(type.Name[0]) + type.Name.Substring(1), id.ToString("N"));
        }

        /// <summary>
        /// Constructors the aggregate.
        /// </summary>
        /// <returns>The aggregate.</returns>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        private static TAggregate ConstructorAggregate<TAggregate>()
        {
            return (TAggregate)Activator.CreateInstance(typeof(Aggregate), true);
        } 

        /// <summary>
        /// Deserailizes the event.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="metadata">Metadata.</param>
        /// <param name="data">Data.</param>
        private static Event DeserailizeEvent(byte[] metadata, byte[] data)
        {
            var metadataString = Encoding.UTF8.GetString(metadata);
            var eventCtrlTypeName = JObject.Parse(metadataString).Property(EventClrTypeHeader).Value;
            var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventCtrlTypeName)) as Event;
            if (@event == null)
                throw new EventDeserializationException((string)eventCtrlTypeName, metadataString);

            return @event;
        }

        private static EventData ToEventData(Guid id, object @event, Dictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, serializationSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    EventClrTypeHeader, @event.GetType().AssemblyQualifiedName

                }
            };

            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@eventHeaders, serializationSettings));
            var typeName = @event.GetType().Name;

            return new EventData(id, typeName, true, data, metadata);
        }
    }
}
