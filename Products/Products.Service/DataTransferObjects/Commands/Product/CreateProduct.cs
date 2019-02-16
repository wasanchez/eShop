using System;
namespace Products.Service.DataTransferObjects.Commands.Product
{
    public class CreateProductCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }      
    }
}
