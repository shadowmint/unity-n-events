namespace N.Package.Events.Legacy
{
    /// A delegate to run event handlers
    public delegate void UntypedEventHandler(IEvent item);

    /// Keep track of typed and untyped handlers
    public class UntypedSimpleEventBinding
    {
        /// The original handler
        public object handler;

        /// The stream handler
        public UntypedEventHandler localHandler;
    }

    /// Keep track of typed and untyped handlers
    public class UntypedEventBinding
    {
        /// The original handler
        public object handler;

        /// The stream handler
        public UntypedEventHandler localHandler;

        /// Is this object scheduled for removal?
        public bool expired;
    }

    /// Keep track of a deferred untyped handler
    public class UntypedDeferredEventBinding
    {
        /// The amount of time spent waiting so far
        public float elapsed;

        /// The total expected wait time
        public float duration;

        /// The original handler
        public object handler;

        /// The stream handler
        public UntypedEventHandler localHandler;
    }
}
