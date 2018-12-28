using System;
using System.Collections.Generic;
using N.Package.Events;
using N.Package.Test;
using NUnit.Framework;
using EventHandler = N.Package.Events.EventHandler;

namespace Tests.Editor.Regressions
{
  /// <summary>
  /// See https://github.com/shadowmint/unity-n-events/issues/2
  /// </summary>
  public class Regression02 : TestCase
  {
    [Test]
    public void test_clearing_event_handler_completes_pending_events()
    {
      var eventHandler = new EventHandler();
      var hits = new Dictionary<Guid, int>();
      var keys = new List<Guid> {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
      keys.ForEach(i => hits[i] = 0);

      Action<EventTypeA> typeAHandler = ep => { hits[ep.Value] += 1; };
      Action<EventTypeB> typeBHandler = ep => { hits[ep.Value] += 1; };

      EventBinding<EventTypeA> binding = null;
      Action<EventTypeA> removeBindingHandler = ep =>
      {
        if (binding != null) binding.Dispose();
      };

      eventHandler.AddEventHandler(typeAHandler);
      eventHandler.AddEventHandler(typeAHandler);

      binding = eventHandler.AddEventHandler(removeBindingHandler);

      eventHandler.AddEventHandler(typeAHandler);
      eventHandler.AddEventHandler(typeAHandler);

      eventHandler.AddEventHandler(typeBHandler);

      // Trigger normal events
      eventHandler.Trigger(new EventTypeB() {Value = keys[0]});
      Assert(hits[keys[0]] == 1);

      // Trigger handler that removes itself
      eventHandler.Trigger(new EventTypeA() {Value = keys[1]});
      Assert(hits[keys[1]] == 4);

      // Remaining event handlers still work
      eventHandler.Trigger(new EventTypeA() {Value = keys[2]});
      Assert(hits[keys[2]] == 4);
    }

    internal class EventTypeA
    {
      public Guid Value { get; set; }
    }

    internal class EventTypeB
    {
      public Guid Value { get; set; }
    }
  }
}