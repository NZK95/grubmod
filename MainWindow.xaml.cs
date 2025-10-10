using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
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
using Microsoft.Win32;

namespace grubmod
{
    internal class ComboOrTextTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Option option)
            {
                switch (option.Fields.OptionType)
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
        public static GridViewColumn DescriptionColumnValue { get; private set; }
        public static GridViewColumn VarStoreIdColumnValue { get; private set; }
        public static GridViewColumn BIOSDefaultValueColumnValue { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            optionsListView.ItemsSource = Grub.DefaultOptions;
            HideColumns();
        }

        private void HideColumns()
        {
            var gridview = optionsListView.View as GridView;

            DescriptionColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("Description"));
            VarStoreIdColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("VarStoreId"));
            BIOSDefaultValueColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("BIOS Default"));

            gridview.Columns.Remove(DescriptionColumnValue);
            gridview.Columns.Remove(VarStoreIdColumnValue);
            gridview.Columns.Remove(BIOSDefaultValueColumnValue);
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

        private void SetNewFileInformation(string path)
        {
            Grub.Path = path;
            BIOSFileParser.Lines = File.ReadAllLines(Grub.Path).ToList();
            Grub.Options = Grub.DefaultOptions = BIOSFileParser.ExtractInformation().GetAwaiter().GetResult();
            optionsListView.ItemsSource = Grub.DefaultOptions;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                SearchButton_Click(sender, new RoutedEventArgs());
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

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var optionToFind = searchBox.Text;

            if (optionToFind == "Search.." || string.IsNullOrEmpty(optionToFind))
                return;

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
            }
            else
                MessageBox.Show($"No options with \"{optionToFind}\" key found.", "Invalid input.", MessageBoxButton.OK, MessageBoxImage.Warning);

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

        private void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Equals("Search..") || string.IsNullOrEmpty(searchBox.Text))
                return;

            var url = "https://www.google.com/search?q=" + Uri.EscapeDataString(searchBox.Text) + " BIOS setting.";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog() { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            bool? result = openFileDialog.ShowDialog();
            var name = openFileDialog.FileName[(openFileDialog.FileName.LastIndexOf('\\') + 1)..];

            if (result is not true)
                return;

            if (!name.StartsWith(Labels.Section_PE32))
            {
                MessageBox.Show("Invalid filetype / name.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetNewFileInformation(openFileDialog.FileName);
            MessageBox.Show($"Found {Grub.DefaultOptions.Count} options.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options = Grub.DefaultOptions;
            ShowAllOptions.IsChecked = true;
            SetToAllComboBox.Items.Clear();
            searchBox.Text = "Search..";
            searchBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
        }

        private void ShowDescription_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (!gridview.Columns.Contains(DescriptionColumnValue))
                gridview.Columns.Insert(gridview.Columns.Count, DescriptionColumnValue);
        }

        private void ShowDescription_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(DescriptionColumnValue))
                gridview.Columns.Remove(DescriptionColumnValue);
        }

        private void ShowVarStoreId_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (!gridview.Columns.Contains(VarStoreIdColumnValue))
                gridview.Columns.Insert(gridview.Columns.Count - (gridview.Columns.Count - 2), VarStoreIdColumnValue);
        }

        private void ShowVarStoreId_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(VarStoreIdColumnValue))
                gridview.Columns.Remove(VarStoreIdColumnValue);
        }

        private void ShowBIOSDefaultValue_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;
            var index = gridview.Columns.Contains(DescriptionColumnValue) ? gridview.Columns.Count - 1 :
                gridview.Columns.Count;

            if (!gridview.Columns.Contains(BIOSDefaultValueColumnValue))
                gridview.Columns.Insert(index, BIOSDefaultValueColumnValue);
        }

        private void ShowBIOSDefaultValue_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(BIOSDefaultValueColumnValue))
                gridview.Columns.Remove(BIOSDefaultValueColumnValue);

        }

        private void ShowAllOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.Options;

        private void ShowAllNormalOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.NORMAL_OPTION_DEFINITION));

        private void ShowAllCheckBoxOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION));

        private void ShowAllNumericOptions_Checked(object sender, RoutedEventArgs e) =>
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.NUMERIC_OPTION_DEFINITION));

        private void MatchCase_Checked(object sender, RoutedEventArgs e) => Grub.IsMatchCaseEnabled = true;

        private void MatchCase_Unchecked(object sender, RoutedEventArgs e) => Grub.IsMatchCaseEnabled = false;

        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Grub.LoggedChanges.Count <= 0)
                {
                    MessageBox.Show("Impossible to export.", "No changed options.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var path = @"C:\Users\User\Desktop\setupvar-script.nsh";

                if (!DoWatermarksExists(path))
                    WriteWatermarks(path);

                File.AppendAllLines(path, Grub.LoggedChanges);
                InsertEndOfScript(path);
                SelectTheLastValue(path);

                MessageBox.Show($"Script exported to - {path}.", $"Exported {Grub.LoggedChanges.Count} options.", MessageBoxButton.OK, MessageBoxImage.Information);
                Grub.LoggedChanges.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool DoWatermarksExists(string path)
        {
            if (File.Exists(path))
            {
                var firstLine = File.ReadAllLines(path).FirstOrDefault() ?? string.Empty;
                return firstLine.Equals(Grub.ReservedStrings.First());
            }

            return false;
        }

        public void WriteWatermarks(string path) => File.WriteAllLinesAsync(path, Grub.ReservedStrings);

        public void InsertEndOfScript(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            if (!lines.Contains(Labels.END_OF_SCRIPT))
                lines.Add(Labels.END_OF_SCRIPT);

            else if (!lines.ElementAt(lines.Count - 1).Equals(Labels.END_OF_SCRIPT))
            {
                lines.Remove(Labels.END_OF_SCRIPT);
                lines.Add(Labels.END_OF_SCRIPT);
            }

            File.WriteAllLines(path, lines);
        }

        public void SelectTheLastValue(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            var names = lines.Where(line => line.StartsWith('#')).Select(line => line[..line.IndexOf('-')].Trim());

            var hasDuplicates = lines.Count.Equals(names.Distinct().Count());

            if (!hasDuplicates) return;

            foreach (var name in names)
            {
                foreach (var line in lines)
                {

                }#if

            }
        }

        private void CreateConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_RequestNavigateToGrubmod(object o, RequestNavigateEventArgs r)
        {
            var url = "https://www.github.com/NZK95/grubmod/";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void Hyperlink_RequestNavigateToAuthor(object o, RequestNavigateEventArgs r)
        {
            var url = "https://www.github.com/NZK95";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
