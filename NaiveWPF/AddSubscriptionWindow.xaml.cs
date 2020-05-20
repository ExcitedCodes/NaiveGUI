using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using NaiveGUI.Data;
using NaiveGUI.Helper;

namespace NaiveGUI
{
    /// <summary>
    /// AddSubscriptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddSubscriptionWindow : Window, INotifyPropertyChanged
    {
        private readonly MainWindow Main = null;

        public ObservableCollection<ISubscription> Subscriptions = null;

        public Prop<string> SubscriptionName { get; set; } = new Prop<string>();
        public Prop<string> SubscriptionURI { get; set; } = new Prop<string>();

        public AddSubscriptionWindow(MainWindow main, ObservableCollection<ISubscription> subs)
        {
            InitializeComponent();
            DataContext = this;

            Main = main;
            Subscriptions = subs;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriptionName.Value.Trim() == "" || SubscriptionURI.Value.Trim() == "")
            {
                return;
            }
            Subscriptions.Insert(Subscriptions.Count - 1, new Subscription(Main, SubscriptionName.Value, SubscriptionURI.Value, false, DateTime.MinValue));
            Main.Save();
            Close();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
