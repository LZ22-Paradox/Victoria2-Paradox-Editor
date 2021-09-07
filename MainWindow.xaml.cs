using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;
using static Paradox_Editor.ProgramProperties;
using System.Linq;

//F1 to see WIKI detail on part
//F12 to see mechanicla usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void MainWindow_Load(object _1, EventArgs _2)
        {
            MapEditor.TestProvinceUpdate();


        }

        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        //make combined dictionary of colour owned by what country
        //province colour


        public ObservableCollection<ProvinceFile> FilePaths { get; set; } = new ObservableCollection<ProvinceFile>(); //Connected to the XAML

        [System.ComponentModel.Bindable(true)]

        private void SelectMasterFolder_Click(object _, EventArgs e)
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
                        //Finding a way to print the entries of CSVRGB. Important thing is that RGB values & names are there
                        Debug.WriteLine(strings); //Get RGB values for each pass
                    }
                }

                /*    //This is strictly code to find if a file is contained in the path
                bool inList = vic2MapFilePath.Contains(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp"));
                Console.WriteLine(inList);
                Debug.WriteLine(inList);
                */

                ImageSourceConverter imgs = new ImageSourceConverter(); //Create instance of the image converter
                ScaleTransform flipTrans = new ScaleTransform(); //creates instance for scale
                mapCanvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
                flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
                mapCanvas.RenderTransform = flipTrans; //Actually render the changes

                mapBackground.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));

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
                        //Debug.WriteLine("Problem File(s)" + " " + SplitName[0]);
                    }
                }
                //Console.ReadLine();
            }
            catch
            {
                Debug.WriteLine("ERROR.FP = Filepath Issue");
                MessageBox.Show("Filepath unselected or invalid!");
                MessageBox.Show("Check mod or game file selected.");
                return;
            }
        }

        public static explicit operator MainWindow(WindowCollection v)
        {
            throw new NotImplementedException();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Process fileopener = new(); //Start a new process under the variable of fileopener
            fileopener.StartInfo.FileName = "explorer"; //Open the windows explorer/files; no other program
            fileopener.StartInfo.Arguments = SelectedItem.FilePath; //Open the file with respective filepath
            fileopener.Start(); //Open file
        }


    }
}
