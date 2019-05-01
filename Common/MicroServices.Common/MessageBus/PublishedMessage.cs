namespace MicroServices.Common.MessageBus
{
    public class PublishedMessage
    {
        public string MessageTypeName { get; set; }
        public string SerialisedMessage { get; set; }
    }
}
