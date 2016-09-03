using System;
using System.Collections.Generic;
using N.Package.Events.Internal;
using UnityEngine;

namespace N.Package.Events
{
  /// EventStream is the common backend for events to travel on.
  public class EventStream : IDisposable
  {
    private static EventStream _default = null;

    private const int DefaultMaxEventThreshold = 100;

    private const int DefaultMaxStashSize = 100;

    /// This default event stream is used if an EventHandler is created without
    /// explicitly providing an event stream to use.
    public static EventStream Default
    {
      get { return _default ?? (_default = new EventStream()); }
      set { _default = value; }
    }

    /// Return the count of event handlers; largely just to help prevent leaks.
    public int Count
    {
      get { return _actions.Count; }
    }

    /// The stash for this event stream
    private readonly EventStash _stash;

    public EventStash Stash
    {
      get { return _stash; }
    }

    /// Set of all held event handlers
    private readonly List<ActionWrapper> _actions = new List<ActionWrapper>();

    /// Set this lock to true when we're inside a trigger statement.
    /// Trigger will raise an exception if too many events are raised. 
    private bool _locked = false;

    /// The count of triggered events since the lock was set.
    private int _eventsSinceLock = 0;

    /// The current 'max events' value
    private int _maxEvents;

    /// Is this registered with an event manager?
    private bool _isRegistered;

    public EventStream()
    {
      _stash = new EventStash(DefaultMaxStashSize);
    }

    public EventStream(int maxStashSize)
    {
      _stash = new EventStash(maxStashSize);
    }

    /// Subscribe to a new event.
    public EventBinding<T> Subscribe<T>(EventHandler source, Action<T> eventHandler) where T : class
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
      return new EventBinding<T>(source, eventHandler);
    }

    /// Remove an event subscription.
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

    /// Trigger an event on a specific event handler with an explicit event threshold
    public void Trigger<T>(EventHandler source, T eventInstance, int maxEventsThreshold) where T : class
    {
      var ownsLock = Lock(maxEventsThreshold);
      new EventDispatchTransaction<T>(_actions.ToArray()).Execute(source, eventInstance);
      Unlock(ownsLock);
    }

    /// Trigger an event on a specific event handler.
    public void Trigger<T>(EventHandler source, T eventInstance) where T : class
    {
      Trigger(source, eventInstance, DefaultMaxEventThreshold);
    }

    /// Clear all event handlers of type T with the given source.
    public void Clear<T>(EventHandler source) where T : class
    {
      _actions.RemoveAll(x => x.Is<T>(source));
    }

    /// Clear all event handlers with the given source.
    public void Clear(EventHandler source)
    {
      _actions.RemoveAll(x => x.Source.Equals(source));
    }

    /// Clear everything!
    public void Clear()
    {
      _actions.Clear();
    }

    /// Register this event stream with the event manager, eg. for deferred events
    public void RegisterStream()
    {
      if (_isRegistered) return;
      EventManager.Instance.Register(this, () => { _isRegistered = false; });
      _isRegistered = true;
    }

    /// Increment and possibly set the event lock
    private bool Lock(int maxEvents)
    {
      if (!_locked)
      {
        _locked = true;
        _eventsSinceLock = 1;
        _maxEvents = maxEvents;
        return true;
      }
      _eventsSinceLock += 1;
      if (_eventsSinceLock > _maxEvents)
      {
        Unlock(true);
        throw new Exception(string.Format("Too many events raised in a single Trigger statement ({0})", maxEvents));
      }
      return false;
    }

    /// Decrement and possibly clear the event lock
    private void Unlock(bool ownsLock)
    {
      if (ownsLock)
      {
        _locked = false;
        _eventsSinceLock = 0;
      }
    }

    public void Dispose()
    {
      if (_isRegistered)
      {
        EventManager.Instance.Deregister(this);
      }
    }
  }
}