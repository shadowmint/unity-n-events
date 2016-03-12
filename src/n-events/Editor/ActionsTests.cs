#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;

public class ActionsTests : N.Tests.Test
{
    private class SimpleAction : IAction
    {
        public Actions Actions { get; set; }
        public Timer Timer { get; set; }
        public void Execute() { Actions.Complete(this); }
    }

    private class DeferredAction : IAction
    {
        public Actions Actions { get; set; }
        public Timer Timer { get; set; }
        public void Complete() { Actions.Complete(this); }
        public void Execute() {}
    }

    [Test]
    public void test_simple_action()
    {
        var timer = new Timer();
        var instance = new Actions(timer);
        var complete = false;

        // Run an action with no callback
        instance.Execute<SimpleAction>();
        Assert(complete == false);

        // Run an action with a callback
        instance.Execute<SimpleAction>((ep) => { complete = true; });
        Assert(complete == true);
    }

    [Test]
    public void test_wait_for_specific_action()
    {
        var timer = new Timer();
        var instance = new Actions(timer);
        var complete = false;
        var count = 0;

        // Run an action which doesn't resolved immediately
        var task = new DeferredAction();
        instance.Execute(task, (ep) =>
        {
            count += 1;
            complete = true;
        });
        Assert(complete == false);

        // Run some other action
        // Notice how all ActionCompleteEvents are skipped until the matching action.
        instance.Execute<SimpleAction>();
        instance.Execute<SimpleAction>();
        instance.Execute<SimpleAction>();
        Assert(count == 0);
        Assert(complete == false);

        // Now we fake the deferred completion, and correctly catch it
        task.Complete();
        Assert(count == 1);
        Assert(complete == true);
    }
}
#endif
