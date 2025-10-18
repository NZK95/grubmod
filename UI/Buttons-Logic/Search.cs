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

            bool searchByName = parametrRadioButton.IsChecked == true;
            bool matchCase = Grub.IsMatchCaseEnabled;
            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            Func<Option,bool> equalsPredicate = searchByName 
                ? new Func<Option,bool>(opt => opt.Fields.VarName.Equals(optionToFind,comparison))
                : new Func<Option,bool>(opt => opt.Fields.VarDescription.Equals(optionToFind,comparison));

            Func<Option, bool> containsPredicate = searchByName
           ? new Func<Option, bool>(opt => opt.Fields.VarName.Contains(optionToFind, comparison))
           : new Func<Option, bool>(opt => opt.Fields.VarDescription.Contains(optionToFind, comparison));

            try
            {
                var matches = await Task.Run(() =>
                {
                    var source = Grub.DefaultOptions;
                    return source.Where(opt => matchCase ? equalsPredicate(opt) : containsPredicate(opt));
                }).ConfigureAwait(false);

                await Dispatcher.InvokeAsync(() =>
                {
                    var result = new ObservableCollection<Option>(matches);

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
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log($"{ex.Message} - {ex.Source}", LogType.Exception);
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
