using System;
using System.Collections.Generic;
using System.Linq;
using MicroServices.Common.General;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MicroServices.Common.Repository
{
    /// <summary>
    /// Redis repository.
    /// </summary>
    public class RedisRepository<T> : IReadModelRepository<T> where T : ReadModel 
    {
        private readonly IDatabase database;

        public RedisRepository(IDatabase database)
        {
            this.database = database;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The all.</returns>
        public IEnumerable<T> GetAll()
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = database.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get).Result;
            var models = result.Select(item => JsonConvert.DeserializeObject<T>(item)).AsEnumerable<T>();
            return models;
        }

        /// <summary>
        /// Gets the model by identifier.
        /// </summary>
        /// <returns>The by identifier.</returns>
        /// <param name="id">Identifier.</param>
        public T GetById(Guid id)
        {

            var result = database.StringGetAsync(Key(id)).Result;
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// Insert the specified model.
        /// </summary>
        /// <param name="model">Model.</param>
        public void Insert(T model)
        {
            var serialised = JsonConvert.SerializeObject(model);
            var key = Key(model.Id);
            var transaction = database.CreateTransaction();
            transaction.StringSetAsync(key, serialised);
            transaction.SetAddAsync(SetName(), model.Id.ToString("N"));
            var committed = transaction.ExecuteAsync().Result;
            if (!committed) throw new ApplicationException("Could not commit isert model.");
        }

        /// <summary>
        /// Update the specified model.
        /// </summary>
        /// <param name="model">Model.</param>
        public void Update(T model)
        {
            var key = Key(model.Id);
            var serialised = JsonConvert.SerializeObject(model);
            database.StringSet(key, serialised, when: When.Exists);

        }

        /// <summary>
        /// Create a Key model.
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="id">Identifier.</param>
        private string Key(Guid id)
        {
            return InstanceName() + id.ToString("N");
        }


        private string InstanceName()
        {
            var type = typeof(T);
            return string.Format("{0}:", type.FullName);
        }

        private string SetName()
        {
            return string.Format("{0}Set", InstanceName());
        }
    }
}
