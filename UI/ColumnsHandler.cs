using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace grubmod
{
    public partial class MainWindow : Window
    {
        private void HideColumns()
        {
            var gridview = optionsListView.View as GridView;

            DescriptionColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("Description"));
            VarStoreIdColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("VarStoreId"));
            BIOSDefaultValueColumnValue = gridview.Columns.FirstOrDefault(x => x.Header.Equals("BIOS Default"));

            gridview.Columns.Remove(DescriptionColumnValue);
            gridview.Columns.Remove(VarStoreIdColumnValue);
            gridview.Columns.Remove(BIOSDefaultValueColumnValue);

            Logger.Log("Columns BIOS Default, Description, VarStoreId are hided",LogType.SuccessfulOperation);
        }
    }
}