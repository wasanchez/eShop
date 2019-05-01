using System;
using System.Threading.Tasks;
using MicroServices.Common.General;

namespace MicroServices.Common.Repository
{
    public class InMemoryRepository : IRepository
    {
        public InMemoryRepository()
        {
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
    }
}
