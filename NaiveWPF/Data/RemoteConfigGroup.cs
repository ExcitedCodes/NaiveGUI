using System.Collections.ObjectModel;

namespace NaiveGUI.Data
{
    public class RemoteConfigGroup : ObservableCollection<RemoteConfig>
    {
        public string Name { get; set; } = null;

        public RemoteConfigGroup(string name)
        {
            Name = name;
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
