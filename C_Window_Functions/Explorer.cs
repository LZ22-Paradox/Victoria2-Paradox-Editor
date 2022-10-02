using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Paradox_Editor.C_Window_Functions
{
    public class Explorer
    {
        public string OpenFolderSelect()
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
            return selectDirectory;
        }

        public void OpenFile(string argument)
        {
            var fileOpener = new Process();
            fileOpener.StartInfo.FileName = "explorer";
            fileOpener.StartInfo.Arguments = argument;
            fileOpener.Start();
        }

    }
}
