#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;
using N;

public class DeferredCollectionTests : N.Tests.Test
{
    private class TestEvent : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_new_instance()
    {
        new DeferredCollection();
    }

    [Test]
    public void test_keep_event()
    {
        var count = 0;
        var done = false;
        var instance = new DeferredCollection();
        instance.AddEventHandler<TestEvent>((ep) =>
        {
            count += 1;
            if (!done)
            {
                ep.Api.Keep<TestEvent>();
            }
            else
            {
                count += 100;
            }
        }, 1f);

        instance.Step(1f);
        instance.Trigger(new TestEvent());
        Assert(count == 1);

        instance.Trigger(new TestEvent());
        Assert(count == 2);

        done = true;
        instance.Trigger(new TestEvent());
        Assert(count == 103);

        instance.Trigger(new TestEvent());
        Assert(count == 103);
    }
}
#endif
