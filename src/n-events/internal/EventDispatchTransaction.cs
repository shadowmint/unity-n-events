using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace N.Package.Events.Internal
{
  /// When a new event arrives, dispatch that event to event listeners using this safe transaction object.
  class EventDispatchTransaction<T>
  {
    private readonly ActionWrapper[] _actions;

    public EventDispatchTransaction(List<ActionWrapper> actions)
    {
      _actions = actions.ToArray();
    }

    public void Execute(EventHandler source, T eventInstance)
    {
      if ((_actions == null) || (_actions.Length <= 0)) return;
      foreach (var action in _actions)
      {
        if (action.Is(source, eventInstance.GetType()))
        {
          action.Dispatch(eventInstance);
        }
      }
    }
  }
}