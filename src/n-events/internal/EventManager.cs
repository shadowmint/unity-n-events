using System.Collections.Generic;
using UnityEngine;

namespace N.Package.Events.Internal
{
  public class EventManager : MonoBehaviour
  {
    [Tooltip("The maximum number of events to allow per frame")]
    public int MaxEventsPerFrame = 1000;

    private static EventManager _instance;

    /// Set of all event bus instances
    private readonly List<IEventBus> _eventStream = new List<IEventBus>();

    /// Return the singleton instance.
    public static EventManager Instance
    {
      get
      {
        if (_instance != null) return _instance;

        var fp = new GameObject();
        fp.transform.name = "EventManager";
        _instance = fp.AddComponent<EventManager>();
        _instance.DefaultEventStream = new EventStream();
        return _instance;
      }
    }

    /// The 'default' event stream for event handlers to use.
    public IEventBus DefaultEventStream { get; private set; }

    /// Clear the singleton instance.
    public static void Clear(bool inEditor = false)
    {
      if (_instance == null) return;
      if (inEditor)
      {
        DestroyImmediate(_instance);
      }
      else
      {
        Destroy(_instance);
      }
      _instance = null;
    }

    /// Register an event bus
    public void Register(IEventBus bus)
    {
      _eventStream.Add(bus);
    }

    /// Deregister an event bus
    public void Deregister(IEventBus bus)
    {
      _eventStream.Remove(bus);
    }

    /// Forbid multiple instances
    public void Start()
    {
      if (_instance != null)
        Destroy(this);
    }

    /// Flush all events on all event bus instances every frame.
    public void Update()
    {
      foreach (var bus in _eventStream)
      {
        bus.Flush(MaxEventsPerFrame);
      }
    }
  }
}