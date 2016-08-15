using System;

namespace N.Package.Events.Internal
{
  // Untyped common base to ActionWrapper
  public abstract class ActionWrapper
  {
    public abstract bool Is<T>(EventHandler source) where T : class;
    public abstract bool Is<T>(EventHandler source, Action<T> handler) where T : class;
    public abstract bool Is(EventHandler source, Type eventType);
    public abstract void Dispatch(EventWrapper eventInstance);
  }

  // Wrap action types
  class ActionWrapper<T> : ActionWrapper where T : class
  {
    public EventHandler Source { get; set; }
    public Action<T> Action { get; set; }

    public override bool Is<TEvent>(EventHandler source)
    {
      return Source.Equals(source) && Action is Action<TEvent>;
    }

    public override bool Is<TEvent>(EventHandler source, Action<TEvent> handler)
    {
      return Is<TEvent>(source) && (Action.Equals(handler));
    }

    public override bool Is(EventHandler source, Type eventType)
    {
      return Source.Equals(source) && typeof(T) == eventType;
    }

    public override void Dispatch(EventWrapper eventInstance)
    {
      Action(eventInstance.As<T>());
    }
  }
}