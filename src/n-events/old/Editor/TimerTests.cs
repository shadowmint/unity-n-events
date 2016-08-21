#if N_EVENTS_TESTS
using N.Package.Core.Tests;
using UnityEngine;
using NUnit.Framework;
using N.Package.Events.Legacy;

public class TimerTests : TestCase
{
    [Test]
    public void test_custom_timer()
    {
        var timer = new Timer();

        var counter = 0;
        timer.Events.AddEventHandler<TimerEvent>((dt) => { counter += 1; });

        timer.Force(1f);
        Assert(timer.Step() == 1f);
        Assert(counter == 1);

        timer.Force(2f);
        Assert(timer.Step() == 2f);
        Assert(counter == 2);
    }

    [Test]
    public void test_oneshot_deferred()
    {
        var timer = new Timer();

        var counter = 0;
        timer.Events.AddEventHandler<TimerEvent>((dt) => { counter += 1; }, 2f);

        timer.Force(1f);
        Assert(timer.Step() == 1f);
        Assert(counter == 0);

        timer.Force(2f);
        Assert(timer.Step() == 2f);
        Assert(counter == 1);

        // expired now, no change
        timer.Force(2f);
        Assert(timer.Step() == 2f);
        Assert(counter == 1);
    }
}
#endif
