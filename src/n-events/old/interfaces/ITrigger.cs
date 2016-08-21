using System;

namespace N.Package.Events.Legacy
{
    /// This interface should be implemented by types that can trigger events
    public interface ITrigger
    {
        /// Trigger an event
        /// @param ep An event pointer
        void Trigger(IEvent ep);
    }
}
