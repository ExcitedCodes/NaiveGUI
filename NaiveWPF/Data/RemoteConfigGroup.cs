using System.Collections.ObjectModel;

using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public class RemoteConfigGroup : ObservableCollection<RemoteConfig>
    {
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Name"));
            }
        }
        private string _name = null;

        public RemoteConfigGroup(string name, MainViewModel model)
        {
            Name = name;
            CollectionChanged += (s, e) => model.ReloadTrayMenu();
        }

        public new void Add(RemoteConfig item)
        {
            item.Group = this;
            base.Add(item);
        }

        public new void Insert(int position, RemoteConfig item)
        {
            item.Group = this;
            base.Insert(position, item);
        }
    }
}
