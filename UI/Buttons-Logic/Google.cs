using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Equals(Labels.SEARCH) || string.IsNullOrEmpty(searchBox.Text))
                return;

            var url = "https://www.google.com/search?q=" + Uri.EscapeDataString(searchBox.Text) + Labels.GOOGLE_SEARCH_PATTERN;
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            Logger.Log("Google button used", LogType.SuccessfulOperation);
        }
    }
}
