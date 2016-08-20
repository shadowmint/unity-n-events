using System;

namespace N.Package.Events.Internal
{
  /// AsyncEvent events are executed the next frame from when they are triggered.
  public class AsyncEvent : IStashableEvent
  {
    public EventHandler Source { get; set; }
    public object Event { get; set; }

    public void Update(float delta)
    {
    }

    public bool Ready { get { return true; } }
  }
}