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
    private readonly List<ActionWrapper> _actions;

    public EventDispatchTransaction(List<ActionWrapper> actions)
    {
      _actions = actions;
    }

    public void Execute(EventHandler source, T eventInstance)
    {
      if ((_actions != null) && (_actions.Count > 0))
      {
        for (int i = 0; i < _actions.Count; i++)
        {
          if (_actions[i].Is(source, eventInstance.GetType()))
          {
            _actions[i].Dispatch(eventInstance);
          }
        }
      }
    }
  }
}