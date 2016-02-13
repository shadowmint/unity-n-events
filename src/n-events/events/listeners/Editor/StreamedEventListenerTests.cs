#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;
using N;

public class StreamedEventListenerTests : N.Tests.Test
{
    private class TestEvent : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_new_instance()
    {
        new StreamedEventListener();
    }

    [Test]
    public void test_reject_invalid_type()
    {
        var instance = new StreamedEventListener();
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
        var instance = new StreamedEventListener();
        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler<TestEvent>((ep) => { });
        instance.AddEventHandler<TestEvent>((ep) => { });
        Assert(instance.Count == 2);
    }

    [Test]
    public void test_manually_invoke_targets()
    {
        var instance = new StreamedEventListener();
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
    public void test_add_handler_during_event()
    {
        var instance = new StreamedEventListener();
        var count = 0;
        var target = new TestEvent();

        instance.AddSupportedType<TestEvent>();
        instance.AddEventHandler<TestEvent>((ep) =>
        {
            count = 10;
            instance.AddEventHandler<TestEvent>((ep2) => { count = 100; });
        });
        Assert(instance.Count == 1);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 1);

        foreach (var handler in instance.Handlers) { handler.localHandler(target); }

        Assert(instance.Count == 0);
        Assert(count == 100);
    }
}
#endif
