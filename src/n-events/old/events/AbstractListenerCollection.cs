using System;
using System.Linq;
using System.Collections.Generic;

namespace N.Package.Events.Legacy
{
    /// A collection of event listeners based on type
    public abstract class AbstractListenerCollection : ITrigger, IEventApi
    {
        /// The maxmium number of events that can be processed in a single round
        private const int MAX_EVENTS_PER_TRIGGER = 10;

        /// The set of typed listeners instances
        protected IDictionary<Type, IEventListener> listeners = new Dictionary<Type, IEventListener>();

        /// Return the event listener which last had a Trigger
        private IEventListener currentEventListener;
        public IEventListener EventListener { get { return currentEventListener; } }

        /// Currently executing handler
        private object currentEventHandler;

        /// Deferred triggers
        private Queue<IEvent> deferredTriggers = new Queue<IEvent>();

        /// Factory to use to create groups
        /// @param T The type to create an event listener for
        protected abstract IEventListener EventListenerFor(System.Type T);

        /// Pass an event to all event listener objects
        /// @param listener The abstract listener instance
        /// @param data The abstract event instance
        protected abstract void Trigger(IEventListener listener, IEvent data);

        /// Return the event listener for type T
        /// @param T The type to find an event listener for
        /// @param fabricate True if a new one should be created if missing
        protected IEventListener ListenerFor(Type T, bool fabricate = true)
        {
            if (!listeners.ContainsKey(T))
            {
                if (fabricate)
                {
                    listeners.Add(T, EventListenerFor(T));
                }
                else
                {
                    return null;
                }
            }
            return listeners[T];
        }

        /// Prune inactive listeners
        protected void Prune()
        {
            var pruneList = listeners.Where(pair => pair.Value.Count == 0)
                                     .Select(pair => pair.Key)
                                     .ToList();

            foreach (var key in pruneList)
            { listeners.Remove(key); }
        }

        /// Schedule an event handler to be removed at the next conveient opportunity.
        /// @param callback A callback to remove.
        public void RemoveEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            var listener = ListenerFor(typeof(T));
            listener.RemoveEventHandler(callback);
        }

        /// Check if a specific handlers is currently held
        /// @param callback A callback to check for.
        public bool ContainsEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            var listener = ListenerFor(typeof(T));
            return listener.ContainsEventHandler(callback);
        }

        /// Clear all handlers
        public void Clear()
        {
            listeners.Clear();
        }

        /// Trigger an event
        /// @param ep An event pointer
        public void Trigger(IEvent ep)
        {
            deferredTriggers.Enqueue(ep);
            Trigger();
        }

        /// Trigger pending events
        public void Trigger()
        {
            if (currentEventListener == null)
            {
                var count = 0;
                while (true)
                {
                    if (deferredTriggers.Count == 0)
                    {
                        break;
                    }
                    else if (count > MAX_EVENTS_PER_TRIGGER)
                    {
                        break;
                    }
                    else
                    {
                        count += 1;
                        var ep = deferredTriggers.Dequeue();
                        var type = ep.GetType();
                        var listener = ListenerFor(type, false);
                        if (listener != null)
                        {
                            currentEventListener = listener;
                            Trigger(listener, ep);
                            currentEventListener = null;
                        }
                    }
                }
                Prune();
            }
        }

        /// Return count of active items
        public int Count
        {
            get
            {
                var count = 0;
                foreach (var item in listeners.Values)
                { count += item.Count; }
                return count;
            }
        }

        /// Run this before invoking a callback
        protected void PreInvokeHandler(IEventListener listener, object eventHandler, IEvent eventPointer)
        {
            currentEventListener = listener;
            currentEventHandler = eventHandler;
            eventPointer.Api = this;
        }

        /// Return the handler being invoked
        public EventHandler<T> EventHandler<T>() where T : class, IEvent
        { return currentEventHandler as EventHandler<T>; }

        /// Trigger api
        public ITrigger EventTrigger { get { return this; } }

        /// Return count of pending events
        public int PendingEvents { get { return deferredTriggers.Count; } }
    }
}
