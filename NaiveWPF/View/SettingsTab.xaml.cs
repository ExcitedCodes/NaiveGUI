using System.Windows;
using System.Windows.Controls;

using NaiveGUI.Model;

namespace NaiveGUI.View
{
    /// <summary>
    /// SettingsTab.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        private readonly MainViewModel Model;

        public SettingsTab(MainViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Model?.SetLanguage((e.AddedItems[0] as ComboBoxItem).Tag as string);

        private void Save(object sender, RoutedEventArgs e) => Model.Save();
    }
}
