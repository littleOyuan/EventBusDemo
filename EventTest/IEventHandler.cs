namespace EventTest
{
    public interface IEventHandler<in TEventData> : Base.IEventHandler where TEventData : IEventData
    {
        void HandleEvent(TEventData eventData);
    }
}