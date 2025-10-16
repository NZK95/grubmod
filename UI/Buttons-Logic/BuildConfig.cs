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

            var path = @"C:\Users\User\Desktop\builded-config.txt";

            if (!Helpers.IsFirstLineOfFileContainsTemplate(path, Grub.ReservedStrings.First()))
                InsertScriptTemplate(path);

            File.AppendAllLines(path, Grub.OptionValueStrings);
            File.WriteAllLines(path, ChooseOnlyTheLastValues(path));

            MessageBox.Show($"Config file builded to - {path}.", $"Builded {Grub.OptionValueStrings.Count} options.", MessageBoxButton.OK, MessageBoxImage.Information);
            Logger.Log($"Config file builded to - {path}. Builded {Grub.OptionValueStrings.Count} options.", LogType.SuccessfulOperation);
        }

        private List<string> ChooseOnlyTheLastValues(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            var onlyNames = lines.Skip(3).Where(line => !string.IsNullOrEmpty(line)).Select(x => x.Split('|')[0].Trim()).ToList();
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
