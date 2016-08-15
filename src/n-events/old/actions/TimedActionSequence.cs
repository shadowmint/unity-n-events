using System.Collections.Generic;
using System.Linq;

namespace N.Package.Events.Legacy
{
    public class TimedActionSequence : IAction
    {
        /// The requested action
        private struct ActionRequest
        {
            public float delay;
            public IAction action;
        }

        /// Events api
        private Timer timer;
        public Timer Timer { set { timer = value; } }

        /// Actions api
        private Actions actions;
        public Actions Actions { set { actions = value; } }

        /// Set of child actions
        private Queue<ActionRequest> children = new Queue<ActionRequest>();

        /// The count of completed actions and all actions
        private int total = 0;
        private int resolved = 0;

        /// Execute the next action
        public void CompletedAction(ActionCompleteEvent item)
        {
            if (children.Any(x => x.action == item.action))
            { resolved += 1; }

            if (resolved == total)
            { actions.Complete(this); }
        }

        /// Add another action to this set
        /// @param action The action to run
        /// @param delay Run the action after this delay
        public TimedActionSequence Add(IAction action, float delay)
        {
            children.Enqueue(new ActionRequest()
            {
                action = action,
                delay = delay
            });
            return this;
        }

        /// Run this action
        public void Execute()
        {
            total = children.Count;
            foreach (var item in children)
            {
                if (item.delay > 0f)
                {
                    timer.Events.AddEventHandler<TimerEvent>((ep) =>
                    {
                        actions.Execute(item.action, CompletedAction);
                    }, item.delay);
                }
                else
                {
                    actions.Execute(item.action, CompletedAction);
                }
            }
        }
    }
}
