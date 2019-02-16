using System.Collections.Generic;

namespace MicroServices.Common.General
{

    public abstract class Aggregate
    {

        internal readonly List<Event> _events = new List<Event>();
        private int _version = -1;

        public int Version { get => _version; internal set => _version = value; }

        public IEnumerable<Event> GetUncommittedEvents()
        {
            return this._events;
        }

        public void MarkEventsAsCommitted()
        {
            this._events.Clear();
        }

        protected internal void ApplyEvent(Event @event)
        {
            this.ApplyEvent(@event, true);

        }

        protected virtual void ApplyEvent(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew)
            {
                @event.Version = ++Version;
                this._events.Add(@event);
            }
            else
            {
                Version = @event.Version;
            }
        }


    }

    public abstract class ReadModelAggregate : Aggregate
    {
        protected internal void ApplyEvent(Event @event, int version)
        {
            @event.Version = version;
            ApplyEvent(@event, true);
        }

        protected override void ApplyEvent(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew)
            {
                this._events.Add(@event);
            }
            Version++;
        }
    }

}
