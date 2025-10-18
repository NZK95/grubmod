using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private async void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("ImportFile button used", LogType.Information);

                var result = GetUserPath(Labels.SECTION_PE32);
                if (!result.Success)
                    return;

                await SetNewFileInformation(result.Path);
                MessageBox.Show($"Found {Grub.DefaultOptions?.Count ?? 0} options.", "Data loaded successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"Found {Grub.DefaultOptions?.Count ?? 0} options.", LogType.Information);
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
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult is not true)
            {
                Logger.Log("No file selected.", LogType.Information);
                return (false, string.Empty);
            }

            var fileName = Path.GetFileName(openFileDialog.FileName);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Logger.Log("Selected file name is empty.", LogType.FailedOperation);
                return (false, string.Empty);
            }

            if (!fileName.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Invalid filetype / name.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("Invalid filetype / name.", LogType.FailedOperation);
                return (false, string.Empty);
            }

            return (true, openFileDialog.FileName);
        }

        private async Task SetNewFileInformation(string path)
        {
            Logger.Log($"Set new path - {path}.", LogType.SuccessfulOperation);

            Grub.PathToMainFile = path;

            var lines = await File.ReadAllLinesAsync(Grub.PathToMainFile).ConfigureAwait(false);
            await Dispatcher.InvokeAsync(() => BIOSFileParser.Lines = lines.ToList());

            var options = await BIOSFileParser.ExtractData().ConfigureAwait(false);

            await Dispatcher.InvokeAsync(() =>
            {
                Grub.Options = Grub.DefaultOptions = options;
                optionsListView.ItemsSource = Grub.DefaultOptions;
            });
        }
    }
}
