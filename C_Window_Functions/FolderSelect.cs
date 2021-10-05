using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Point = System.Windows.Point;
using static Paradox_Editor.ProgramProperties;
using System.Windows;
using System.Windows.Controls;

namespace Paradox_Editor.C_Window_Functions
{
    public class FolderSelect
    {
        public ObservableCollection<ProvinceFile> FilePaths { get; set; } = new ObservableCollection<ProvinceFile>(); //Connected to the XAML
        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        private Canvas Canvas;
        private Image Image;

        public FolderSelect(Canvas canvas, Image image)
        {
            Image = image;
            Canvas = canvas;
        }

        [System.ComponentModel.Bindable(true)]

        public void Click_Specific_Entry(object sender, RoutedEventArgs e)
        {
            Process fileopener = new(); //Start a new process under the variable of fileopener
            fileopener.StartInfo.FileName = "explorer"; //Open the windows explorer/files; no other program
            fileopener.StartInfo.Arguments = SelectedItem.FilePath; //Open the file with respective filepath
            fileopener.Start(); //Open file
        }

        public void SelectMainFolder(object _, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new(); //Open new dialogue
            dialog.SelectedPath = StoredOpener; //Start directory of dialogue. Stored opener is the filepath the opener begins upon

            if (dialog.ShowDialog() is System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show("You selected Filepath: " + dialog.SelectedPath); //State the filepath selected
                Console.WriteLine(ProvinceDirectory);
                ProvinceDirectory = dialog.SelectedPath;
                StoredOpener = dialog.SelectedPath;
                Console.ReadLine();
            }
            else { }

            FilePaths.Clear(); //Clear all Rows as a "Refresh"

            try
            {
                var masterFolder = Directory.GetFiles(ProvinceDirectory, "*.txt", SearchOption.AllDirectories);
                var vic2ProvinceFilePath = Directory.GetFiles(Path.Combine(dialog.SelectedPath, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
                var vic2DefinitionCSVFile = Directory.GetFiles(Path.Combine(dialog.SelectedPath, "map"), "definition.csv", SearchOption.AllDirectories);
                List<string> CSVProvince = new List<string>();
                List<string[]> CSVRGB = new List<string[]>();
                List<string> CSVProvinceName = new List<string>();

                var CSVFile = vic2DefinitionCSVFile[0]; //Reads each entry from the filepath (the CSV File) as a part of an array
                using (var reader = new StreamReader(CSVFile)) //THIS FUNCTION GETS ALL PROVINCE CSV DATA (INCLUDING RGB)
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine(); //Current line of the CSV being read
                        var values = line.Split(';'); //Current values of said-line

                        CSVProvince.Add(values[0]); //Adding the province ID of current line to the Province ID List
                        string[] strArr = { values[1] + " " + " " + values[2] + " " + values[3] }; //Adding RGB codes
                        CSVProvinceName.Add(values[4]); //Adding province name of current line to Province Name List

                        CSVRGB.Add(strArr); //Adds the RGB Array Values from current line into the Province RGB List
                        var strings = CSVRGB[0].Cast<string>().ToArray(); //Sets array values as strings
                        Debug.WriteLine(strings); //Get RGB values for each pass
                    }
                } //May be overshadowed later by methods implimented in DrawMapColors

                ImageSourceConverter imgs = new ImageSourceConverter(); //Create instance of the image converter
                ScaleTransform flipTrans = new ScaleTransform(); //creates instance for scale
                Canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
                flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
                Canvas.RenderTransform = flipTrans; //Actually render the changes

                Image.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));

                foreach (string fileEntry in vic2ProvinceFilePath) //"For each file in the path list"
                {
                    string fileName = Path.GetFileName(fileEntry);
                    string[] SplitName = fileName.Split('-');
                    if (int.TryParse(SplitName[0], out int IDValue))
                    {
                        FilePaths.Add(new ProvinceFile() { ProvinceID = IDValue, ProvinceName = SplitName[1], FilePath = fileEntry }); //Add the respective province data into the FilePaths source
                    }
                    else
                    {
                        //Console.WriteLine("Problem File(s)" + " " + SplitName[0]);
                        Debug.WriteLine("Problem File(s)" + " " + SplitName[0]);
                    }
                }
                //Console.ReadLine();
            }
            catch
            {
                Debug.WriteLine("ERROR.FP = Filepath Issue");
                MessageBox.Show("Filepath unselected or invalid!" + "Check mod or game file selected.");
                return;
            }
        }

    }
}
