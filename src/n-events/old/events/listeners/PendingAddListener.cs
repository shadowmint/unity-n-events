using System.Collections.Generic;
using System;

namespace N.Package.Events.Legacy
{
    /// Keep track of requests to add an object when they're made, and flush the requests later.
    public class PendingAddListener<T> where T : class
    {
        /// Pending items
        private Queue<T> pendingObjects = new Queue<T>();

        /// Count of pending items
        protected int deferred = 0;

        /// Add a pending item
        protected void DeferAdd(T target)
        {
            pendingObjects.Enqueue(target);
            deferred += 1;
        }

        /// Yeild and remove pending objects
        protected IEnumerable<T> DeferredAdds
        {
            get
            {
                deferred = 0;
                int count = pendingObjects.Count;
                for (var i = 0; i < count; ++i)
                {
                    yield return pendingObjects.Dequeue() as T;
                }
            }
        }
    }
}
