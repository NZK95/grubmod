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
            optionsListView.ItemsSource = Grub.DefaultOptions;
            DataContext = ViewModel;
            HideColumns();
            MessageBox.Show($"Found {Grub.DefaultOptions.Count} options.", "All data is loaded", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AsyncSearchButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key.Equals(Key.C))
            {
                ClearButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key.Equals(Key.G))
            {
                GoogleButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}
