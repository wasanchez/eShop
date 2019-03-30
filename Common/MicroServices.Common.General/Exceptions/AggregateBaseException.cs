using System;
namespace MicroServices.Common.General.Exceptions
{
    public class AggregateBaseException : Exception
    {
        public Guid Id { get; protected set; }
        public Type Type { get; protected set; }

        public AggregateBaseException(Guid id, Type type, string message) : base (message)
        {
            Id = id;
            Type = type;
        }
    }
}
