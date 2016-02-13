#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;

public class EventsTest : N.Tests.Test
{
    private class TestEventA : IEvent
    { public IEventApi Api { get; set; } }

    private class TestEventB : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_repeating_event()
    {
        var instance = new Events();
        var count = 0;
        instance.AddEventHandler<TestEventA>((ep) => { count += 1; });
        instance.Trigger(new TestEventA());
        Assert(count == 1);
    }

    [Test]
    public void test_trigger_event_removal()
    {
        var instance = new Events();
        var count = 0;
        EventHandler<TestEventA> handler;
        handler = (ep) =>
        {
            instance.RemoveEventHandler(handler);
            count += 1;
        };
        instance.AddEventHandler(handler);

        instance.Trigger(new TestEventA());
        Assert(count == 1);

        instance.Trigger(new TestEventA());
        Assert(count == 1);
    }

    [Test]
    public void test_single_event()
    {
        var instance = new Events();
        var count = 0;
        instance.AddEventHandler<TestEventA>((ep) => { count += 1; }, true);
        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventA());
        Assert(count == 1);
    }

    [Test]
    public void test_deferred_event()
    {
        var instance = new Events();
        var count = 0;
        instance.AddEventHandler<TestEventA>((ep) => { count += 1; }, 1f);

        instance.Trigger(new TestEventA());
        Assert(count == 0);

        instance.Step(1f);
        instance.Trigger(new TestEventA());
        Assert(count == 1);
    }

    [Test]
    public void test_trigger_deferred_event()
    {
        var instance = new Events();
        var count = 0;
        instance.AddEventHandler<TestEventA>((ep) =>
        {
            instance.AddEventHandler<TestEventB>((ep2) => { count += 100; }, 1f);
            count += 1;
        });
        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventA());
        Assert(count == 2);

        instance.Trigger(new TestEventB());
        Assert(count == 2);

        instance.Step(1f);
        instance.Trigger(new TestEventB());
        Assert(count == 202);

        // All are expired now, doesn't Trigger again
        instance.Trigger(new TestEventB());
        Assert(count == 202);
    }
}
#endif
