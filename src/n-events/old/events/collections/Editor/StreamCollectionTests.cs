#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events.Legacy;
using N;
using N.Package.Core.Tests;

public class StreamedCollectionTests : TestCase
{
    private class TestEvent : IEvent
    {
        public IEventApi Api { get; set; }
        public int id;
    }

    [Test]
    public void test_new_instance()
    {
        new StreamedCollection();
    }

    [Test]
    public void test_recursive_trigger()
    {
        var count = 0;
        var doneHandlerRun = false;
        var instance = new StreamedCollection();

        instance.AddEventHandler<TestEvent>((ep) =>
        {
            if (ep.id == 2)
            {
                doneHandlerRun = true;
            }
            else
            {
                ep.Api.Keep<TestEvent>();
            }
        });

        instance.AddEventHandler<TestEvent>((ep) =>
        {
            if (ep.id == 1)
            {
                count += 1;
                if (count < 3)
                {
                    ep.Api.Keep<TestEvent>();
                    ep.Api.Trigger(new TestEvent() { id = 1 });
                }
                else
                {
                    ep.Api.Trigger(new TestEvent() { id = 2 });
                }
            }
        });

        instance.Trigger(new TestEvent() { id = 1 });
        Assert(doneHandlerRun);
    }
}
#endif
