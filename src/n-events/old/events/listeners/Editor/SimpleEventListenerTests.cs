#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events.Legacy;
using N;
using N.Package.Core.Tests;

public class SimpleEventListenerTests : TestCase
{
    private class TestEvent : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_new_instance()
    {
        new SimpleEventListener();
    }

    [Test]
    public void test_reject_invalid_type()
    {
        var instance = new SimpleEventListener();
        try
        {
            instance.AddEventHandler<TestEvent>((ep) => { });
            Unreachable();
        }
        catch (EventException err)
        {
            Assert(err.errorCode == EventErrors.INVALID_HANDLER_TYPE);
        }
    }

    [Test]
    public void test_add_event_handler()
    {
        var instance = new SimpleEventListener();
        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler<TestEvent>((ep) => { });
        instance.AddEventHandler<TestEvent>((ep) => { });
        Assert(instance.Count == 2);
    }

    [Test]
    public void test_manually_invoke_targets()
    {
        var instance = new SimpleEventListener();
        var count = 0;
        var target = new TestEvent();
        instance.AddSupportedType<TestEvent>();

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } });

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } });

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(count == 2);
        Assert(instance.Count == 2);
    }

    [Test]
    public void test_remove_target()
    {
        var instance = new SimpleEventListener();
        instance.AddSupportedType<TestEvent>();
        EventHandler<TestEvent> ep = (ep2) => { };

        instance.AddEventHandler<TestEvent>(ep);
        Assert(instance.Count == 1);

        instance.RemoveEventHandler(ep);
        Assert(instance.Count == 0);
    }

    [Test]
    public void test_add_remove_handler_during_event()
    {
        var instance = new SimpleEventListener();
        var count = 0;
        var target = new TestEvent();

        EventHandler<TestEvent> handler1 = (ep) =>
        {
            count = 100;
        };

        EventHandler<TestEvent> handler2 = (ep) =>
        {
            count += 1;
        };

        EventHandler<TestEvent> handler3 = null;
        handler3 = (ep) =>
        {
            instance.RemoveEventHandler(handler3);
            instance.RemoveEventHandler(handler2);
            instance.AddEventHandler(handler1);
        };

        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler(handler2);
        Assert(instance.Count == 1);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        instance.AddEventHandler(handler3);
        Assert(instance.Count == 2);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 1);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 1);
        Assert(count == 100);
    }
}
#endif
