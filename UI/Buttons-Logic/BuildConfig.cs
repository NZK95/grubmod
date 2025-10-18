using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void BuildConfig_Click(object sender, RoutedEventArgs e)
        {
            if (Grub.OptionValueStrings.Count is 0)
            {
                MessageBox.Show("No options to build config file.", "Nothing to build", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("No options to build config file.", LogType.Warning);
                return;
            }

            var path = Grub.PathToConfig;

            try
            {
                if (!Helpers.IsFirstLineOfFileContainsTemplate(path, Grub.ReservedStrings.First()))
                    InsertScriptTemplate(path);

                File.AppendAllLines(path, Grub.OptionValueStrings);
                var finalLines = ChooseOnlyTheLastValues(path);
                File.WriteAllLines(path, finalLines);

                MessageBox.Show($"Config file built to - {path}.", $"Built {Grub.OptionValueStrings.Count} options.", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"Config file built to - {path}. Built {Grub.OptionValueStrings.Count} options.", LogType.SuccessfulOperation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log($"{ex.Message} - {ex.Source}", LogType.Exception);
            }
        }

        private List<string> ChooseOnlyTheLastValues(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            var onlyNames = lines.Skip(Grub.ReservedStrings.Count).Where(line => !string.IsNullOrEmpty(line)).
                Select(x => x.Split('|')[0].Trim()).ToList();
            var hasDuplicates = !onlyNames.Count().Equals(onlyNames.Distinct().Count());

            if (!hasDuplicates)
                return lines;

            var result = new List<string>();

            foreach (var name in onlyNames.Distinct())
            {
                var lastValue = lines.Where(x => x.Split('|')[0].Trim().Equals(name, StringComparison.OrdinalIgnoreCase)).Last();
                result.Add(lastValue);
            }

            return Grub.ReservedStrings.Concat(result).Distinct().ToList();
        }
    }
}
