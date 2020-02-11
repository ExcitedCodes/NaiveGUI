using System;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

using NaiveGUI.Data;

namespace NaiveGUI.View
{
    /// <summary>
    /// SubscriptionTab.xaml 的交互逻辑
    /// </summary>
    public partial class SubscriptionTab : UserControl, INotifyPropertyChanged
    {
        private readonly MainWindow Main = null;

        public ObservableCollection<SubscriptionData> Subscriptions { get; private set; } = new ObservableCollection<SubscriptionData>();

        public bool AutoUpdate
        {
            get => UpdateInterval != -1;
            set
            {
                UpdateInterval = value ? 7200 : -1;
                RaisePropertyChanged();
                RaisePropertyChanged("IntervalOpacity");
            }
        }

        public int UpdateInterval
        {
            get => _updateInterval;
            set
            {
                if(_updateInterval != value)
                {
                    _updateInterval = value;
                    RaisePropertyChanged("UpdateInterval");
                    Main.Save();
                }
            }
        }
        private int _updateInterval = -1;

        public Prop<bool> Updating { get; set; } = new Prop<bool>();

        public Prop<DateTime> LastUpdate { get; set; } = new Prop<DateTime>(DateTime.MinValue);

        public double IntervalOpacity => AutoUpdate ? 1 : 0.5;

        public SubscriptionData CurrentSubscription = null;

        public SubscriptionTab(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            DataContext = this;
        }

        public int Update(bool silent = true, bool force = false)
        {
            if(!force && (UpdateInterval < 0 || (DateTime.Now - LastUpdate).TotalSeconds < UpdateInterval))
            {
                return -1;
            }
            int count = 0;
            foreach(var s in Subscriptions)
            {
                if(s.Enabled && s.Update(silent))
                {
                    count++;
                }
            }
            LastUpdate.Value = DateTime.Now;
            Main.Save();
            return count;
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count != 0)
            {
                CurrentSubscription = e.AddedItems[0] as SubscriptionData;
            }
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e) => Main.Save();

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(Updating)
            {
                return;
            }
            Updating.Value = true;
            ThreadPool.QueueUserWorkItem((s) =>
            {
                var count = Update(false, true);
                Main.snackbarMessageQueue.Enqueue(string.Format("Updated {0} subscription{1}.", count, count > 1 ? "s" : ""));
                Updating.Value = false;
            });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
