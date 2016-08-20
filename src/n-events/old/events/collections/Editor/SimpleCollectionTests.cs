#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events.Legacy;
using N;

public class SimpleCollectionTests : N.Tests.Test
{
    private class TestEvent : IEvent
    { public IEventApi Api { get; set; } }

    [Test]
    public void test_new_instance()
    {
        new SimpleCollection();
    }

    [Test]
    public void test_remove_event()
    {
        var count = 0;
        var done = false;
        var instance = new SimpleCollection();
        instance.AddEventHandler<TestEvent>((ep) =>
        {
            count += 1;
            if (done)
            {
                count += 100;
                ep.Api.Remove<TestEvent>();
            }
        });

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

    [Test]
    public void test_remove_specific_event()
    {
        var count = 0;
        var scount = 0;
        var instance = new SimpleCollection();

        instance.AddEventHandler<TestEvent>((ep) => { count += 1; });

        EventHandler<TestEvent> target = null;
        target = (ep) =>
        {
            if (count > 2)
            {
                if (ep.Api.EventHandler<TestEvent>() == target)
                {
                    scount += 1;
                    ep.Api.Remove<TestEvent>();
                }
            }
        };
        instance.AddEventHandler(target);

        instance.Trigger(new TestEvent());
        instance.Trigger(new TestEvent());
        Assert(count == 2);
        Assert(scount == 0);

        instance.Trigger(new TestEvent());
        instance.Trigger(new TestEvent());
        Assert(count == 4);
        Assert(scount == 1);
    }
}
#endif
