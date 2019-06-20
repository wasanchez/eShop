using System;
using System.Collections.Generic;
using MicroServices.Common.General;
using MicroServices.Common.General.Exceptions;
using MicroServices.Common.Repository;
using Products.Common.Models;

namespace Products.ReadModels.Service.Views
{
    public class ProductView : ReadModelAggregate
    {
        private readonly IReadModelRepository<ProductModel> repository;

        public ProductView(IReadModelRepository<ProductModel> repository)
        {
            this.repository = repository;
        }

        public ProductModel GetById(Guid id)
        {
            try
            {
                return repository.GetById(id);
            }
            catch
            {
                throw new ReadModelNotFoundException(id, typeof(ProductModel));
            }
        }

        public IEnumerable<ProductModel> GetAll()
        {
            return repository.GetAll();
        }
    }
}
