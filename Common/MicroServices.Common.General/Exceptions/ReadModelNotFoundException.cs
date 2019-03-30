using System;
namespace MicroServices.Common.General.Exceptions
{
    public class ReadModelNotFoundException : AggregateBaseException
    {
        public ReadModelNotFoundException(Guid id, Type type)
            : base (id, type, string.Format("ReadModel '{0}' (type {1}) not was not found.", id, type.Name))
        {
        }
    }
}
