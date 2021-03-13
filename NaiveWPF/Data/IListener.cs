using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public interface IListener
    {
        bool IsReal { get; }
        ListenerModel Real { get; }
    }
}
