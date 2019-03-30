using System;
namespace MicroServices.Common.General.Exceptions
{
    public class AggregateDeletedException :AggregateBaseException
    {
         
        public AggregateDeletedException(Guid id, Type type) : 
            base (id, type, string.Format("Aggregate '{0}' (type {1}) was deleted.", id, type.Name))
        {

        }
    }
}
