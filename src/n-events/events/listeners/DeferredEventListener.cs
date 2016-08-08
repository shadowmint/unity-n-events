using System.Collections.Generic;
using System;

namespace N.Package.Events
{
    /// A one-shot stream of deferred event handlers
    public class DeferredEventListener : PendingAddListener<UntypedDeferredEventBinding>, IEventListener
    {
        /// The set of types supported in this stream
        private List<Type> types = new List<Type>();

        /// The set of event listeners
        private Queue<UntypedDeferredEventBinding> handlers = new Queue<UntypedDeferredEventBinding>();

        /// The type that this event stream is listening for
        public IEnumerable<Type> Types { get { return types; } }

        /// Return the count of held event listeners
        public int Count { get { return handlers.Count; } }

        /// Currently busy?
        private bool busy = false;

        /// Yield the set of handlers
        /// This is a consumptive iterator; it removes items.
        public IEnumerable<UntypedDeferredEventBinding> Handlers
        {
            get
            {
                if (!busy)
                {
                    busy = true;
                    var total = handlers.Count;
                    for (var i = 0; i < total; ++i)
                    {
                        var handler = handlers.Dequeue();
                        if (handler.elapsed >= handler.duration)
                        {
                            yield return handler;
                        }
                        else
                        {
                            handlers.Enqueue(handler);
                        }
                    }
                    busy = false;
                    AddDeferredEventHandlers();
                }

            }
        }

        /// Update the time increment on all handlers
        public void Step(float dt)
        {
            foreach (var handler in handlers)
            {
                handler.elapsed += dt;
            }
        }

        /// Add a type for this event stream
        public void AddSupportedType<T>() where T : class
        { AddSupportedType(typeof(T)); }

        /// Add a type for this event stream
        public void AddSupportedType(Type T)
        {
            if (!types.Contains(T))
            {
                types.Add(T);
            }
        }

        /// Check if we support a given type
        /// @param T The type to check
        public bool Supports<T>() where T : class
        { return Supports(typeof(T)); }

        /// Check if we support a given type
        /// @param T The type to check
        public bool Supports(Type T)
        { return types.Contains(T); }

        /// Add an event handler
        public void AddEventHandler<T>(EventHandler<T> handler) where T : class, IEvent
        { AddEventHandler(handler, 0f); }

        /// Add an event handler
        public void AddEventHandler<T>(EventHandler<T> handler, float deferInterval) where T : class, IEvent
        {
            if (Supports<T>())
            {
                var target = new UntypedDeferredEventBinding()
                {
                    handler = handler,
                    localHandler = (ep) => { handler(ep as T); },
                    elapsed = 0f,
                    duration = deferInterval
                };
                if (!busy)
                {
                    handlers.Enqueue(target);
                }
                else
                {
                    DeferAdd(target);
                }
                return;
            }
            throw new EventException("Invalid T for StreamedEventListener")
            { errorCode = EventErrors.INVALID_HANDLER_TYPE };
        }

        /// Add deferred handlers
        private void AddDeferredEventHandlers()
        {
            if (deferred > 0)
            {
                foreach (var item in DeferredAdds)
                {
                    handlers.Enqueue(item);
                }
            }
        }

        /// Check if a specific handlers is currently held
        /// @param callback A callback to check for.
        public bool ContainsEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            foreach (var item in this.handlers)
            {
                if (item.handler.Equals(callback))
                {
                    return true;
                }
            }
            return false;
        }

        /// For API compliance, this does nothing.
        public void RemoveEventHandler<T>(EventHandler<T> handler) where T : class, IEvent { }

        /// Clear all handlers
        public void Clear()
        { handlers.Clear(); }
    }
}
