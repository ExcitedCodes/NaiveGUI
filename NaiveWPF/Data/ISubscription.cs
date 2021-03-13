using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public interface ISubscription
    {
        bool IsReal { get; }
        SubscriptionModel Real { get; }
    }
}
