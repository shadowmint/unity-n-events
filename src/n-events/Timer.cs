using UnityEngine;

namespace N.Package.Events
{
    /// Timer event type
    public class TimerEvent : IEvent
    {
        /// Delta step
        public float delta;

        /// Api
        public IEventApi Api { get; set; }
    }

    /// Custom suspendable timer
    public class Timer
    {
        /// Event handler for on-update events
        private EventHandler events = new EventHandler();
        public EventHandler Events { get { return events; } }

        /// The last frame we updated on
        private int frameIndex = 0;

        /// The timestamp from the previous frame
        private float lastTime = 0;

        /// The last timestamp we saw from this frame
        private float thisTime = 0;

        /// If the next step time is forced?
        private bool forceTime = false;

        /// What the next forced step time is
        private float nextTime = 0;

        /// Are we paused?
        private bool paused = false;

        /// Init the timer
        public Timer()
        {
            Resume();
        }

        /// Pause the timer
        public void Pause()
        {
            paused = true;
        }

        /// On resume, reset the timer
        public void Resume()
        {
            lastTime = Time.time;
            thisTime = Time.time;
            frameIndex = Time.frameCount;
            forceTime = false;
            paused = false;
        }

        /// Force the next interval to be step
        /// Notice this actually invokes step as well.
        /// There's no reason to use this outside of tests.
        public void Force(float step)
        {
            forceTime = true;
            nextTime = step;
            frameIndex = -1;
            Step();
        }

        /// The last time interval
        /// Notice that this call tracks the frame index and returns the last
        /// delta regardless of how many times it is called during a frame.
        public float Step()
        {
            var rtn = 0f;
            if (!paused)
            {
                // By default return the same timestamp we used last time.
                rtn = nextTime;

                // If this is a new frame...
                if (frameIndex != Time.frameCount)
                {
                    // In debug mode, take the fixed time signature given
                    if (forceTime)
                    {
                        forceTime = false;
                    }

                    // Otherwise, calculate the delta
                    else
                    {
                        lastTime = thisTime;
                        thisTime = Time.time;
                        nextTime = thisTime - lastTime;
                    }

                    // Trigger an event and update the frame counter
                    frameIndex = Time.frameCount;
                    events.Step(nextTime);
                    events.Trigger(new TimerEvent() { delta = nextTime });
                }
            }
            return rtn;
        }

        /// A no-op to suppress warnings
        public void NoOp() { }
    }
}
