using N.Package.Events;
using N.Package.Events.Internal;
using N.Package.Test;
using NUnit.Framework;
using EventHandler = N.Package.Events.EventHandler;

namespace Tests.Editor
{

  public class DeferredEventTests : TestCase
  {
    [Test]
    public void test_async_event()
    {
      EventManager.Instance.Clear(true);

      var handler = new EventHandler();
      var gotEvent = false;
      handler.AddEventHandler<DeferredEventTests>((fp) =>
      {
        gotEvent = true;
      });

      handler.TriggerAsync(this);

      Assert(gotEvent == false);
      EventManager.Instance.Update();

      Assert(gotEvent == true);

      EventManager.Instance.Clear(true);
    }

    [Test]
    public void test_deferred_event()
    {
      EventManager.Instance.Clear(true);

      var handler = new EventHandler();
      var gotEvent = false;
      handler.AddEventHandler<DeferredEventTests>((fp) =>
      {
        gotEvent = true;
      });

      handler.TriggerDeferred(this, 2.0f);

      Assert(gotEvent == false);
      EventManager.Instance.Update(1.0f);

      Assert(gotEvent == false);
      EventManager.Instance.Update(1.0f);

      Assert(gotEvent == true);

      EventManager.Instance.Clear(true);
    }
  }
}