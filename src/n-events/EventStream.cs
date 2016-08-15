using System;
using System.Collections.Generic;
using N.Package.Events.Internal;

namespace N.Package.Events
{
  /// EventStream is the common backend for events to travel on.
  public class EventStream : IEventBus, IDisposable
  {
    /// Set of all held event handlers
    private List<ActionWrapper> _actions = new List<ActionWrapper>();

    /// Set of all pending events
    private Queue<EventWrapper> _events = new Queue<EventWrapper>();

    public EventStream()
    {
      EventManager.Instance.Register(this);
    }

    public void Subscribe<T>(EventHandler source, Action<T> eventHandler) where T : class
    {
      if (source == null)
      {
        throw new Exception("Invalid event source (null)");
      }
      if (eventHandler == null)
      {
        throw new Exception("Invalid event handler (null)");
      }
      _actions.Add(new ActionWrapper<T> {Action = eventHandler, Source = source});
    }

    public void Unsubscribe<T>(EventHandler source, Action<T> eventHandler) where T : class
    {
      if (source == null)
      {
        throw new Exception("Invalid event source (null)");
      }
      if (eventHandler == null)
      {
        throw new Exception("Invalid event handler (null)");
      }
      _actions.RemoveAll(a => a.Is(source, eventHandler));
    }

    public void Trigger<T>(EventHandler source, T eventInstance) where T : class
    {
      _events.Enqueue(new EventWrapper<T>() { Event = eventInstance, Source = source });
    }

    public void Flush(int maxEvents)
    {
      var count = 0;
      while ((_events.Count > 0) && (count < maxEvents))
      {
        var next = _events.Dequeue();
        foreach (var action in _actions)
        {
          if (action.Is(next.Source, next.EventType))
          {
            action.Dispatch(next);
          }
        }
        count += 1;
      }
    }

    public void Dispose()
    {
      EventManager.Instance.Deregister(this);
    }
  }
}