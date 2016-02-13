namespace N.Package.Events
{
    /// An api for various event related tasks
    public interface IEventApi
    {
        /// Return the handler being invoked
        EventHandler<T> EventHandler<T>() where T : class, IEvent;

        /// Return the event listener for this event
        IEventListener EventListener { get; }

        /// Trigger api
        ITrigger EventTrigger { get; }
    }

    /// Helpers
    public static class IEventApiHelpers
    {
        /// Remove the current event from the event handler
        public static void Remove<T>(this IEventApi self) where T : class, IEvent
        {
            var handler = self.EventHandler<T>();
            if (handler != null)
            {
                self.EventListener.RemoveEventHandler(self.EventHandler<T>());
            }
        }

        /// Push the current event back into the event handler.
        /// Useful for one-shot event handlers that don't want to expired.
        public static void Keep<T>(this IEventApi self) where T : class, IEvent
        {
            var handler = self.EventHandler<T>();
            if (handler != null)
            {
                if (!self.EventListener.ContainsEventHandler(handler))
                {
                    self.EventListener.AddEventHandler(handler);
                }
            }
        }

        /// Defer a trigger action to run at the end of an event
        public static void Trigger(this IEventApi self, IEvent data)
        { self.EventTrigger.Trigger(data); }
    }
}
