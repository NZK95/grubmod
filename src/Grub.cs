using System.Collections.ObjectModel;
using System.IO;

namespace grubmod
{
    internal class Grub
    {
        public static string BasePath { get; } = AppContext.BaseDirectory;
        public static string PathToMainFile { get; set; } = string.Empty;
        public static string PathToLogs { get; } = @$"{BasePath}logs-file({DateTime.Now:yyyy-MM-dd_HH-mm-ss}).txt";
        public static string PathToConfig { get; } = @$"{BasePath}builded-config.txt";
        public static string PathToScript { get; } = @$"{BasePath}setupvar-script.nsh";

        public static string EndOfScript => GetEndOfScript();
        public static ObservableCollection<Option> DefaultOptions { get; set; } = new ObservableCollection<Option>();
        public static ObservableCollection<Option> Options { get; set; } = DefaultOptions;

        public static IReadOnlyList<string> ReservedStrings { get; private set; } = new List<string>() { Labels.AUTHOR_WATERMARK, Labels.GRUBMOD_WATERMARK, Labels.SCRIPT_TEMPLATE + "\n" };
        public static List<string> LoggedChanges { get; private set; } = new List<string>();
        public static List<string> OptionValueStrings { get; private set; } = new List<string>();

        public static bool IsMatchCaseEnabled { get; set; } = false;

        public static void LogChanges(string varName, string varOffset, string hexvalue, string textValue, string varSize, string varSectionName)
        {
            var command = $"{Labels.SCRIPT_COMMAND_PREFIX} {varOffset} {hexvalue} -s {varSize} -n {varSectionName}";
            var comment = $"# {varName}({varOffset}) - {textValue}";
            var script = $"{comment}\n{command}";

            LoggedChanges.Add(script);
            OptionValueStrings.Add($"{varName} | {textValue}");
            Logger.Log("Changes are logged.", LogType.SuccessfulOperation);
        }

        private static string GetEndOfScript()
        {
            var firstOption = DefaultOptions.FirstOrDefault();
            return $"setup_var.efi {firstOption?.Fields?.VarOffset} -n {firstOption?.Fields?.VarSectionName} -r";
        }
    }
}
