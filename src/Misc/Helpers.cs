using System.Windows;
using System.IO;

namespace grubmod
{
    internal class Helpers
    {
        public static bool AllNotNull(params object?[] values) =>
            values.All(v => v is not null);

        public static string ConvertDecimalToHex(string? decimalValue) =>
            !string.IsNullOrEmpty(decimalValue) ? "0x" + Convert.ToString(int.Parse(decimalValue), 16) : string.Empty;

        public static bool AreThereChanges()
        {
            if (Grub.LoggedChanges.Count <= 0)
            {
                MessageBox.Show("Impossible to export.", "No changed options.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("No changed options! Impossible to export.", LogType.Warning);
                return false;
            }

            return true;
        }

        public static bool IsDataLoaded()
        {
            if (Grub.DefaultOptions.Count.Equals(0))
            {
                MessageBox.Show("Load the data!", string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
                Logger.Log("No data loaded.", LogType.Warning);
                return false;
            }

            return true;
        }

        public static bool IsFirstLineOfFileContainsTemplate(string path, string template)
        {
                if (File.Exists(path))
                {
                    var firstLine = File.ReadAllLines(path).FirstOrDefault() ?? string.Empty;
                    return firstLine.Equals(template);
                }

                return false;
        }
    }
}
