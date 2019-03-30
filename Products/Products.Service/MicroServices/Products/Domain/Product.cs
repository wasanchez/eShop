using System;
using MicroServices.Common.General;
using Products.Common.Events;


namespace Products.Service.MicroServices.Products.Domain
{
    public class Product : Aggregate
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; set; }


        private Product () { }

        public Product (Guid id, string name, string description, decimal price)
        {
            ValidateName(name);
            Apply(new ProductCreated(id, name, description, price));

        }

        private void Apply(ProductCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            Description = e.Description;
            Price = e.Price;
        }

        private void Apply(ProductPriceChanged e)
        {
            Price = e.NewPrice;
        }

        private void Apply(ProductNameChanged e)
        {
            Name = e.NewName;
        }

        private void Apply(ProductDescriptionChanged e)
        {
            Description = e.NewDescription;
        }

        public void ChangeName(string newName, int originalVersion)
        {
            ValidateVersion(originalVersion);
            ValidateName(newName);

            ApplyEvent(new ProductNameChanged(Id, newName));
        }

        public void ChangePrice(decimal newPrice, int originalVersion)
        {
            ValidateVersion(originalVersion);
            ApplyEvent(new ProductPriceChanged(Id, newPrice));
        }

        public void ChangeDescription(string newDescription, int orginalVersion)
        {
            ValidateVersion(orginalVersion);
            ApplyEvent(new ProductDescriptionChanged(Id, newDescription));
        }

        void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid name specified: cannot be empty.", new Exception("Invalid Product name"));
            }
        }

        void ValidateVersion(int version)
        {
            if (Version != version)
            {
                throw new ArgumentOutOfRangeException("version", "Invalid version specified: the version is out of sync.");
            }
        }

    }
}
