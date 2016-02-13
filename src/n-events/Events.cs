using System.Collections.Generic;

namespace N.Package.Events
{
    /// A high level event handler
    public class Events : IEventListener, ITrigger
    {
        /// Various event handler groups
        private SimpleCollection simple = new SimpleCollection();
        private DeferredCollection deferred = new DeferredCollection();
        private StreamedCollection stream = new StreamedCollection();

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        /// @param single Make this callback a single invoke type
        public void AddEventHandler<T>(EventHandler<T> callback, float deferInterval) where T : class, IEvent
        {
            deferred.AddEventHandler(callback, deferInterval);
        }

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        /// @param single Make this callback a single invoke type
        public void AddEventHandler<T>(EventHandler<T> callback, bool single) where T : class, IEvent
        {
            if (single)
            {
                stream.AddEventHandler(callback);
            }
            else
            {
                AddEventHandler(callback);
            }
        }

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        public void AddEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            simple.AddEventHandler(callback);
        }

        /// Schedule an event handler to be removed at the next conveient opportunity.
        /// @param callback A callback to remove.
        public void RemoveEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            simple.RemoveEventHandler(callback);
        }

        /// Check if a specific handlers is currently held
        /// @param callback A callback to check for.
        public bool ContainsEventHandler<T>(EventHandler<T> callback) where T : class, IEvent
        {
            return simple.ContainsEventHandler(callback) ||
                   deferred.ContainsEventHandler(callback) ||
                   stream.ContainsEventHandler(callback);
        }

        /// Clear all handlers
        public void Clear()
        {
            simple.Clear();
            deferred.Clear();
            stream.Clear();
        }

        /// Step for deferred handlers
        public void Step(float dt)
        {
            deferred.Step(dt);
        }

        /// Return active count
        public int Count
        {
            get
            {
                return simple.Count + stream.Count + deferred.Count;
            }
        }

        /// Trigger an event
        public void Trigger(IEvent ep)
        {
            simple.Trigger(ep);
            stream.Trigger(ep);
            deferred.Trigger(ep);
        }

        /// Return count of pending events
        public int PendingEvents
        {
            get
            {
                return simple.PendingEvents + stream.PendingEvents + deferred.PendingEvents;
            }
        }
    }
}
