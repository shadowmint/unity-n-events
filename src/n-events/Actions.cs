using System.Collections.Generic;

namespace N.Package.Events
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
        {
            if (action == this.action)
            {
                if (removeOnMatch)
                {
                    Api.Remove<ActionCompleteEvent>();
                }
                return true;
            }
            else
            {
                Api.Keep<ActionCompleteEvent>();
            }
            return false;
        }
    }

    /// Actions is a high level api for running tasks
    public class Actions
    {
        /// The timer for this set of actions
        private Timer timer;

        /// The events queue to use; if omitted defaults to the timer one
        private Events events;

        /// Create an instance
        /// @param timer The Timer instance to use.
        public Actions(Timer timer)
        {
            this.timer = timer;
            this.events = timer.Events;
        }

        /// Create an instance
        /// @param timer The Timer instance to use.
        public Actions(Timer timer, Events events)
        {
            this.timer = timer;
            this.events = events;
        }

        /// Execute an action
        public void Execute<T>() where T : IAction, new()
        {
            Execute(new T());
        }

        /// Execute an action
        public void Execute(IAction action)
        {
            action.Actions = this;
            action.Timer = this.timer;
            action.Execute();
        }

        /// Execute an action
        public void Execute<T>(EventHandler<ActionCompleteEvent> then) where T : IAction, new()
        {
            Execute(new T(), then);
        }

        /// Execute an action
        public void Execute(IAction action, EventHandler<ActionCompleteEvent> then)
        {
            action.Actions = this;
            action.Timer = this.timer;
            events.AddEventHandler(then, true);
            action.Execute();
        }

        /// Run when an action is complete
        public void Complete(IAction action, bool success = true)
        {
            var evp = new ActionCompleteEvent() { action = action };
            events.Trigger(evp);
        }

        /// Trigger any pending events by calling this at most
        /// AbstractListenerCollection.MAX_EVENTS_PER_TRIGGER will run
        /// per AbstractListenerCollection attached to this object.
        /// @return true If There are pending events
        public bool Pending()
        { return events.PendingEvents > 0; }
    }
}
