using System;
using System.Collections.Generic;

namespace N.Package.Events
{
  /// EventContext are cleaned up when disposed and unbind all event handlers.
  public class EventContext : IDisposable
  {
    private readonly List<GenericEventBinding> _events = new List<GenericEventBinding>();

    public void Dispose()
    {
      _events.ForEach(i => i.Dispose());
      _events.Clear();
    }

    public void Add(GenericEventBinding eventBinding)
    {
      _events.Add(eventBinding);
    }
  }
}