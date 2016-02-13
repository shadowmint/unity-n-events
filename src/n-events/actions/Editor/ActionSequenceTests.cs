#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;

public class ActionSequenceTests : N.Tests.Test
{
    public class SimpleAction : IAction
    {
        public static int started;
        public static int completed;
        public Actions Actions { get; set; }
        public Timer Timer { get; set; }

        public void Execute()
        {
            started += 1;
        }

        public void Complete()
        {
            completed += 1;
            Actions.Complete(this);
        }
    }

    [Test]
    public void test_action_sequence()
    {
        var timer = new Timer();
        var actions = new Actions(timer);
        var children = new SimpleAction[] { new SimpleAction(), new SimpleAction(), new SimpleAction() };
        var instance = new ActionSequence().Add(children[0]).Add(children[1]).Add(children[2]);

        bool done = false;
        SimpleAction.completed = 0;
        SimpleAction.started = 0;

        actions.Execute(instance, (ep) =>
        {
            if (ep.Is(instance))
            { done = true; }
        });

        Assert(SimpleAction.started == 1);
        Assert(SimpleAction.completed == 0);
        Assert(done == false);

        children[0].Complete();
        Assert(SimpleAction.started == 2);
        Assert(SimpleAction.completed == 1);
        Assert(done == false);

        children[1].Complete();
        Assert(SimpleAction.started == 3);
        Assert(SimpleAction.completed == 2);
        Assert(done == false);

        children[2].Complete();
        Assert(SimpleAction.started == 3);
        Assert(SimpleAction.completed == 3);
        Assert(done == true);
    }
}
#endif
