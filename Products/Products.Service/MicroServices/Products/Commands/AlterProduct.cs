using System;
using MicroServices.Common.General;

namespace Products.Service.MicroServicces.Product.Commands
{
    public class AlterProduct : ICommand
    {
        public AlterProduct(Guid id, string newName, string newDescription, decimal newPrice, int originalVersion)
        {
            Id = id;
            NewName = newName;
            NewDescription = newDescription;
            NewPrice = newPrice;
            OriginalVersion = originalVersion;
        }

        public Guid Id { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public decimal NewPrice { get; set; }
        public int OriginalVersion { get; set; }
    }
}
