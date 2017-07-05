using System;
using System.Collections.Generic;
using N.Package.Events.Internal;

namespace N.Package.Events.Internal
{
  /// EventStash allows events to be buffered before being triggered.
  public class EventStash
  {
    private readonly Queue<IStashableEvent> _events = new Queue<IStashableEvent>();

    private readonly int _maxStashSize;

    public EventStash(int maxStashSize)
    {
      _maxStashSize = maxStashSize;
    }

    /// Stash an event
    public void Push(IStashableEvent eventInstance)
    {
      if (_events.Count >= _maxStashSize)
      {
        throw new Exception(string.Format("Deferred event stash is full ({0})", _maxStashSize));
      }
      _events.Enqueue(eventInstance);
    }

    /// Update stashed events
    public void Update(float delta)
    {
      foreach (var item in _events)
      {
        item.Update(delta);
      }
    }

    /// Yield a set of events which are ready to dispatch
    public IEnumerable<IStashableEvent> Ready
    {
      get
      {
        var count = _events.Count;
        for (var i = 0; i < count; ++i)
        {
          var next = _events.Dequeue();
          if (next.Ready)
          {
            yield return next;
          }
          else
          {
            _events.Enqueue(next);
          }
        }
      }
    }

    public void Clear()
    {
      _events.Clear();
    }
  }
}