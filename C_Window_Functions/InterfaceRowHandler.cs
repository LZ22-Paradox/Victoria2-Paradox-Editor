using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Paradox_Editor.C_Window_Functions
{
    public class InterfaceRowHandler
    {
        public MainWindow MainWindow { get; set; }
        public void RemoveInterfaceRow() //IS CAUSING ISSUES WITH DELETING ALL CORES
        {
            var rowCount = MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            if (rowCount > MainWindow.InterfaceActualRows + 1) //Requires changing every time the row count is altered.
            {
                MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                var heightChange = MainWindow.FileInterface.COREGRID.Height - 25;
                MainWindow.FileInterface.COREGRID.Height = heightChange;
                MainWindow.FileInterface.CoreGridRow.Height = new GridLength(heightChange);

            }
            else if (rowCount == MainWindow.InterfaceActualRows)
            {
                MainWindow.FileInterface.COREGRID.Height = 0;

            }
            else {
                MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                MainWindow.FileInterface.COREGRID.Height = 0;
            }
        }

        public void ResetListRows()
        {
            var rowCount = MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            for (int i = rowCount; i > MainWindow.InterfaceActualRows; i--)
            {
                MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.RemoveAt(rowCount - 1);
                rowCount = MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.Count;
            }
            MainWindow.FileInterface.COREGRID.Height = 0;
            MainWindow.FileInterface.CoreGridRow.Height = new GridLength(25);
            MainWindow.FileInterface.STATEBUILDING_GRID.Height = 0;
            MainWindow.FileInterface.StateBuildingGridRow.Height = new GridLength(44);
        }

        public void AddInterfaceRow()
        {
            var row = new RowDefinition();
            row.Height = new GridLength(25);
            MainWindow.FileInterface.ProvinceInterfaceViewerGrid.RowDefinitions.Add(row);
            var p = MainWindow.FileInterface.COREGRID.Height + 25;
            MainWindow.FileInterface.COREGRID.Height = p;
            MainWindow.FileInterface.CoreGridRow.Height = new GridLength(p);
        }

    }
}
