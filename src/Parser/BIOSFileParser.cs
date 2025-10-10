using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Collections.Immutable;

namespace grubmod
{
    internal class BIOSFileParser
    {
        public static List<string> Lines { get; set; } = File.ReadAllLines(Grub.Path).ToList();

        public async static Task<ObservableCollection<Option>> ExtractInformation()
        {
            var options = new ObservableCollection<Option>();

            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];
                var optionType = BIOSParsingHelpers.ExtractOptionType(line);

                if (optionType is null) continue;

                try
                {
                    var varOffset = ExtractValueByLabel(i, Labels.VAROFFSET_DEFINITION);
                    var varStoreId = ExtractValueByLabel(i, Labels.VARSTOREID_DEFINITION);
                    var varDescription = ExtractValueByLabel(i, Labels.DESCRIPTION_DEFINITION);
                    var varSize = BIOSParsingHelpers.GetVarSizeValue(i, optionType);
                    var varName = ExtractValueByLabel(i, optionType);
                    var varSectionName = ExtractVarSectionName(varStoreId);
                    var varDefaultValue = ExtractDefaultValue(i, optionType);
                    var varValues = ExtractValues(i);
                    var varIndex = i;

                    options.Add(new Option(new OptionFields(optionType, varName, varOffset, varStoreId, varSectionName, varSize, varDescription, varDefaultValue, varIndex, varValues)));
                }
                catch (Exception)
                {
                    //Logger.Log();
                }
            }

            return new ObservableCollection<Option>(options.Distinct());
        }

        public static string ExtractHexValue(in Option option)
        {
            if (option.Fields.OptionType.Equals(Labels.NORMAL_OPTION_DEFINITION))
                return ExtractHexValueForNormalOption(option.Fields.VarIndex, option.VarSelectedValue);

            else if (option.Fields.OptionType.Equals(Labels.NUMERIC_OPTION_DEFINITION))
                return Helpers.ConvertDecimalToHex(option.VarSelectedValue);

            else
                return option.VarSelectedValue.Equals("True") ? "0x1" : "0x0";
        }

        private static string ExtractDefaultValue(int startIndex, string optionType)
        {
            var lines = BIOSParsingHelpers.ExtractListOfLinesRelatedToAnOption(startIndex);
            var defaultNumericValue = BIOSParsingHelpers.ExtractDefaultIdValueIfExists(lines);

            if (optionType.Equals(Labels.NUMERIC_OPTION_DEFINITION))
                return defaultNumericValue;

            else if (optionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION))
                return ExtractDefaultValueForCheckBoxes(lines);

            else
                return ExtractDefaultValueForNormalOption(lines, defaultNumericValue);
        }

        private static string ExtractHexValueForNormalOption(int index, string value)
        {
            var lines = BIOSParsingHelpers.ExtractListOfLinesRelatedToAnOption(index);

            foreach (var line in lines)
            {
                if (line.Contains(Labels.VALUE_DEFINITION) && line.Contains(value))
                {
                    var decimalValue = Regex.Match(line, @"Value:\s*(\d+)").Groups[1].Value;
                    return Helpers.ConvertDecimalToHex(decimalValue);
                }
            }

            return string.Empty;
        }

        private static string ExtractDefaultValueForCheckBoxes(List<string> lines) =>
          string.Join("", (from line in lines
                           where line.Contains(Labels.DEFAULT)
                           let startIndex = line.IndexOf(Labels.DEFAULT) + Labels.DEFAULT.Length + 1
                           let endIndex = line.IndexOf(',', startIndex)
                           select line[startIndex..endIndex].Trim()));

        private static string ExtractDefaultValueForNormalOption(List<string> lines, string defaultNumericValue)
        {
            foreach (var line in lines)
            {
                if (line.Contains(Labels.VALUE_DEFINITION))
                {
                    var valueName = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value.Trim().Replace("\"", string.Empty);
                    var conditionForLine = valueName.Contains(Labels.DEFAULT) ? Labels.MFG_DEFAULT : Labels.DEFAULT;

                    if (line.Contains(conditionForLine))
                        return valueName.Trim();
                }

                if (BIOSParsingHelpers.IsLineDefaultValueDefinition(line, defaultNumericValue))
                    return Regex.Match(line, "\"([^\"]+)\"").Groups[1].Value.Trim();
            }

            return Labels.NOT_FOUND;
        }

        public static string ExtractValueByLabel(int lineIndex, string label)
        {
            var lines = BIOSParsingHelpers.ExtractStringOfLinesRelatedToAnOption(lineIndex);

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
                        return value is null or "" ? Labels.NOT_FOUND : value;
                }
            }

            return Labels.NOT_FOUND;
        }

        private static string ExtractVarSectionName(string varStoreId)
        {
            var pattern = $"{Labels.VARSTOREID_DEFINITION}: {varStoreId}";

            foreach (var line in Lines)
            {
                if (BIOSParsingHelpers.IsLineVarSectionNameDefinition(line) && line.Contains(pattern))
                {
                    var startIndex = line.IndexOf(Labels.NAME) + Labels.NAME.Length;
                    return line[startIndex..].Replace("\"", string.Empty).Trim();
                }
            }

            return Labels.NOT_FOUND;
        }

        private static List<string> ExtractValues(int index)
        {
            var values = new List<string>();
            var allLinesRelatedToOption = BIOSParsingHelpers.ExtractStringOfLinesRelatedToAnOption(index);
            var matches = Regex.Matches(allLinesRelatedToOption, Labels.VALUE_DEFINITION);
            var indexes = matches.Cast<Match>().Select(m => m.Index).ToList();

            for (int i = 0; i < matches.Count; ++i)
            {
                var startIndex = indexes[i] + Labels.VALUE_DEFINITION.Length;
                var quoteStart = allLinesRelatedToOption.IndexOf('"', startIndex);
                var quoteEnd = allLinesRelatedToOption.IndexOf('"', quoteStart + 1);

                if (quoteStart == -1 || quoteEnd == -1)
                    continue;

                var extracted = allLinesRelatedToOption.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                values.Add(extracted);
            }

            return values;
        }
    }
}
