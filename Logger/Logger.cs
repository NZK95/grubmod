using System.IO;

namespace grubmod
{
    internal enum LogType
    {
        Exception, Warning, Information, SuccessfulOperation, FailedOperation
    }

    internal record Log(DateTime dateTime, LogType logType, string message)
    {
        public override string ToString() =>
            Logger.PadString(dateTime.ToString(), logType.ToString(), message);
    }

    internal class Logger
    {
        private static readonly string _firstLineTemplate = PadString("| DateTime |", "| LogType |", "| Message |");

        public static string PadString(params string[] lines) =>
            string.Concat(lines.Select(line => line.PadRight(30)));

        public static void Log(string message, LogType logType)
        {
            if (!Helpers.IsFirstLineOfFileContainsTemplate(Grub.PathToLogs, _firstLineTemplate))
                InsertFileTemplate();

            File.AppendAllText(Grub.PathToLogs, new Log(DateTime.Now, logType, message).ToString() + "\n");
        }

        private static void InsertFileTemplate() =>
            File.WriteAllText(Grub.PathToLogs, _firstLineTemplate + "\n\n");
    }
}
