using System.ComponentModel;

namespace NaiveGUI
{
    public class Prop<T> : INotifyPropertyChanged where T : struct
    {
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }
        private T _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public Prop(T initial = default(T)) => _value = initial;

        public override string ToString() => _value.ToString();
        public static implicit operator T(Prop<T> p) => p.Value;
    }
}
