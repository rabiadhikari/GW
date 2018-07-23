using System.Diagnostics;
using System.Windows;

namespace REVIT_GW_WPFAddin.Views
{
    public partial class GenslerWindow : Window
    {
        public GenslerWindow()
        {
            InitializeComponent();
            label_version.Content = Globals.ToolVersion;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void button_Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Globals.KnowHelpLink);
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
    }
}