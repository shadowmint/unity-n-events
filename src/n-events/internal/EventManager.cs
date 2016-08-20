using System;
using System.Collections.Generic;
using N.Package.Events.Internal;
using UnityEngine;

namespace N.Package.Events.Internal
{
  /// A component to process async events.
  public class EventManager : MonoBehaviour
  {
    private static EventManager _defaultInstance;
    private readonly List<StreamContainer> _streams = new List<StreamContainer>();

    public static EventManager Instance
    {
      get
      {
        if (_defaultInstance != null) return _defaultInstance;
        var rp = new GameObject();
        rp.hideFlags = HideFlags.HideInHierarchy;
        rp.transform.name = typeof(EventManager).AssemblyQualifiedName;
        _defaultInstance = rp.AddComponent<EventManager>();
        return _defaultInstance;
      }
    }

    // Clear the event manager
    public void Clear(bool inEditor = false)
    {
      _streams.ForEach(s => s.OnReleaseStream());
      _streams.Clear();

      _defaultInstance = null;

      if (inEditor)
      {
        DestroyImmediate(gameObject);
      }
      else
      {
        Destroy(gameObject);
      }
    }

    // Register an event stream
    public void Register(EventStream stream, Action onReleaseStream)
    {
      _streams.Add(new StreamContainer
      {
        Stream = stream,
        OnReleaseStream = onReleaseStream
      });
    }

    // Deregister an event stream
    public void Deregister(EventStream stream)
    {
      _streams.RemoveAll(s => s.Stream.Equals(stream));
    }

    // Process pending events
    public void Update()
    {
      Update(Time.deltaTime);
    }

    // Process pending events
    public void Update(float delta)
    {
      foreach (var stream in _streams)
      {
        stream.Stream.Stash.Update(delta);
        foreach (var item in stream.Stream.Stash.Ready)
        {
          stream.Stream.Trigger(item.Source, item.Event);
        }
      }
    }
  }

  class StreamContainer
  {
    public EventStream Stream { get; set; }
    public Action OnReleaseStream { get; set; }
  }
}