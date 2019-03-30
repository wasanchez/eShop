using System;
namespace MicroServices.Common.General.Exceptions
{
    public class EventDeserializationException : Exception
    {
        public string EventTypeName { get; private set; }
        public string Metadata { get; private set; }

        public EventDeserializationException(string eventTypeName, string metadata)
            : base(string.Format("Could not deserialize {0} as an Event (metadata: {1})", eventTypeName, metadata))
        {
            EventTypeName = eventTypeName;
            Metadata = metadata;
        }
    }
}
