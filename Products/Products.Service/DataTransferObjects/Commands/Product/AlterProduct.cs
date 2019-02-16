using System;
using MicroServices.Common.General;

namespace Products.Service.DataTransferObjects.Commands.Product
{
    public class AlterProductCommand
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public decimal NewPrice { get; set; }
        public int OriginalVersion { get; set; }
    }
}
