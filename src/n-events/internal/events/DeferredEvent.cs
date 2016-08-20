using System;

namespace N.Package.Events.Internal
{
  /// DeferredEvent events are executed after a specific time interval has elapsed.
  public class DeferredEvent : IStashableEvent
  {
    public EventHandler Source { get; set; }
    public object Event { get; set; }
    public float Delay { get; set; }
    private float _elpased;

    public void Update(float delta)
    {
      _elpased += delta;
    }

    public bool Ready
    {
      get { return _elpased >= Delay; }
    }
  }
}