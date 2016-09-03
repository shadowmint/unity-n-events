using System;

namespace N.Package.Events
{
  /// EventBindings remove the handler object associated with them when Dispose() is called.
  /// Notice that these objects are *not* IDisposable; use an EventContext for that.
  public class EventBinding<T> : GenericEventBinding where T : class
  {
    private readonly EventHandler _source;
    private readonly Action<T> _handler;

    public EventBinding(EventHandler source, Action<T> handler)
    {
      _source = source;
      _handler = handler;
    }

    protected override void RemoveEventHandler()
    {
      _source.RemoveEventHandler(_handler);
    }
  }

  /// Common base class for event bindings
  public abstract class GenericEventBinding
  {
    protected bool Bound = true;
    protected abstract void RemoveEventHandler();

    public void Dispose()
    {
      if (!Bound) return;
      Bound = false;
      RemoveEventHandler();
    }
  }
}