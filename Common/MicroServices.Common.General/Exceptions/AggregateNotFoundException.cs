using System;
namespace MicroServices.Common.General.Exceptions
{
    public class AggregateNotFoundException : AggregateBaseException
    {
        public AggregateNotFoundException(Guid id, Type type) :
           base(id, type, string.Format("Aggregate '{0}' (type {1}) was not found.", id, type.Name))
        {

        }
    }
}
