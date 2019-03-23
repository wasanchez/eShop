using System;
using MicroServices.Common.General;
using Newtonsoft.Json;

namespace Products.Common.Events
{
    public class ProducCreatedtEvent : Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        [JsonConstructor]
        private ProducCreatedtEvent(Guid id, string name, string description, decimal price, int version) 
        : this (id, name, description, price) 
        {
           
            Version = version;
        }

        public ProducCreatedtEvent(Guid id, string name, string description, decimal price)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }

    }

    public class ProductDescriptionChanged : Event
    {
        public ProductDescriptionChanged(Guid id, string newDescription)
        {
            Id = id;
            NewDescription = newDescription;
        }

        [JsonConstructor]
        private ProductDescriptionChanged(Guid id, string newDescription, int version) : this (id, newDescription)
        {
            Version = version;
        }

        public string NewDescription { get; private set; }

    }

    public class ProductNameChanged : Event
    {
        public ProductNameChanged(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }

        [JsonConstructor]
        private ProductNameChanged (Guid id, string newName, int version) : this (id, newName)
        {
            Version = version;
        }

        public string NewName { get; private set; }

    }

    public class ProductPriceChanged : Event
    {
        public decimal NewPrice  { get; private set; }

        public ProductPriceChanged(Guid id, decimal price)
        {
            Id = id;
            NewPrice = price;

        }

        [JsonConstructor]
        private ProductPriceChanged (Guid id, decimal price, int version) : this(id, price)
        {
            Version = version;
        }
    }
}
