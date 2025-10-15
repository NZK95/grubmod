using System;
using System.Collections.ObjectModel;
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
        private async void AsyncSearchButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("Search button used", LogType.Information);

            var optionToFind = searchBox.Text;

            if (optionToFind.Equals(Labels.SEARCH) || string.IsNullOrEmpty(optionToFind))
            {
                Logger.Log("Invalid input for search button", LogType.FailedOperation);
                return;
            }

            var result = new ObservableCollection<Option>(await Task.Run(() => Grub.DefaultOptions.Where(x => (bool)parametrRadioButton.IsChecked ?
            Grub.IsMatchCaseEnabled ? x.Fields.VarName.ToLower().Equals(optionToFind.ToLower())
            : x.Fields.VarName.ToLower().Contains(optionToFind.ToLower()) : Grub.IsMatchCaseEnabled ?
            x.Fields.VarDescription.ToLower().Equals(optionToFind.ToLower()) :
            x.Fields.VarDescription.ToLower().Contains(optionToFind.ToLower()))));

            if (result.Count > 0)
            {
                SetToAllComboBox.Items.Clear();
                optionsListView.ItemsSource = Grub.Options = result;

                ShowAllOptions.IsChecked = true;
                FindCommonValues();

                MessageBox.Show($"All options with \"{optionToFind}\" key printed.", $"Found {result.Count} matches.", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"All options with \"{optionToFind}\" key printed. Found {result.Count} matches.", LogType.Information);
            }
            else
            {
                MessageBox.Show($"No options with \"{optionToFind}\" key found.", "Invalid input.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log($"No options with \"{optionToFind}\" key found. Invalid input.", LogType.Warning);
            }

        }

        private void SearchBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Equals(Labels.SEARCH))
            {
                searchBox.Text = string.Empty;
                searchBox.Foreground = Brushes.White;
            }
        }

        private void SearchBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = Labels.SEARCH;
                searchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
        }
    }
}
