using System.Collections.Generic;
using System.Linq;
using System;

namespace N.Package.Events
{
    /// A carefully typed collection of event handlers
    public class SimpleEventListener : PendingAddListener<UntypedEventBinding>, IEventListener
    {
        /// The set of types supported in this stream
        private List<Type> types = new List<Type>();

        /// The set of event listeners
        private List<UntypedEventBinding> handlers = new List<UntypedEventBinding>();

        /// The type that this event stream is listening for
        public IEnumerable<Type> Types { get { return types; } }

        /// Return the count of held event listeners
        public int Count { get { return handlers.Count; } }

        /// Busy right now?
        private bool busy = false;

        /// Count of expired items
        private int expired = 0;

        /// Yield the set of handlers
        public IEnumerable<UntypedEventBinding> Handlers
        {
            get
            {
                if (!busy)
                {
                    busy = true;
                    foreach (var handler in handlers)
                    {
                        if ((!handler.expired) && (handler.localHandler != null))
                        {
                            yield return handler;
                        }
                    }
                    busy = false;
                    PruneExpiredEventHandlers();
                    AddPendingEventHandlers();
                }
                else
                {
                    yield break;
                }
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
        {
            if (Supports<T>())
            {
                var item = new UntypedEventBinding()
                {
                    handler = handler,
                    localHandler = (ep) => { handler(ep as T); },
                };
                if (busy)
                {
                    DeferAdd(item);
                }
                else
                {
                    handlers.Add(item);
                }
                return;
            }
            throw new EventException("Invalid T for EventStream")
            { errorCode = EventErrors.INVALID_HANDLER_TYPE };
        }

        /// Remove an event handler
        public void RemoveEventHandler<T>(EventHandler<T> handler) where T : class, IEvent
        {
            if (!busy)
            {
                handlers.RemoveAll(x => x.handler.Equals(handler));
            }
            else
            {
                var target = handlers.First(x => x.handler.Equals(handler));
                if (target != null)
                {
                    target.expired = true;
                    expired += 1;
                }
            }
        }

        /// Check if a specific handlers is currently held
        /// @param callback A callback to check for.
        public bool ContainsEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            foreach (var item in this.handlers)
            {
                if (item.handler == callback)
                {
                    return true;
                }
            }
            return false;
        }

        /// Prune all expired
        private void PruneExpiredEventHandlers()
        {
            if (expired > 0)
            {
                handlers.RemoveAll(x => x.expired);
                expired = 0;
            }
        }

        /// Add pending items
        private void AddPendingEventHandlers()
        {
            if (deferred > 0)
            {
                foreach (var item in DeferredAdds)
                {
                    handlers.Add(item);
                }
            }
        }

        /// Clear all handlers
        public void Clear()
        { handlers.Clear(); }
    }
}
