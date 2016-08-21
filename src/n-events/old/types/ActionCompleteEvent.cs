using System.Collections.Generic;

namespace N.Package.Events.Legacy
{
    /// An actions was completed
    public class ActionCompleteEvent : IEvent
    {
        /// The action completed
        public IAction action;

        /// Api
        public IEventApi Api { get; set; }

        /// Create a new instance with no requeue
        public ActionCompleteEvent()
        {
        }

        /// Return true if this is the given action, Otherwise defer
        /// @param action The action to check for
        /// @param removeOnMatch default action, if its match, remove the action
        public bool Is(IAction action, bool removeOnMatch = true)
        { return this.Is<IAction, ActionCompleteEvent>(action, this.action, removeOnMatch); }
    }
}
