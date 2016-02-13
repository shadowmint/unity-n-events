using System.Collections.Generic;

namespace N.Package.Events
{
    /// Run actions one after each other
    public class ActionSequence : IAction
    {
        /// Events api
        private Timer timer;
        public Timer Timer { set { timer = value; } }

        /// Actions api
        private Actions actions;
        public Actions Actions { set { actions = value; } }

        /// Set of child actions
        private Queue<IAction> children = new Queue<IAction>();
        private IAction currentAction = null;

        /// Execute the next action
        public void NextAction(ActionCompleteEvent item)
        {
            if (children.Count > 0)
            {
                if ((item == null) || (currentAction == item.action))
                {
                    currentAction = children.Dequeue();
                    actions.Execute(currentAction, NextAction);
                }
            }
            else
            {
                actions.Complete(this);
            }
        }

        /// Add another action to this set
        public ActionSequence Add(IAction action)
        {
            children.Enqueue(action);
            return this;
        }

        /// Run this action
        public void Execute()
        {
            timer.NoOp();
            NextAction(null);
        }
    }
}
