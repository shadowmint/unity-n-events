#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;
using N.Package.Events.Internal;

namespace N.Tests.Events
{
  public class EventHandlerTests : N.Tests.Test
  {
    [Test]
    public void test_trigger_event()
    {
      EventManager.Clear(true);
      var handler = new EventHandler();
      EventHandlerTests eventResult = null;
      handler.AddEventHandler<EventHandlerTests>((fp) => { eventResult = fp; });

      handler.Trigger(this);
      Assert(eventResult == null);

      EventManager.Instance.Update();
      Assert(eventResult == this);

      eventResult = null;

      EventManager.Instance.Update();
      Assert(eventResult == null);
    }

    [Test]
    public void test_event_handlers_are_independent()
    {
      EventManager.Clear(true);
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
      EventManager.Instance.Update();

      handler1.Trigger(this);
      handler2.Trigger(this);
      handler3.Trigger(this);
      EventManager.Instance.Update();

      Assert(count1 == 2);
      Assert(count2 == 4);
      Assert(count3 == 6);
    }
  }
}
#endif