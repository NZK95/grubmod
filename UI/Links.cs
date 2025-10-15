using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void GrubmodLink(object o, RequestNavigateEventArgs r)
        {
            var url = "https://www.github.com/NZK95/grubmod/";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            Logger.Log("Redirected to grubmod GitHub repository.", LogType.SuccessfulOperation);
        }

        private void AuthorLink(object o, RequestNavigateEventArgs r)
        {
            var url = "https://www.github.com/NZK95";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            Logger.Log("Redirected to author GitHub profile.", LogType.SuccessfulOperation);
        }
    }
}
