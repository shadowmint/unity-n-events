using System.Collections.Generic;

namespace N.Package.Events.Legacy
{
    /// Common helpers for event types
    public static class EventHelpers
    {
        /// Return true if this is the given action, Otherwise defer
        /// @param action The action to check for
        /// @param query The query value
        /// @param value The value to check with
        /// @param removeOnMatch default action, if its match, remove the action
        public static bool Is<T, TEvent>(this IEvent action, T query, T value, bool removeOnMatch = true) where T: class where TEvent: class, IEvent
        {
            if (query == value)
            {
                if (removeOnMatch)
                {
                    action.Api.Remove<TEvent>();
                }
                return true;
            }
            else
            {
                action.Api.Keep<TEvent>();
            }
            return false;
        }
    }
}
