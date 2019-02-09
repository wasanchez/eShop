using System;
using MicroServices.Common.General;
using Newtonsoft.Json;

namespace Products.Common.Events
{
    public class ProducCreatedtEvent : Event
    {
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

        public ProducCreatedtEvent()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


    }
}
