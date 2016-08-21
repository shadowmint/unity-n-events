#if N_EVENTS_TESTS
using System;
using N.Package.Core.Tests;
using NUnit.Framework;
using N.Package.Events.Internal;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Tests.Events
{
  class DummyCompletionEvent
  {
    public bool Complete;
  }

  class DummyAction
  {
    private EventHandler _events = new EventHandler();

    public void Execute()
    {
      _events.Trigger(new DummyCompletionEvent { Complete = true });
      _events.ClearEventHandlers<DummyCompletionEvent>();
    }

    public DummyAction OnComplete(Action<DummyCompletionEvent> handler)
    {
      _events.AddEventHandler(handler);
      return this;
    }
  }

  public class SingleEventTests : TestCase
  {
    [Test]
    public void test_trigger_event_singular()
    {
      var completed = false;
      new DummyAction().OnComplete((ep) =>
      {
        Assert(ep.Complete);
        completed = true;
      }).Execute();

      Assert(completed);
    }

    [Test]
    public void test_trigger_event_reuse_without_duplicate_events()
    {
      var completed1 = 0;
      var completed2 = 0;
      var action = new DummyAction();
        
      action.OnComplete((ep) =>
      {
        completed1 += 1;
        
      }).Execute();
      action.OnComplete((ep) =>
      {
        completed2 += 1;
        
      }).Execute();

      Assert(completed1 == 1);
      Assert(completed2 == 1);
    }
  }
}

#endif