using System;
using System.Collections.Generic;
using MicroServices.Common.General;

namespace MicroServices.Common.Repository
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadModel
    {
        private readonly Dictionary<Guid, T> items = new Dictionary<Guid, T>();
         

        public IEnumerable<T> GetAll()
        {
            return items.Values;
        }

        public T GetById(Guid id)
        {
            return items[id];
        }

        public void Insert(T model)
        {
            items.Add(model.Id, model);
        }

        public void Update(T model)
        {
            items[model.Id] = model;
        }
    }
}
