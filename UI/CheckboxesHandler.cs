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
        private void ShowDescription_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (!gridview.Columns.Contains(DescriptionColumnValue))
            {
                gridview.Columns.Insert(gridview.Columns.Count, DescriptionColumnValue);
                Logger.Log("Description column added.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to add column (Description), already exists.", LogType.FailedOperation);
        }

        private void ShowDescription_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(DescriptionColumnValue))
            {
                gridview.Columns.Remove(DescriptionColumnValue);
                Logger.Log("Description column removed.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to remove column (description), not exists yet.", LogType.FailedOperation);
        }

        private void ShowVarStoreId_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (!gridview.Columns.Contains(VarStoreIdColumnValue))
            {
                gridview.Columns.Insert(gridview.Columns.Count - (gridview.Columns.Count - 2), VarStoreIdColumnValue);
                Logger.Log("VarStoreId column added.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to add column (VarStoreId), already exists.", LogType.FailedOperation);
        }

        private void ShowVarStoreId_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(VarStoreIdColumnValue))
            {
                gridview.Columns.Remove(VarStoreIdColumnValue);
                Logger.Log("VarStoreId column removed.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to remove column (VarStoreId), not exists yet.", LogType.FailedOperation);
        }

        private void ShowBIOSDefaultValue_Checked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;
            var index = gridview.Columns.Contains(DescriptionColumnValue) ? gridview.Columns.Count - 1 :
                gridview.Columns.Count;

            if (!gridview.Columns.Contains(BIOSDefaultValueColumnValue))
            {
                gridview.Columns.Insert(index, BIOSDefaultValueColumnValue);
                Logger.Log("BIOS Default Value column added.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to add column (BIOS Default Value), already exists.", LogType.FailedOperation);
        }

        private void ShowBIOSDefaultValue_Unchecked(object sender, RoutedEventArgs e)
        {
            var gridview = optionsListView.View as GridView;

            if (gridview.Columns.Contains(BIOSDefaultValueColumnValue))
            {
                gridview.Columns.Remove(BIOSDefaultValueColumnValue);
                Logger.Log("BIOS Default Value column removed.", LogType.SuccessfulOperation);
                return;
            }

            Logger.Log("Impossible to remove column (BIOS Default Value), not exists yet.", LogType.FailedOperation);
        }

        private void ShowAllOptions_Checked(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options;
            Logger.Log("All types of options are showed.", LogType.SuccessfulOperation);
        }

        private void ShowAllNormalOptions_Checked(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.NORMAL_OPTION_DEFINITION));
            Logger.Log("All normal options are showed.", LogType.SuccessfulOperation);
        }

        private void ShowAllCheckBoxOptions_Checked(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.CHECKBOX_OPTION_DEFINITION));
            Logger.Log("All check-box options are showed.", LogType.SuccessfulOperation);
        }

        private void ShowAllNumericOptions_Checked(object sender, RoutedEventArgs e)
        {
            optionsListView.ItemsSource = Grub.Options.Where(x => x.Fields.OptionType.Equals(Labels.NUMERIC_OPTION_DEFINITION));
            Logger.Log("All numeric options are showed.", LogType.SuccessfulOperation);

        }
        private void MatchCase_Checked(object sender, RoutedEventArgs e)
        {
            Grub.IsMatchCaseEnabled = true;
            Logger.Log("Match case - Enabled", LogType.SuccessfulOperation);
        }

        private void MatchCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grub.IsMatchCaseEnabled = false;
            Logger.Log("Match case - Disabled", LogType.SuccessfulOperation);
        }
    }
}
