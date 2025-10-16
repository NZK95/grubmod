using Microsoft.Win32;
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
        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("ImportFile button used", LogType.Information);

                var result = GetUserPath(Labels.SECTION_PE32);

                if (result.Success is false)
                    return;

                SetNewFileInformation(result.Path);
                MessageBox.Show($"Found {Grub.DefaultOptions.Count} options.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"Found {Grub.DefaultOptions.Count} options.", LogType.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log($"{ex.Message} - {ex.Source}", LogType.Exception);
            }
        }

        private (bool Success, string Path) GetUserPath(string pattern)
        {
            var openFileDialog = new OpenFileDialog() { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            bool? result = openFileDialog.ShowDialog();
            var name = openFileDialog.FileName[(openFileDialog.FileName.LastIndexOf('\\') + 1)..];

            if (result is not true)
            {
                Logger.Log("No file selected.", LogType.Information);
                return (false, "");
            }

            if (!name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Invalid filetype / name.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("Invalid filetype / name.", LogType.FailedOperation);
                return (false, "");
            }

            return (true, openFileDialog.FileName);
        }

        private void SetNewFileInformation(string path)
        {
            Logger.Log($"Path - {Grub.Path} changed to {path}.", LogType.SuccessfulOperation);

            Grub.Path = path;
            BIOSFileParser.Lines = File.ReadAllLines(Grub.Path).ToList();
            Grub.Options = Grub.DefaultOptions = BIOSFileParser.ExtractData().GetAwaiter().GetResult();
            optionsListView.ItemsSource = Grub.DefaultOptions;
        }
    }
}
