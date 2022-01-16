using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Paradox_Editor.C_Window_Functions
{
    public class InterfaceRowHandler
    {
        public MainWindow MainWindow { get; set; }
        public int HistoryInterfaceRowCount { get; set; }
        public void RemoveRowFromCoreList()
        {
            var rowCount = MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.Count;
            //Could impliment OldRowCount variable to make it more fluid
            if (rowCount > MainWindow.DefaultHistoryInterfaceRows) //Requires changing every time the row count is altered.
            {
                MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                var heightChange = MainWindow.FileInterface.COREGRID.Height - 25;
                MainWindow.FileInterface.COREGRID.Height = heightChange;
                MainWindow.FileInterface.CoreGridRow.Height = new GridLength(heightChange);
            }
            else
            {
                MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                MainWindow.FileInterface.COREGRID.Height = 0;
            }
        }

        public void ResetRowsFromCoreList()
        {
            var rowCount = MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.Count;
            for (int i = rowCount; i > MainWindow.DefaultHistoryInterfaceRows - 1; i--) //The "8" will require changing whenever new rows are added or removed
            {
                MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                rowCount = MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.Count;
            }
            MainWindow.FileInterface.COREGRID.Height = 0;
            MainWindow.FileInterface.CoreGridRow.Height = new GridLength(25);
        }

        public void AddRowFromCoreList()
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(25);
            MainWindow.FileInterface.FileInterfaceViewerGrid.RowDefinitions.Add(row);
            var p = MainWindow.FileInterface.COREGRID.Height + 25;
            MainWindow.FileInterface.COREGRID.Height = p;
            MainWindow.FileInterface.CoreGridRow.Height = new GridLength(p);
        }




    }
}
