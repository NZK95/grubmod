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
        private bool AreThereCommonValues()
        {
            if (SetToAllComboBox.Items.Count <= 0)
            {
                MessageBox.Show("No common values found", "No matches", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("No common values found.", LogType.Warning);
                return false;
            }

            Logger.Log($"Common values found - {SetToAllComboBox.Items.Count}", LogType.Information);
            return true;
        }

        private void ApplyToAll_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("Apply to all button used.", LogType.Information);

            if (!AreThereCommonValues())
                return;

            var valueToSet = (SetToAllComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var listWithChangedSelectedValues = new ObservableCollection<Option>();

            for (var i = 0; i < optionsListView.Items.Count; i++)
            {
                var option = optionsListView.Items[i] as Option;
                option.VarSelectedValue = valueToSet;

                listWithChangedSelectedValues.Add(option);
            }

            optionsListView.ItemsSource = listWithChangedSelectedValues;
        }

        private void FindCommonValues()
        {
            var listOfMatchedOptions = new List<Option>();

            foreach (var option in optionsListView.Items)
                listOfMatchedOptions.Add(option as Option);

            foreach (var option in listOfMatchedOptions[0].Fields.VarValues)
            {
                if (listOfMatchedOptions.All(x => x.Fields.VarValues.Any(y => y.Equals(option, StringComparison.OrdinalIgnoreCase))))
                    SetToAllComboBox.Items.Add(new ComboBoxItem { Content = $"{option}", Foreground = Brushes.Black, Background = Brushes.White });
            }

            SetToAllComboBox.SelectedItem = SetToAllComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault();
        }
    }
}
