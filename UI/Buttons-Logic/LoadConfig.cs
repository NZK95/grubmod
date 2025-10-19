using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private async void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("LoadConfig button used", LogType.Information);

                if (!Helpers.IsDataLoaded())
                    return;

                MessageBox.Show("Read the documentation on GitHub, and respect the syntax.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);

                var result = GetUserPath(Labels.CONFIG);
                if (!result.Success)
                    return;

                var pairsOptionValue = await ParseConfigAsync(result.Path).ConfigureAwait(false);
                await Dispatcher.InvokeAsync(() => ApplyConfig(pairsOptionValue));

                MessageBox.Show("Config loaded. Analyze the logs.", $"Loaded {pairsOptionValue.Count} options", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log("Config loaded successfully.", LogType.SuccessfulOperation);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log($"{ex.Message} - {ex.Source}.", LogType.Exception);
            }
        }

        private async Task<Dictionary<string, string>> ParseConfigAsync(string path)
        {
            var lines = await File.ReadAllLinesAsync(path).ConfigureAwait(false);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            const char SEPARATOR = '|';

            foreach (var raw in lines)
            {
                var line = raw.Trim();  

                if (string.IsNullOrWhiteSpace(raw) || line.StartsWith('#') || !line.Contains('|'))
                {
                    Logger.Log($"{raw} is not added to dictionary.", LogType.Warning);
                    continue;
                }

                var result = line.Split(SEPARATOR);
                var key = result[0].Trim();
                var value = result[1].Trim();

                if (string.IsNullOrEmpty(key))
                {
                    Logger.Log($"Skipping config line with empty key: {line}", LogType.Warning);
                    continue;
                }

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                    Logger.Log($"Added {key} - {value} pair to dictionary.", LogType.SuccessfulOperation);
                }
                else
                    Logger.Log($"Dictionary already contains {key}.", LogType.FailedOperation);
            }

            return RemoveNonExistentOptions(dict);
        }


        private Dictionary<string, string> RemoveNonExistentOptions(Dictionary<string, string> dict)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (dict is null || dict.Count == 0)
            {
                Logger.Log("No valid pairs found in config.", LogType.Warning);
                return result;
            }

            foreach (var pair in dict)
            {
                var option = Grub.DefaultOptions.FirstOrDefault(opt => opt.Fields.VarName.Equals(pair.Key));
                
                if(option is null) 
                    continue;

                var value = pair.Value.Equals("Max") ? option.Fields.VarValues.Last() : pair.Value;

                if (option.Fields.VarValues.Contains(value))
                {
                    result[pair.Key] = value;
                    Logger.Log($"Option from config is found - {pair.Key}", LogType.SuccessfulOperation);
                }
                else
                    Logger.Log($"Option from config - {pair.Key} is not found", LogType.FailedOperation);
            }

            return result;
        }

        private void ApplyConfig(Dictionary<string, string> dict)
        {
            if (dict is null || dict.Count == 0)
            {
                Logger.Log("No options to apply from config.", LogType.Information);
                return;
            }

            foreach (var pair in dict)
            {
                var matches = Grub.DefaultOptions
                    .Select((opt, i) => (Option: opt, Index: i))
                    .Where(x => x.Option.Fields.VarName.Equals(pair.Key)).ToList();

                foreach (var opt in matches)
                {
                    Grub.DefaultOptions[opt.Index] = new Option(opt.Option.Fields) { VarSelectedValue = pair.Value };
                    Logger.Log($"Applied {pair.Key} = {pair.Value}", LogType.SuccessfulOperation);
                }
            }

            optionsListView.ItemsSource = Grub.DefaultOptions;
            Logger.Log($"All options from config are applied ({dict.Count})", LogType.SuccessfulOperation); ;
        }
    }
}
