namespace NaiveGUI.Data
{
    public class FakeListener : IListener
    {
        public bool IsReal => false;
        public Listener Real => null;
    }
}
