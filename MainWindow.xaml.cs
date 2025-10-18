using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        public static MainViewModel ViewModel { get; } = new MainViewModel();
        public static GridViewColumn DescriptionColumnValue { get; private set; }
        public static GridViewColumn VarStoreIdColumnValue { get; private set; }
        public static GridViewColumn BIOSDefaultValueColumnValue { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            HideColumns();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AsyncSearchButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                switch (e.Key)
                {
                    case Key.C:
                        ClearButton_Click(sender, new RoutedEventArgs());
                        break;
                    case Key.G:
                        GoogleButton_Click(sender, new RoutedEventArgs());
                        break;
                    case Key.E:
                        ExportScript_Click(sender, new RoutedEventArgs());
                        break;
                    case Key.L:
                        LoadConfig_Click(sender, new RoutedEventArgs());
                        break;
                }

                e.Handled = true;
            }
        }
    }
}
