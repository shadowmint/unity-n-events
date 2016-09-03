using System;

namespace N.Package.Events
{
  /// EventHandlers are used to subscribe and trigger events.
  public class EventHandler : IDisposable
  {
    private readonly EventStream _eventStream;

    public EventHandler()
    {
      _eventStream = EventStream.Default;
    }

    public EventHandler(EventStream eventStream)
    {
      _eventStream = eventStream;
    }

    /// Subscribe to an event type
    public EventBinding<T> AddEventHandler<T>(Action<T> eventHandler) where T : class
    {
      return _eventStream.Subscribe(this, eventHandler);
    }

    /// Subscribe to an event type, and associate the binding with a context
    public void AddEventHandler<T>(Action<T> eventHandler, EventContext context) where T : class
    {
      context.Add(_eventStream.Subscribe(this, eventHandler));
    }

    /// Clear the subscription for an event handler
    public void RemoveEventHandler<T>(Action<T> eventHandler) where T : class
    {
      _eventStream.Unsubscribe(this, eventHandler);
    }

    /// Clear all event handlers of a specific type on an object.
    public void ClearEventHandlers<T>() where T : class
    {
      _eventStream.Clear<T>(this);
    }

    /// Trigger an event
    public void Trigger<T>(T eventInstance) where T : class
    {
      _eventStream.Trigger(this, eventInstance);
    }

    /// Trigger an event with an explicit max recursive depth
    public void Trigger<T>(T eventInstance, int maxChildEvents) where T : class
    {
      _eventStream.Trigger(this, eventInstance, maxChildEvents);
    }

    /// Return the actual event stream
    public EventStream Stream
    {
      get { return _eventStream; }
    }

    public void Dispose()
    {
      _eventStream.Clear(this);
    }
  }
}