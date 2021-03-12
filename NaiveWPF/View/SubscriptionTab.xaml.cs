using System.Windows;
using System.Threading;
using System.Windows.Controls;

using NaiveGUI.Data;
using NaiveGUI.Model;

namespace NaiveGUI.View
{
    /// <summary>
    /// SubscriptionTab.xaml 的交互逻辑
    /// </summary>
    public partial class SubscriptionTab : UserControl
    {
        private readonly MainViewModel Model;

        public Timer updateTimer = null;

        public Subscription CurrentSubscription = null;

        public SubscriptionTab(MainViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                CurrentSubscription = e.AddedItems[0] as Subscription;
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SubscriptionUpdating)
            {
                return;
            }
            Model.SubscriptionUpdating = true;
            ThreadPool.QueueUserWorkItem((s) =>
            {
                var count = Model.UpdateSubscription(false, true);
                Model.SubscriptionUpdating = false;
                Model.View.snackbarMessageQueue.Enqueue(string.Format(MainViewModel.GetLocalized("Subscription_UpdateNotification"), count, count > 1 ? "s" : ""));
            });
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => new AddSubscriptionWindow(Model).ShowDialog();

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Subscription s)
            {
                Model.Subscriptions.Remove(s);
                Model.Save();
            }
        }

        private void Save(object sender, RoutedEventArgs e) => Model.Save();
    }
}
