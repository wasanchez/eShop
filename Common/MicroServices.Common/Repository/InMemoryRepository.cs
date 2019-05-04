using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServices.Common.General;
using MicroServices.Common.MessageBus;
using Newtonsoft.Json;

namespace MicroServices.Common.Repository
{
    public class InMemoryRepository : IRepository
    {
        
        private readonly IMessageBus bus;
        private readonly List<Event> latestEventes = new List<Event>();
        private readonly JsonSerializerSettings jsonSerilizeSettings;

        public Dictionary<Guid,List<string>> EventStore { get; private set; }

        public List<Event> LatestEventes => latestEventes;

        public InMemoryRepository(IMessageBus bus)
        {
            this.bus = bus;
            this.EventStore = new Dictionary<Guid, List<string>>();
            jsonSerilizeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }

        public Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }

        public void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }

        public string Serialize(Event arg)
        {
            return JsonConvert.SerializeObject(arg, jsonSerilizeSettings);
        }
    }
}
