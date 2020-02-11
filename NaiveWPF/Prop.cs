using NaiveGUI.Model;

namespace NaiveGUI
{
    public class Prop<T> : ModelBase
    {
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }
        private T _value;

        public Prop(T initial = default(T)) => _value = initial;

        public override string ToString() => _value.ToString();
        public static implicit operator T(Prop<T> p) => p.Value;
    }
}
