using System;
using System.Windows.Forms;

namespace Paradox_Editor.C_Window_Functions
{
    public class Explorer
    {
        public string OpenFileSelect()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            MessageBox.Show("You selected Filepath: " + dialog.SelectedPath);
            Console.WriteLine(dialog.SelectedPath);
            var selectDirectory = dialog.SelectedPath;

            var storedOpener = dialog.SelectedPath; //Unused; Reimpliment stored opener.

            Console.ReadLine();
            MainWindow.ProvinceData.Clear();
            return selectDirectory;
        }

    }
}
