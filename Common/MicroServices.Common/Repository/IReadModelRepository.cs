using System;
using System.Collections.Generic;
using MicroServices.Common.General;

namespace MicroServices.Common.Repository
{
    public interface IReadModelRepository<T> where T : ReadModel
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Insert(T model);
        void Update(T model);
    }
}
