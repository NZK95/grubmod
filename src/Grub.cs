using System.Collections.ObjectModel;
using System.IO;

namespace grubmod
{
    internal class Grub
    {
        public static string Path { get; set; } = @"C:\Users\User\Desktop\BIOS\files\Section_PE32_image_Setup_Setup.sct.0.0.en-US.uefi.ifr.txt";
        public static ObservableCollection<Option> DefaultOptions { get; set; } = BIOSFileParser.ExtractInformation().GetAwaiter().GetResult();
        public static ObservableCollection<Option> Options { get; set; } = DefaultOptions;

        public static IReadOnlyList<string> ReservedStrings { get; private set; } = new List<string>() { Labels.AUTHOR_WATERMARK, Labels.GRUBMOD_LINK_WATERMARK, Labels.SCRIPT_TEMPLATE + "\n" };
        public static List<string> LoggedChanges { get; private set; } = new List<string>();

        public static bool IsMatchCaseEnabled { get; set; } = false;

        public static void LogChanges(string varName, string varOffset, string hexvalue, string textValue, string varSize, string varSectionName)
        {
            var command = $"{Labels.SCRIPT_COMMAND_PREFIX} {varOffset} {hexvalue} -s {varSize} -n {varSectionName}";
            var script = $"# {varName} - {textValue}\n{command}\n";

            LoggedChanges.Add(script);
        }
    }
}
