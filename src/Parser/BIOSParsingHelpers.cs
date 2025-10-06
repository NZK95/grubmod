using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace grubmod
{
    internal class BIOSParsingHelpers
    {
        public static bool IsLineVarSectionNameDefinition(string line) =>
         Labels.VarSectionNameLabels.All(line.Contains);

        public static bool IsLineDefaultValueDefinition(string line, string defaultNumericValue) =>
            !string.IsNullOrEmpty(defaultNumericValue) && line.Contains(Labels.VALUE_DEFINITION) && line.Contains(defaultNumericValue);

        public static string? ExtractOptionType(string line) =>
                                  line.Contains(Labels.NORMAL_OPTION_DEFINITION) ? Labels.NORMAL_OPTION_DEFINITION :
                                  line.Contains(Labels.NUMERIC_OPTION_DEFINITION) ? Labels.NUMERIC_OPTION_DEFINITION :
                                  line.Contains(Labels.CHECKBOX_OPTION_DEFINITION) ? Labels.CHECKBOX_OPTION_DEFINITION : null;

        public static string GetVarSizeValue(int index, string optionType) =>
           optionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION) ? "0x1"
           : "0x" + (Convert.ToInt32(BIOSFileParser.ExtractValueByLabel(index, Labels.SIZE_DEFINITION)) / 8).ToString("X");

        public static string ExtractDefaultIdValueIfExists(List<string> lines) => lines
         .Where(line => line.Contains(Labels.DEFAULTID))
         .Select(line => Regex.Match(line, @"Value:\s*(\S+)\s*$").Groups[1].Value.Replace("\"", string.Empty).Trim())
         .FirstOrDefault() ?? string.Empty;

        public static string ExtractStringOfLinesRelatedToAnOption(int startIndex)
        {
            var endIndex = BIOSFileParser.Lines.FindIndex(startIndex, x => x.Trim().Equals(Labels.END_OF_OPTION_SPACE));
            return string.Join("\n", BIOSFileParser.Lines[startIndex..endIndex]);
        }

        public static List<string> ExtractListOfLinesRelatedToAnOption(int startIndex)
        {
            var endIndex = BIOSFileParser.Lines.FindIndex(startIndex, x => x.Trim().Equals(Labels.END_OF_OPTION_SPACE));
            return BIOSFileParser.Lines[startIndex..endIndex];
        }
    }
}
