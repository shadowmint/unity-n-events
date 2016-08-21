using System;
using N.Package.Events.Internal;

namespace N.Package.Events.Internal
{
  /// Common interface for stashable types.
  public interface IStashableEvent
  {
    EventHandler Source { get; set; }
    object Event { get; set; }
    void Update(float delta);
    bool Ready { get; }
  }
}