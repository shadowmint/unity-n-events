using System;
using N.Package.Events.Internal;

namespace N.Package.Events
{
  /// Helper methods for EventHandler additional functionality.
  public static class EventHandlerExtensions
  {
    /// Add an event that is only triggered after a certain amount of time has elapsed.
    public static void TriggerDeferred<T>(this EventHandler self, T eventInstance, float delay)
    {
      self.Stream.RegisterStream();
      self.Stream.Stash.Push(new DeferredEvent()
      {
        Source = self,
        Event = eventInstance,
        Delay = delay
      });
    }

    /// Add an event that is only triggered after at least one frame has passed.
    public static void TriggerAsync<T>(this EventHandler self, T eventInstance)
    {
      self.Stream.RegisterStream();
      self.Stream.Stash.Push(new AsyncEvent()
      {
        Source = self,
        Event = eventInstance,
      });
    }
  }
}