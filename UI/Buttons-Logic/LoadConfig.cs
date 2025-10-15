using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("LoadConfig button used", LogType.Information);

                if (!Helpers.IsDataLoaded())
                    return;

                MessageBox.Show("Read the documentation on GitHub, and respect the syntax.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);

                var result = GetUserPath(Labels.CONFIG);

                if (result.Success is false)
                    return;

                var pairsOptionValue = ParseConfig(result.Path);
                ApplyConfig(pairsOptionValue);

                MessageBox.Show("Config loaded. Analyze the logs.", $"Loaded {pairsOptionValue.Count} options", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log("Config loaded successfully!", LogType.SuccessfulOperation);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log($"{ex.Message} - {ex.Source}.", LogType.Exception);
            }
        }

        private Dictionary<string, string> ParseConfig(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            var dict = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                char? hyphenType = line.Contains('|') ? '|' : null;
                Logger.Log($"{line} is not added to dictionary.", LogType.Warning);

                if (hyphenType is not null)
                {
                    var result = line.Split((char)hyphenType);
                    var key = result[0].Trim();
                    var value = result[1].Trim();

                    if (dict.ContainsKey(key))
                        Logger.Log($"Dictionary already contains {key}.", LogType.FailedOperation);
                    else
                    {
                        dict.Add(key, value);
                        Logger.Log($"Added {key} - {value} pair to dictionary.", LogType.SuccessfulOperation);
                    }
                }
            }

            return RemoveNonExistentOptions(dict);
        }

        private Dictionary<string, string> RemoveNonExistentOptions(Dictionary<string, string> dict)
        {
            var result = new Dictionary<string, string>();

            foreach (var pair in dict)
            {
                var option = Grub.DefaultOptions.FirstOrDefault(opt => opt.Fields.VarName.Equals(pair.Key));

                if (option is not null && option.Fields.VarValues.Contains(pair.Value))
                {
                    result[pair.Key] = pair.Value;
                    Logger.Log($"Option from config is found - {pair.Key}", LogType.SuccessfulOperation);
                }
                else
                    Logger.Log($"Option from config - {pair.Key} is not found", LogType.FailedOperation);
            }

            return result;
        }

        private void ApplyConfig(Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                var result = Grub.DefaultOptions.Select((opt, i) => (Option: opt, Index: i)).Where(x => x.Option.Fields.VarName.Equals(pair.Key)).ToList();

                foreach (var res in result)
                    Grub.DefaultOptions[res.Index] = new Option(res.Option.Fields) { VarSelectedValue = pair.Value };
            }

            optionsListView.ItemsSource = Grub.DefaultOptions;
            Logger.Log($"All options from config are applied ({dict.Count})", LogType.SuccessfulOperation); ;
        }
    }
}
