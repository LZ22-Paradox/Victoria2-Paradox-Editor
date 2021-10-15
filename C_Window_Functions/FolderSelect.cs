using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Point = System.Windows.Point;
using static Paradox_Editor.D_Static_Variables.ProgramProperties;
using System.Windows;
using System.Windows.Controls;
using Paradox_Editor.B_Map_Functions;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Paradox_Editor.D_Static_Variables;

namespace Paradox_Editor.C_Window_Functions
{
    public class FolderSelect
    {
        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>(); //Connected to the XAML
        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        public string[] MasterFolder;
        public string[] Path_Provinces { get; set; }
        public static string[] Path_CSV { get; set; }
        public static string[] Path_CountriesTxt { get; set; }

        public static string[] Path_Countries { get; set; }

        private Canvas Canvas;
        private Image Image1;
        private Image Image2;


        public FolderSelect(string[] filepath1, Canvas canvas, Image image1)
        {
            Path_Provinces = filepath1;
            Image1 = image1;
            Canvas = canvas;
        }

        public FolderSelect(Canvas canvas, Image image1, Image image2)
        {
            Canvas = canvas;
            Image1 = image1;
            Image2 = image2;
        }

        public FolderSelect() { }

        public void Click_Specific_Entry(object sender, RoutedEventArgs e)
        {
            Process fileopener = new(); //Start a new process under the variable of fileopener
            fileopener.StartInfo.FileName = "explorer"; //Open the windows explorer/files; no other program
            fileopener.StartInfo.Arguments = SelectedItem.FilePath; //Open the file with respective filepath
            fileopener.Start(); //Open file
        }

        public void CollectDirectoryData(string CollectDirectory)
        {
            MasterFolder = Directory.GetFiles(ProvinceDirectory, "*.txt", SearchOption.AllDirectories);
            Path_Provinces = Directory.GetFiles(Path.Combine(ProvinceDirectory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            Path_CSV = Directory.GetFiles(Path.Combine(ProvinceDirectory, "map"), "definition.csv", SearchOption.AllDirectories);
            Path_CountriesTxt = Directory.GetFiles(Path.Combine(ProvinceDirectory, "common"), "countries.txt", SearchOption.AllDirectories);
            Path_Countries = Directory.GetFiles(Path.Combine(ProvinceDirectory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
            return;
        }

        public void SelectMainFolder(object _, EventArgs e)
        {
            var dialog = new FolderBrowserDialog(); //Open new dialogue
            dialog.SelectedPath = StoredOpener; //Start directory of dialogue. Stored opener is the filepath the opener begins upon

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("You selected Filepath: " + dialog.SelectedPath); //State the filepath selected
                Console.WriteLine(ProvinceDirectory);
                ProvinceDirectory = dialog.SelectedPath;
                StoredOpener = dialog.SelectedPath;
                Console.ReadLine();
            }

            ProvinceData.Clear(); //Clear all Rows as a "Refresh"

            //try
            //{
            CollectDirectoryData("CollectDirectory");

            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            var flipTrans = new ScaleTransform(); //creates instance for scale
            Canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            Canvas.RenderTransform = flipTrans; //Actually render the changes

            Image1.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));
            Image2 = Image1;

            var provinceDataCollection = new TextFileExtract(Path_Provinces, ProvinceData);
            provinceDataCollection.ExtractForCollection("Collection");

            MapEditor.UpdatePoliticalMap(); //Call Political Map Mode Update
            /* }
           catch (Exception exception)
            {
                Debug.WriteLine("ERROR.FP = Filepath Issue");
                System.Windows.MessageBox.Show(exception.Message + "There is an exception! Probably the filepath selected");
            }*/
        }
    }
}
