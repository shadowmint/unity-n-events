namespace N.Package.Events.Legacy
{
    /// A callback for an event handler
    /// @param ep An event pointer
    public delegate void EventHandler<T>(T ep) where T : IEvent;

    /// This interface should be implemented by types that can listen for events
    /// Note that `RemoveEventHandler` may not be an immediate operation depending
    /// on the context (eg. as an iterator resolved).
    public interface IEventListener
    {
        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        void AddEventHandler<T>(EventHandler<T> callback) where T : class, IEvent;

        /// Schedule an event handler to be removed at the next conveient opportunity.
        /// @param callback A callback to remove.
        void RemoveEventHandler<T>(EventHandler<T> callback) where T : class, IEvent;

        /// Check if a specific handlers is currently held
        /// @param callback A callback to check for.
        bool ContainsEventHandler<T>(EventHandler<T> callback) where T : class, IEvent;

        /// Clear all handlers
        void Clear();

        /// Return a count of active handlers
        int Count { get; }
    }
}
