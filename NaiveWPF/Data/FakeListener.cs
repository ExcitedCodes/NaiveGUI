using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public class FakeListener : ModelBase, IListener
    {
        private readonly MainViewModel Model;

        public bool IsReal => false;
        public ListenerModel Real => null;

        public bool AllowAddListener => Model.AllowAddListener;

        public FakeListener(MainViewModel model)
        {
            Model = model;
            model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Model.AllowAddListener))
                {
                    RaisePropertyChanged(nameof(AllowAddListener));
                }
            };
        }
    }
}
