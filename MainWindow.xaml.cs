using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace grubmod
{
    public class ComboOrTextTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Option option)
            {
                switch (option.OptionType)
                {
                    case var s when s == Labels.NORMAL_OPTION_DEFINITION: return ComboTemplate;
                    case var s when s == Labels.NUMERIC_OPTION_DEFINITION: return TextTemplate;
                    case var s when s == Labels.CHECKBOX_OPTION_DEFINITION: return CheckBoxTemplate;
                    default: return base.SelectTemplate(item, container);
                }
            }

            return base.SelectTemplate(item, container);
        }
    }

    public partial class MainWindow : Window
    {
        public static GridViewColumn DescriptionColumnValue { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            optionsListView.ItemsSource = Grub.DefaultOptions;
            HideDescriptionColumn();
        }

        private void HideDescriptionColumn()
        {
            var gridview = optionsListView.View as GridView;
            DescriptionColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("Description"));
            gridview.Columns.Remove(DescriptionColumnValue);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var optionToFind = searchBox.Text;

            if (optionToFind == "Search.." || string.IsNullOrEmpty(optionToFind))
                return;

            var result = new ObservableCollection<Option>(await Task.Run(() => Grub.Options.Where(x => (bool)parametrRadioButton.IsChecked ?
            Grub.IsMatchCaseEnabled ? x.VarName.ToLower().Equals(optionToFind.ToLower())
            : x.VarName.ToLower().Contains(optionToFind.ToLower()) : Grub.IsMatchCaseEnabled ?
            x.VarDescription.ToLower().Equals(optionToFind.ToLower()) :
            x.VarDescription.ToLower().Contains(optionToFind.ToLower()))));

            if (result.Count > 0)
            {
                SetToAllComboBox.Items.Clear();
                optionsListView.ItemsSource = result;
                FindCommonValues();
                MessageBox.Show($"All options with \"{optionToFind}\" key printed.", "Found some options.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show($"No options with \"{optionToFind}\" key found.", "Invalid input.", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private void FindCommonValues()
        {
            var listOfMatchedOptions = new List<Option>();

            foreach (var option in optionsListView.Items)
                listOfMatchedOptions.Add(option as Option);

            foreach (var option in listOfMatchedOptions[0].VarValues)
            {
                if (listOfMatchedOptions.All(x => x.VarValues.Any(y => y.Equals(option, StringComparison.OrdinalIgnoreCase))))
                    SetToAllComboBox.Items.Add(new ComboBoxItem { Content = $"{option}", Foreground = Brushes.Black, Background = Brushes.White });
            }

            SetToAllComboBox.SelectedItem = SetToAllComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.DefaultOptions;
            SetToAllComboBox.Items.Clear();
            searchBox.Text = "Search..";
            searchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
        }

        private void SearchBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text == "Search..")
            {
                searchBox.Text = string.Empty;
                searchBox.Foreground = Brushes.White;
            }
        }

        private void SearchBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = "Search..";
                searchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
        }

        private void ShowDescription_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (!gridview.Columns.Contains(DescriptionColumnValue))
                gridview.Columns.Add(DescriptionColumnValue);
        }

        private void ShowDescription_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(DescriptionColumnValue))
                gridview.Columns.Remove(DescriptionColumnValue);
        }



        private void ApplyToAll_Click(object sender, RoutedEventArgs e)
        {
            if (SetToAllComboBox.Items.Count <= 0)
            {
                MessageBox.Show("No common values found", "No matches", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

        private void ShowAllOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.DefaultOptions;

        private void ShowAllNormalOptions_Checked(object sender, RoutedEventArgs e) =>
              optionsListView.ItemsSource = Grub.DefaultOptions.Where(x => x.OptionType.Equals(Labels.NORMAL_OPTION_DEFINITION));

        private void ShowAllCheckBoxOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.DefaultOptions.Where(x => x.OptionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION));

        private void ShowAllNumericOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.DefaultOptions.Where(x => x.OptionType.Equals(Labels.NUMERIC_OPTION_DEFINITION));

        private void MatchCase_Checked(object sender, RoutedEventArgs e) => Grub.IsMatchCaseEnabled = true;

        private void MatchCase_Unchecked(object sender, RoutedEventArgs e) => Grub.IsMatchCaseEnabled = false;
    }
}
