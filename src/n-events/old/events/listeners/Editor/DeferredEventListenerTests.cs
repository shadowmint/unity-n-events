#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events.Legacy;
using N;

public class DeferredEventListenerTests : N.Tests.Test
{
    private class TestEvent : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_new_instance()
    {
        new DeferredEventListener();
    }

    [Test]
    public void test_reject_invalid_type()
    {
        var instance = new DeferredEventListener();
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
        var instance = new DeferredEventListener();
        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler<TestEvent>((ep) => { });
        instance.AddEventHandler<TestEvent>((ep) => { });
        Assert(instance.Count == 2);
    }

    [Test]
    public void test_manually_invoke_targets()
    {
        var instance = new DeferredEventListener();
        var count = 0;
        var target = new TestEvent();
        instance.AddSupportedType<TestEvent>();

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } });

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } });

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(count == 2);
        Assert(instance.Count == 0);
    }

    [Test]
    public void test_deferred_task()
    {
        var instance = new DeferredEventListener();
        var count = 0;
        var target = new TestEvent();
        instance.AddSupportedType<TestEvent>();

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } }, 2f);

        instance.AddEventHandler<TestEvent>((ep) =>
        { if (ep == target) { count += 1; } }, 4f);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }
        Assert(count == 0);
        Assert(instance.Count == 2);

        instance.Step(2f);
        foreach (var handler in instance.Handlers) { handler.localHandler(target); }
        Assert(count == 1);
        Assert(instance.Count == 1);

        instance.Step(2f);
        foreach (var handler in instance.Handlers) { handler.localHandler(target); }
        Assert(count == 2);
        Assert(instance.Count == 0);
    }

    [Test]
    public void test_add_handler_during_event()
    {
        var instance = new DeferredEventListener();
        var count = 0;
        var target = new TestEvent();

        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler<TestEvent>((ep) =>
        {
            count = 10;
            instance.AddEventHandler<TestEvent>((ep2) => { count = 100; }, 1f);
        }, 1f);
        Assert(instance.Count == 1);
        instance.Step(1f);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 1);
        instance.Step(1f);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 0);
        Assert(count == 100);
    }
}
#endif
