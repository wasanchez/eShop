using System;
namespace MicroServices.Common.General.Exceptions
{
    public class AggregateVersionException : AggregateBaseException
    {
        public int AggregateVersion { get; protected set; }
        public int RequestedVersion { get; protected set; }

        public AggregateVersionException(Guid id, Type type, int aggregateVersion, int requestedVersion) :
            base(id, type, string.Format("Requested version {0} of aggregate '{1}' (type {2}) - aggregate version is {3}", requestedVersion, id, type.Name, aggregateVersion))
        {
            AggregateVersion = aggregateVersion;
            RequestedVersion = requestedVersion;
        }
    }
}
