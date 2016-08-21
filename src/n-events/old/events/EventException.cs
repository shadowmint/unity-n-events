using System;

namespace N.Package.Events.Legacy
{
    /// Event error types
    public enum EventErrors
    {
        /// An attempt was made to add an event handler with the wrong type.
        INVALID_HANDLER_TYPE,
    }

    /// Basic event exception type
    public class EventException : Exception
    {
        /// The error code
        public EventErrors errorCode;

        public EventException() : base() {}
        public EventException(string msg) : base(msg) {}
    }
}
