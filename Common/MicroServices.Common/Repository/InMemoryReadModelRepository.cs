using System;
using System.Collections.Generic;
using MicroServices.Common.General;

namespace MicroServices.Common.Repository
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadModel
    {
        public InMemoryReadModelRepository()
        {
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(T model)
        {
            throw new NotImplementedException();
        }

        public void Update(T model)
        {
            throw new NotImplementedException();
        }
    }
}
