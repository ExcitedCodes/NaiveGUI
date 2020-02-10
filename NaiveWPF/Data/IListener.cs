namespace NaiveGUI.Data
{
    public interface IListener
    {
        bool IsReal { get; }
        Listener Real { get; }
    }
}
