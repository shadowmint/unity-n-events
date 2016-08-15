using System;

namespace N.Package.Events.Internal
{
  // Untyped common base to ActionWrapper
  public abstract class EventWrapper
  {
    public abstract Type EventType { get; }
    public abstract TEvent As<TEvent>() where TEvent : class;

    public EventHandler Source { get; set; }
  }

  // Wrap action types
  class EventWrapper<T> : EventWrapper where T : class
  {
    public T Event { get; set; }

    public override Type EventType
    {
      get { return typeof(T); }
    }

    public override TEvent As<TEvent>()
    {
      return Event as TEvent;
    }
  }
}