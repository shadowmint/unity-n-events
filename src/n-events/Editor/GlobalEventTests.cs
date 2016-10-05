#if N_EVENTS_TESTS
using System;
using N.Package.Core.Tests;
using N.Package.Events;
using NUnit.Framework;
using N.Package.Events.Internal;
using N.Package.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using EventHandler = N.Package.Events.EventHandler;
using Object = UnityEngine.Object;

namespace N.Tests.Events
{
  public class GlobalEvent
  {
  }

  public class ListensToEvents : IDisposable
  {
    public int EventsHandled { get; set; }

    // Manually track event subscription and release when the object is destroyed.
    private EventBinding<GlobalEvent> _binding;

    public ListensToEvents()
    {
      _binding = GlobalEventStream.Events.AddEventHandler<GlobalEvent>(HandleEvent);
    }

    public void HandleEvent(GlobalEvent e)
    {
      EventsHandled += 1;
      GlobalEventStream.EventsDispatched += 1;
    }

    public void Dispose()
    {
      if (_binding != null) _binding.Dispose();
    }
  }

  public class ListensToEventsWithContext : IDisposable
  {
    public int EventsHandled { get; set; }

    // Automatically track event subscription and release when the object is destroyed.
    private readonly EventContext _eventContext = new EventContext();

    public ListensToEventsWithContext()
    {
      GlobalEventStream.Events.AddEventHandler<GlobalEvent>(HandleEvent, _eventContext);
    }

    public void HandleEvent(GlobalEvent e)
    {
      EventsHandled += 1;
      GlobalEventStream.EventsDispatched += 1;
    }

    public void Dispose()
    {
      _eventContext.Dispose();
    }
  }

  [ExecuteInEditMode]
  public class ListensToEventsBehaviour : MonoBehaviour
  {
    public int EventsHandled { get; set; }

    // Automatically track event subscription and release when the object is destroyed.
    private readonly EventContext _eventContext = new EventContext();

    public void Start()
    {
      GlobalEventStream.Events.AddEventHandler<GlobalEvent>(HandleEvent, _eventContext);
    }

    public void HandleEvent(GlobalEvent e)
    {
      EventsHandled += 1;
      GlobalEventStream.EventsDispatched += 1;
    }

    public void OnDestroy()
    {
      _eventContext.Dispose();
    }
  }

  public class GlobalEventStream
  {
    public static int EventsDispatched { get; set; }

    public static EventHandler Events = new EventHandler();

    public static void Trigger(GlobalEvent e)
    {
      Events.Trigger(e);
    }

    public static void Reset()
    {
      Events = new EventHandler();
      EventsDispatched = 0;
    }
  }

  public class GlobalEventTests : TestCase
  {
    [Test]
    public void test_global_events_fire()
    {
      GlobalEventStream.Reset();
      var listener = new ListensToEvents();
      var listener2 = new ListensToEventsWithContext();

      GlobalEventStream.Trigger(new GlobalEvent());
      GlobalEventStream.Trigger(new GlobalEvent());

      Assert(listener.EventsHandled == 2);
      Assert(listener2.EventsHandled == 2);
      Assert(GlobalEventStream.EventsDispatched == 4);
    }

    [Test]
    public void test_when_object_expires_it_no_longer_receives_events()
    {
      GlobalEventStream.Reset();
      using (var listener = new ListensToEvents())
      {
        using(var listener2 = new ListensToEventsWithContext())
        {
          GlobalEventStream.Trigger(new GlobalEvent());

          Assert(listener2.EventsHandled == 1);
          Assert(GlobalEventStream.EventsDispatched == 2);
        }

        GlobalEventStream.Trigger(new GlobalEvent());
        Assert(listener.EventsHandled == 2);
        Assert(GlobalEventStream.EventsDispatched == 3);
      }

      GlobalEventStream.Trigger(new GlobalEvent());
      Assert(GlobalEventStream.EventsDispatched == 3);
    }

    [Test]
    public void test_when_gameobject_expires_it_no_longer_receives_events()
    {
      GlobalEventStream.Reset();

      var instance = this.SpawnComponent<ListensToEventsBehaviour>();
      instance.Start();

      GlobalEventStream.Trigger(new GlobalEvent());
      Assert(instance.EventsHandled == 1);
      Assert(GlobalEventStream.EventsDispatched == 1);

      Object.DestroyImmediate(instance.gameObject);

      GlobalEventStream.Trigger(new GlobalEvent());
      Assert(GlobalEventStream.EventsDispatched == 1);
    }
  }
}
#endif
