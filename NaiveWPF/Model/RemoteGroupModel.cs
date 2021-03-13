using System.Collections.ObjectModel;

namespace NaiveGUI.Model
{
    public class RemoteGroupModel : ObservableCollection<RemoteModel>
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

        public RemoteGroupModel(string name, MainViewModel model)
        {
            Name = name;
            CollectionChanged += (s, e) => model.ReloadTrayMenu();
        }

        public new void Add(RemoteModel item)
        {
            item.Group = this;
            base.Add(item);
        }

        public new void Insert(int position, RemoteModel item)
        {
            item.Group = this;
            base.Insert(position, item);
        }
    }
}
