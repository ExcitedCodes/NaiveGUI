using System.Windows;

using NaiveGUI.Data;
using NaiveGUI.Model;

namespace NaiveGUI
{
    /// <summary>
    /// AddRemoteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddRemoteWindow : Window
    {
        private readonly AddRemoteViewModel Model;

        public AddRemoteWindow(RemoteModel config)
        {
            InitializeComponent();
            DataContext = Model = new AddRemoteViewModel(this, config);

            Title = MainViewModel.GetLocalized("AddRemote_EditTitle");
            text_add.Text = MainViewModel.GetLocalized("AddRemote_EditSave");
        }

        public AddRemoteWindow(RemoteGroupModel group, string name = null, string uri = null, string extra_headers = null)
        {
            InitializeComponent();
            DataContext = Model = new AddRemoteViewModel(this, group, name, uri, extra_headers);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Commit())
            {
                Close();
            }
        }
    }
}
