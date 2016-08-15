using System;

namespace N.Package.Events.Internal
{
  /// The IEventBus is a common backend for all events to travel on.
  /// Multiple event listeners should all share a common IEventBus singleton.
  public interface IEventBus
  {
    /// Subscribe to an event type
    void Subscribe<T>(EventHandler source, Action<T> eventHandler) where T : class;

    /// Clear the subscription for an event handler
    void Unsubscribe<T>(EventHandler source, Action<T> eventHandler) where T : class;

    /// Trigger an event from a specific event handler
    void Trigger<T>(EventHandler source, T eventInstance) where T : class;

    /// Flush `maxEvents` pending events
    void Flush(int maxEvents);
  }
}