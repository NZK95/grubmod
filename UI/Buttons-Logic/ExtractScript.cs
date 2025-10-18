using System.Windows;
using System.IO;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("ExportFile button used", LogType.Information);

                if (!Helpers.AreThereChanges())
                    return;

                var path = Grub.PathToScript;

                if (!Helpers.IsFirstLineOfFileContainsTemplate(path, Grub.ReservedStrings.First()))
                    InsertScriptTemplate(path);

                File.AppendAllLines(path, Grub.LoggedChanges);
                InsertEndOfScript(path);
                SelectTheLastValue(path);

                MessageBox.Show($"Script exported to - {path}.", $"Exported {Grub.LoggedChanges.Count} options.", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"Script exported to - {path}. Exported {Grub.LoggedChanges.Count} options.", LogType.SuccessfulOperation);
                Grub.LoggedChanges.Clear();
                ViewModel.Name = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log($"{ex.Message} - {ex.Source}", LogType.Exception);
            }
        }

        private void InsertScriptTemplate(string path) =>
            File.WriteAllLines(path, Grub.ReservedStrings);

        private void InsertEndOfScript(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            if (!lines.Contains(Labels.END_OF_SCRIPT))
                lines.Add("\n" + Labels.END_OF_SCRIPT);
            else if (!lines[^1].Equals(Labels.END_OF_SCRIPT))
            {
                lines.Remove(Labels.END_OF_SCRIPT);
                lines.Add("\n" + Labels.END_OF_SCRIPT);
            }

            File.WriteAllLines(path, lines);
        }

        private void SelectTheLastValue(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            var commentedLines = lines.Skip(Grub.ReservedStrings.Count)
                .Where(line => line.StartsWith('#')).ToList();

            var names = (from line in commentedLines
                         let endIndex = line.IndexOf('-')
                         select line[..endIndex].Trim()).ToList();

            var hasDuplicates = !names.Count().Equals(names.Distinct().Count());

            if (!hasDuplicates)
                return;

            var pairs = ExtractPairs(names, lines);
            var result = ExtractFilteredPairs(pairs);
            var modifiedLines = ExtractModifiedLines(lines, result);
            var modifiedAndFormattedLines = FormatLines(modifiedLines);

            File.WriteAllLines(path, modifiedAndFormattedLines);
        }

        private List<KeyValuePair<string, List<int>>> ExtractPairs(List<string> names, List<string> lines)
        {
            var pairs = new List<KeyValuePair<string, List<int>>>();

            foreach (var name in names)
            {
                var indexes = new List<int>();

                for (var i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains(name))
                        indexes.Add(i);
                }

                pairs.Add(new KeyValuePair<string, List<int>>(name, indexes));
            }

            return pairs;
        }

        private List<KeyValuePair<string, List<int>>> ExtractFilteredPairs(List<KeyValuePair<string, List<int>>> pairs) =>
            pairs.Where(p => p.Value.Count > 1).GroupBy
            (p => new
            {
                p.Key,
                Values = string.Join(",", p.Value)
            }).Select(g => g.First()).ToList();

        private List<string> ExtractModifiedLines(List<string> lines, List<KeyValuePair<string, List<int>>> pairs)
        {
            const int TIMES_TO_REMOVE_RELATED_LINES = 2;

            foreach (var pair in pairs)
            {
                var lastIndex = pair.Value.Max();

                foreach (var index in pair.Value)
                {
                    if (index == lastIndex)
                        continue;

                    for (var i = 0; i < TIMES_TO_REMOVE_RELATED_LINES; ++i)
                        lines[index + i] = string.Empty;
                }
            }

            return lines;
        }

        private List<string> FormatLines(List<string> lines)
        {
            const int INDEX_AFTER_ALL_WATERMARKS = 3;

            lines.RemoveAll(string.IsNullOrEmpty);
            lines.Insert(INDEX_AFTER_ALL_WATERMARKS, string.Empty);
            lines.Insert(lines.Count - 1, string.Empty);

            return lines;
        }
    }
}
