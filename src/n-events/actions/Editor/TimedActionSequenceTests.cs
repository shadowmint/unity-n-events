#if N_EVENTS_TESTS
using NUnit.Framework;
using System.Linq;
using N.Package.Events;

public class TimedActionSequenceTests : N.Tests.Test
{
    public class SimpleAction : IAction
    {
        public static int completed;
        public Actions Actions { get; set; }
        public Timer Timer { get; set; }

        public void Execute()
        {
            completed += 1;
            Actions.Complete(this);
        }
    }

    [Test]
    public void test_timed_action_sequence()
    {
        var timer = new Timer();
        var actions = new Actions(timer);
        var children = new SimpleAction[] { new SimpleAction(), new SimpleAction(), new SimpleAction() };
        var instance = new TimedActionSequence().Add(children[0], 0f).Add(children[1], 2f).Add(children[2], 1f);

        bool done = false;
        SimpleAction.completed = 0;

        // Start~
        actions.Execute(instance, (ep) =>
        {
            if (ep.Is(instance))
            { done = true; }
        });

        // 0-delay actions are immediately executed
        Assert(SimpleAction.completed == 1);
        Assert(done == false);

        // Step to start first action
        timer.Force(1f);
        timer.Step();
        Assert(SimpleAction.completed == 2);
        Assert(done == false);

        // Step to start first action
        timer.Force(1f);
        timer.Step();
        Assert(SimpleAction.completed == 3);
        Assert(done == true);
    }
}
#endif
