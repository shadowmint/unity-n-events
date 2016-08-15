#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events.Legacy;

public class ListenerCollectionTests : N.Tests.Test
{
    private class TestEventA : IEvent
    { public IEventApi Api { get; set; } }

    private class TestEventB : IEvent
    { public IEventApi Api { get; set; } }

    private class TestCollection : AbstractListenerCollection
    {
        /// Factory to use to create groups
        protected override IEventListener EventListenerFor(System.Type T)
        {
            var rtn = new SimpleEventListener();
            rtn.AddSupportedType(T);
            return rtn;
        }

        /// Pass an event to all event listener objects
        protected override void Trigger(IEventListener listener, IEvent data)
        {
            foreach (var handler in (listener as SimpleEventListener).Handlers)
            {
                handler.localHandler(data);
            }
        }

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        public void AddEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            var listener = ListenerFor(typeof(T));
            listener.AddEventHandler(callback);
        }
    }

    private class TestDeferredCollection : AbstractListenerCollection
    {
        /// Factory to use to create groups
        protected override IEventListener EventListenerFor(System.Type T)
        {
            var rtn = new DeferredEventListener();
            rtn.AddSupportedType(T);
            return rtn;
        }

        /// Pass an event to all event listener objects
        protected override void Trigger(IEventListener listener, IEvent data)
        {
            foreach (var handler in (listener as DeferredEventListener).Handlers)
            {
                handler.localHandler(data);
            }
        }

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        public void AddEventHandler<T>(EventHandler<T> callback, float deferInterval) where T : class, IEvent
        {
            var listener = ListenerFor(typeof(T));
            (listener as DeferredEventListener).AddEventHandler(callback, deferInterval);
        }

        /// Step
        public void Step(float dt)
        {
            foreach (var handler in listeners.Values)
            {
                (handler as DeferredEventListener).Step(dt);
            }
        }
    }

    [Test]
    public void test_trigger()
    {
        var instance = new TestCollection();
        var countA = 0;
        var countB = 0;

        instance.AddEventHandler<TestEventA>((gp) => { countA += 1; });
        instance.AddEventHandler<TestEventB>((gp) => { countB += 1; });

        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventB());
        instance.Trigger(new TestEventB());
        instance.Trigger(new TestEventA());

        Assert(countA == 3);
        Assert(countB == 2);
    }

    [Test]
    public void test_alternative_trigger()
    {
        var instance = new TestDeferredCollection();
        var countA = 0;
        var countB = 0;

        instance.AddEventHandler<TestEventA>((gp) => { countA += 1; }, 1f);
        instance.AddEventHandler<TestEventB>((gp) => { countB += 1; }, 2f);

        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventB());

        Assert(countA == 0);
        Assert(countB == 0);

        instance.Step(1f);
        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventB());

        Assert(countA == 1);
        Assert(countB == 0);

        instance.Step(1f);
        instance.Trigger(new TestEventA());
        instance.Trigger(new TestEventB());

        Assert(countA == 1);
        Assert(countB == 1);
    }
}
#endif
