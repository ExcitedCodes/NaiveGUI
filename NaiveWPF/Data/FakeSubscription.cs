namespace NaiveGUI.Data
{
    public class FakeSubscription : ISubscription
    {
        public bool IsReal => false;
        public Subscription Real => null;
    }
}
