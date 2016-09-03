#if N_EVENTS_TESTS
using System;
using N.Package.Core.Tests;
using N.Package.Events;
using NUnit.Framework;
using N.Package.Events.Internal;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Tests.Events
{
  public class EventHandlerTests : TestCase
  {
    [Test]
    public void test_trigger_event()
    {
      var handler = new EventHandler();
      EventHandlerTests eventResult = null;
      handler.AddEventHandler<EventHandlerTests>((fp) => { eventResult = fp; });

      handler.Trigger(this);
      Assert(eventResult == this);
    }

    [Test]
    public void test_recursive_event_dispatch()
    {
      var handler1 = new EventHandler();
      var handler2 = new EventHandler();
      var handler3 = new EventHandler();

      var count1 = 0;
      var count2 = 0;
      var count3 = 0;

      handler1.AddEventHandler<EventHandlerTests>((fp) =>
      {
        count1 += 1;

        var context = new EventContext();
        handler3.AddEventHandler<EventHandlerTests>((ep) =>
        {
          count3 += 1;
          context.Dispose();
        }, context);

        handler2.Trigger(this);
      });

      handler2.AddEventHandler<EventHandlerTests>((fp) =>
      {
        count2 += 1;
        handler3.Trigger(this);
      });

      handler1.Trigger(this);

      Assert(count1 == 1);
      Assert(count2 == 1);
      Assert(count3 == 1);
    }

    [Test]
    public void test_infinite_recursion_event_dispatch()
    {
      var handler1 = new EventHandler();
      var handler2 = new EventHandler();

      var count1 = 0;
      var count2 = 0;

      handler1.AddEventHandler<EventHandlerTests>((fp) =>
      {
        count1 += 1;
        handler2.Trigger(this);
      });

      handler2.AddEventHandler<EventHandlerTests>((fp) =>
      {
        count2 += 1;
        handler1.Trigger(this);
      });

      try
      {
        handler1.Trigger(this);
        Unreachable();
      }
      catch(Exception)
      {
      }

      Assert(count1 == 50);
      Assert(count2 == 50);

      count1 = 0;
      count2 = 0;

      try
      {
        handler1.Trigger(this, 10);
        Unreachable();
      }
      catch(Exception)
      {
      }

      Assert(count1 == 5);
      Assert(count2 == 5);
    }

    [Test]
    public void test_event_handlers_are_independent()
    {
      var handler1 = new EventHandler();
      var handler2 = new EventHandler();
      var handler3 = new EventHandler();

      var count1 = 0;
      var count2 = 0;
      var count3 = 0;

      handler1.AddEventHandler<EventHandlerTests>((fp) => { count1 += 1; });
      handler2.AddEventHandler<EventHandlerTests>((fp) => { count2 += 1; });
      handler2.AddEventHandler<EventHandlerTests>((fp) => { count2 += 1; });
      handler3.AddEventHandler<EventHandlerTests>((fp) => { count3 += 1; });
      handler3.AddEventHandler<EventHandlerTests>((fp) => { count3 += 1; });
      handler3.AddEventHandler<EventHandlerTests>((fp) => { count3 += 1; });

      handler1.Trigger(this);
      handler2.Trigger(this);
      handler3.Trigger(this);

      handler1.Trigger(this);
      handler2.Trigger(this);
      handler3.Trigger(this);

      Assert(count1 == 2);
      Assert(count2 == 4);
      Assert(count3 == 6);
    }

    [Test]
    public void test_disposing_of_an_event_handler_disposes_correctly()
    {
      EventStream.Default.Clear();
      using (var handler1 = new EventHandler())
      {
        using (var handler2 = new EventHandler())
        {
          using (var handler3 = new EventHandler())
          {
            handler1.AddEventHandler<EventHandlerTests>((fp) => { });
            handler2.AddEventHandler<EventHandlerTests>((fp) => { });
            handler2.AddEventHandler<EventHandlerTests>((fp) => { });
            handler3.AddEventHandler<EventHandlerTests>((fp) => { });
            handler3.AddEventHandler<EventHandlerTests>((fp) => { });
            handler3.AddEventHandler<EventHandlerTests>((fp) => { });
            Assert(EventStream.Default.Count == 6);
          }
          Assert(EventStream.Default.Count == 3);
        }
        Assert(EventStream.Default.Count == 1);
      }
      Assert(EventStream.Default.Count == 0);
    }
  }
}

#endif