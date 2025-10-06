using System.Collections.ObjectModel;
using System.IO;

namespace grubmod
{
    internal class Grub
    {
        public static ObservableCollection<Option> DefaultOptions { get; set; } = BIOSFileParser.ExtractInformation().GetAwaiter().GetResult();
        public static ObservableCollection<Option> Options { get; set; } = DefaultOptions;

        public static readonly Dictionary<string, string> LoggedChanges = new Dictionary<string, string>();
        public static List<string> LoggedChangedValues { get; private set; } = new List<string>();
        public static bool IsMatchCaseEnabled { get; set; } = false;

        public static void LogChanges(string varName, string varOffset, string hexvalue, string varSize, string varSectionName)
        {
            const string WATERMARK = "# (https://www.github.com/NZK95)";
            const string SCRIPT_COMMAND_PREFIX = "setup_var.efi";

            var command = $"{SCRIPT_COMMAND_PREFIX} {varOffset} {hexvalue} -s {varSize} -n {varSectionName}";
            var script = $"# {varName}\n{command}\n";

            LoggedChangedValues.Add(script);
            File.AppendAllText(@"C:\Users\User\Desktop\123.txt", script);
        }
    }
}
