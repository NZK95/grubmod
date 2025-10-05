using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Collections.Immutable;

namespace grubmod
{
    public class BIOSFileParser
    {
        public static readonly List<string> Lines = File.ReadAllLines(@"C:\Users\User\Desktop\BIOS\123.txt").ToList();

        private static bool IsLineVarSectionNameDefinition(string line) =>
           Labels.VarSectionNameLabels.All(line.Contains);

        private static bool IsLineDefaultValueDefinition(string line, string defaultNumericValue) =>
            line.Contains(Labels.VALUE_DEFINITION) && line.Contains(defaultNumericValue) && !string.IsNullOrEmpty(defaultNumericValue);

        public async static Task<ObservableCollection<Option>> ExtractInformation()
        {
            var options = new ObservableCollection<Option>();

            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];
                var optionType = line.Contains(Labels.NORMAL_OPTION_DEFINITION) ? Labels.NORMAL_OPTION_DEFINITION :
                                  line.Contains(Labels.NUMERIC_OPTION_DEFINITION) ? Labels.NUMERIC_OPTION_DEFINITION :
                                  line.Contains(Labels.CHECKBOX_OPTION_DEFINITION) ? Labels.CHECKBOX_OPTION_DEFINITION : null;

                if (optionType is null) continue;

                try
                {
                    var varSize = optionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION) ? "0x1" :"0x" + (Convert.ToInt32(ExtractValueByLabel(i, Labels.SIZE_DEFINITION)) / 8).ToString("X");
                    var varName = ExtractValueByLabel(i, optionType);
                    var varOffset = ExtractValueByLabel(i, Labels.VAROFFSET_DEFINITION);
                    var varStoreId = ExtractValueByLabel(i, Labels.VARSTOREID_DEFINITION);
                    var varSectionName = ExtractVarSectionName(varStoreId);
                    var varValues = ExtractValues(i);
                    var varDescription = ExtractValueByLabel(i, Labels.DESCRIPTION_DEFINITION);
                    var varDefaultValue = ExtractDefaultValue(i, optionType);

                    options.Add(new Option(new OptionFields(optionType, varName, varOffset, varStoreId, varSectionName, varSize, varDescription, varDefaultValue, varValues)));
                }
                catch (Exception) { }
            }

            return new ObservableCollection<Option>(options.Distinct());
        }

        private static string ExtractValueByLabel(int lineIndex, string label)
        {
            var lines = ExtractAllLinesRelatedToAnOption(lineIndex);

            //To fix the problem with some names.
            lines = Regex.Replace(lines, "\"\\s+", "\"");
            lines = Regex.Replace(lines, "\\s+\"", "\"");

            var pattern = @",(?![^""]*"" )";
            var parts = Regex.Split(lines, pattern);

            foreach (var part in parts)
            {
                var idx = part.IndexOf(':');

                if (idx > 0)
                {
                    var key = part[..idx].Replace("\"", string.Empty).Replace(":", string.Empty).Trim();
                    var value = part[idx..].Replace("\"", string.Empty).Replace(":", string.Empty).Trim();

                    if (key.Equals(label, StringComparison.OrdinalIgnoreCase))
                        return value is null or "" ? "N/A" : value;
                }
            }

            return "N/A";
        }

        private static string ExtractVarSectionName(string varStoreId)
        {
            foreach (var line in Lines)
            {
                if (IsLineVarSectionNameDefinition(line) && line.Contains($"{Labels.VARSTOREID_DEFINITION}: {varStoreId}"))
                {
                    var startIndex = line.IndexOf("Name:") + "Name:".Length;
                    return line[startIndex..].Replace("\"", string.Empty).Trim();
                }
            }

            return "N/A";
        }

        private static List<string> ExtractValues(int index)
        {
            var values = new List<string>();
            var allLinesRelatedToOption = ExtractAllLinesRelatedToAnOption(index);
            var matches = Regex.Matches(allLinesRelatedToOption, Labels.VALUE_DEFINITION);
            var indexes = matches.Cast<Match>().Select(m => m.Index).ToList();

            for (int i = 0; i < matches.Count; ++i)
            {
                var startIndex = indexes[i] + Labels.VALUE_DEFINITION.Length;
                var quoteStart = allLinesRelatedToOption.IndexOf('"', startIndex);
                var quoteEnd = allLinesRelatedToOption.IndexOf('"', quoteStart + 1);

                if (quoteStart == -1 || quoteEnd == -1)
                    continue;

                string extracted = allLinesRelatedToOption.Substring(
                quoteStart + 1,
                quoteEnd - quoteStart - 1);

                values.Add(extracted);
            }

            return values;
        }

        private static string ExtractDefaultValue(int startIndex, string optionType)
        {
            var endIndex = Lines.FindIndex(startIndex, x => x.Trim().Equals("End"));
            var lines = Lines[startIndex..endIndex];
            var defaultNumericValue = RecoverDefaultIdValueIfExists(lines);

            if (optionType.Equals(Labels.NUMERIC_OPTION_DEFINITION))
                return defaultNumericValue;

            if (optionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION))
                return GetDefaultValueForCheckBoxes(lines);

            foreach (var line in lines)
            {
                if (line.Contains(Labels.VALUE_DEFINITION))
                {
                    var valueName = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value.Trim().Replace("\"", string.Empty);
                    var conditionForLine = valueName.Contains("Default") ? "Default, MfgDefault" : "Default";

                    if (line.Contains(conditionForLine))
                        return valueName.Trim();
                }

                if (IsLineDefaultValueDefinition(line, defaultNumericValue))
                    return Regex.Match(line, "\"([^\"]+)\"").Groups[1].Value.Trim();
            }

            return "N/A";
        }

        private static string RecoverDefaultIdValueIfExists(List<string> lines) => lines
            .Where(line => line.Contains("Default DefaultId:"))
            .Select(line => Regex.Match(line, @"Value:\s*(\S+)\s*$").Groups[1].Value.Replace("\"", string.Empty).Trim())
            .FirstOrDefault() ?? string.Empty;

        private static string GetDefaultValueForCheckBoxes(List<string> lines) =>
            string.Join("", (from line in lines
                             where line.Contains("Default:")
                             let startIndex = line.IndexOf("Default:") + "Default:".Length
                             let endIndex = line.IndexOf(',', startIndex)
                             select line[startIndex..endIndex].Trim()));

        private static string ExtractAllLinesRelatedToAnOption(int startIndex)
        {
            var endIndex = Lines.FindIndex(startIndex, x => x.Trim().Equals("End"));
            return string.Join("\n", Lines[startIndex..endIndex]);
        }
    }
}
