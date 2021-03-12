using System;
using System.Windows;

using NaiveGUI.Data;
using NaiveGUI.Model;

namespace NaiveGUI
{
    /// <summary>
    /// AddSubscriptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddSubscriptionWindow : Window
    {
        private readonly MainViewModel Model;

        public AddSubscriptionWindow(MainViewModel model)
        {
            InitializeComponent();
            Model = model;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var uri = textBox_URI.Text;
            var name = textBox_Name.Text;

            if (uri.Trim() == "" || name.Trim() == "")
            {
                return;
            }
            Model.Subscriptions.Insert(Model.Subscriptions.Count - 1, new Subscription(Model, name, uri, false, DateTime.MinValue));
            Model.Save();

            Close();
        }
    }
}
