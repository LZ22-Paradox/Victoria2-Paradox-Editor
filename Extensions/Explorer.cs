using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Paradox_Editor.Extensions
{
    public static class Explorer
    {
        public static string OpenFolderSelect()
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

        public static void OpenFile(string argument)
        {
            using Process fileopener = new();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + argument + "\"";
            fileopener.Start();
        }

    }
}
