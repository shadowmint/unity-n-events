using System;
using N.Package.Events.Internal;

namespace N.Package.Events
{
  /// EventHandlers are used to subscribe and trigger events.
  public class EventHandler
  {
    private readonly IEventBus _eventBus;

    public EventHandler()
    {
      _eventBus = EventManager.Instance.DefaultEventStream;
    }

    public EventHandler(IEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    /// Subscribe to an event type
    public void AddEventHandler<T>(Action<T> eventHandler) where T : class
    {
      _eventBus.Subscribe(this, eventHandler);
    }

    /// Clear the subscription for an event handler
    public void RemoveEventHandler<T>(Action<T> eventHandler) where T : class
    {
      _eventBus.Unsubscribe(this, eventHandler);
    }

    /// Trigger an event
    public void Trigger<T>(T eventInstance) where T : class
    {
      _eventBus.Trigger(this, eventInstance);
    }
  }
}