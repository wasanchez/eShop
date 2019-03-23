using System;
using MicroServices.Common.General;

namespace Products.Service.MicroServices.Products.Domain
{
    public class Product : Aggregate
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Price { get; set; }


        private Product () { }

        public Product (Guid id, string name, string description, double price)
        {
            ValidateName(name);

        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid name specified: cannot be empty.", new Exception("Invalid Product name"));
            }
        }
    }
}
