using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Windows.Controls;
using Paradox_Editor.B_Map_Functions;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Paradox_Editor.D__Static_Classes_Types;
using Paradox_Editor.D_Static_Classes_Types;

namespace Paradox_Editor.C_Window_Functions
{
    public class FolderSelect
    {

        //public string StoredOpener { get; set; } = Path.Combine("E:", "Games", "Victoria II", "mod", "LZ22");
        //Stored opener will be subject to change for user convienence

        public static ObservableCollection<ProvinceFile> ProvinceData { get; set; } = new ObservableCollection<ProvinceFile>(); //Connected to the XAML
        private Canvas Canvas;
        private Image Image1;
        private Image Image2;

        public FolderSelect(Canvas canvas, Image image1, Image image2)
        {
            Canvas = canvas;
            Image1 = image1;
            Image2 = image2;
        }

        public FolderSelect() { }



        public void SelectMainFolder(object _, EventArgs e)
        {
            var dialog = new FolderBrowserDialog(); //Open new dialogue
            //dialog.SelectedPath = storedOpener;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            MessageBox.Show("You selected Filepath: " + dialog.SelectedPath); //State the filepath selected
            Console.WriteLine(dialog.SelectedPath);
            var selectDirectory = dialog.SelectedPath;
            var storedOpener = dialog.SelectedPath; //Unused; Reimpliment stored opener.
            Console.ReadLine();
            ProvinceData.Clear(); //Clear all Rows as a "Refresh"


            var mapEditorInstance = new MapEditor();
            var directoryData = mapEditorInstance.CollectDirectoryData(selectDirectory);

            var provinceTagToColor = mapEditorInstance.GetProvinceColorToID(directoryData.DefinitionCSV);
            var tagToCountryName = mapEditorInstance.GetTagToCountryName(directoryData.CountriesTxt);

            var supacollection_wip = mapEditorInstance.GetProvinceIDToData(directoryData.HistoryProvinces);

            //SORT REST OF METHODS FROM HERE

            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            var flipTrans = new ScaleTransform(); //creates instance for scale
            Canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            Canvas.RenderTransform = flipTrans; //Actually render the changes

            Image1.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));
            Image2 = Image1;
            
            var provinceDataCollection = new TextFileExtractor(directoryData.HistoryProvinces, ProvinceData);
            provinceDataCollection.ExtractForCollection();


            //update.UpdatePoliticalMap();

        }
    }
}
