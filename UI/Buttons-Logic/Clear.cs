using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options = Grub.DefaultOptions;
            ShowAllOptions.IsChecked = true;
            SetToAllComboBox.Items.Clear();
            searchBox.Text = Labels.SEARCH;
            searchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));

            Logger.Log("Clear button used", LogType.SuccessfulOperation);
        }
    }
}
