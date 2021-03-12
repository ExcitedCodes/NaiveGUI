using NaiveGUI.Model;

namespace NaiveGUI.Helper
{
    public class TabIndexTester
    {
        public MainViewModel Model;

        public bool this[int offset] => Model.CurrentTab == offset;

        public TabIndexTester(MainViewModel model)
        {
            Model = model;
        }
    }
}
