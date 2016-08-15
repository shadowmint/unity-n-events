namespace N.Package.Events.Legacy
{
    /// The base event type interface
    public interface IEvent
    {
        /// Set and get access to the event helper api
        IEventApi Api { get; set; }
    }
}
