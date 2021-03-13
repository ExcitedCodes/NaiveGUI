using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public class FakeSubscription : ISubscription
    {
        public bool IsReal => false;
        public SubscriptionModel Real => null;
    }
}
