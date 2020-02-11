using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NaiveGUI.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool RaisePropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return PropertyChanged == null;
        }
    }
}
