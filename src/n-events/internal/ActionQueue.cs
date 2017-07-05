using System;
using System.Collections.Generic;

namespace N.Package.Events.Internal
{
  internal class ActionQueue
  {
    public const int DefaultReallocateThreshold = 10;

    private readonly int _reallocateThreshold;

    private int _size;

    public ActionWrapper[] Actions { get; private set; }

    public int Length
    {
      get { return _size; }
    }

    public ActionQueue()
    {
      _reallocateThreshold = DefaultReallocateThreshold;
    }

    public ActionQueue(int reallocateThreshold)
    {
      _reallocateThreshold = reallocateThreshold;
    }

    private void Resize(int size)
    {
      if (Actions == null || size > Actions.Length || Math.Abs(Actions.Length - size) > _reallocateThreshold)
      {
        Actions = new ActionWrapper[size];
      }
      _size = size;
    }

    public void CopyActionList(List<ActionWrapper> actions)
    {
      Resize(actions.Count);
      actions.CopyTo(Actions, 0);
    }
  }
}