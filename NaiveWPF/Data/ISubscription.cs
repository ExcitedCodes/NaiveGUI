namespace NaiveGUI.Data
{
    public interface ISubscription
    {
        bool IsReal { get; }
        Subscription Real { get; }
    }
}
