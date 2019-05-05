using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServices.Common.General;

namespace MicroServices.Common.Repository
{
    public abstract class Repository : IRepository
    {

        public abstract TAggregate GetById<TAggregate>(Guid id) where TAggregate : Aggregate;
        public abstract Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate;
        public abstract void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;
        public abstract Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;

        /// <summary>
        /// Calculates the expected version.
        /// </summary>
        /// <returns>The expected version.</returns>
        /// <param name="aggregate">Aggregate.</param>
        /// <param name="eventes">Eventes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected int CalculateExpectedVersion<T>(Aggregate aggregate, List<T> eventes)
        {
            return aggregate.Version - eventes.Count;
        }

        /// <summary>
        /// Builds the aggregate.
        /// </summary>
        /// <returns>The aggregate.</returns>
        /// <param name="eventes">Eventes.</param>
        /// <typeparam name="TAggregate">The 1st type parameter.</typeparam>
        protected TAggregate BuildAggregate<TAggregate>(IEnumerable<Event> eventes) where TAggregate: Aggregate
        {
            var instance = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            instance.LoadStateFromHistory(eventes);
            return instance;
        }
           
    }
}
