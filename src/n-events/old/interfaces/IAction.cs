namespace N.Package.Events.Legacy
{
    /// The base interface for actions
    public interface IAction
    {
        /// Set the Actions api for this action
        Actions Actions { set; }

        /// Set the timer for this action
        Timer Timer { set; }

        /// Run the action
        void Execute();
    }
}
