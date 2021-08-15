using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;

//F1 to see WIKI detail on part
//F12 to see mechanicla usage in VS
//CTRL +press+ K, D sorts all tabs

namespace Paradox_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class ProvinceFile
    {
        public int ID { get; set; }
        public string ProvinceName { get; set; }
        public string FilePath { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FlipTranslate = 1;
        }

        public ProvinceFile SelectedItem { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        public double FlipTranslate { get; set; } //Acquires the data under ProvinceFile; ID, provinceName, Filepath

        public ObservableCollection<ProvinceFile> FilePaths { get; set; } = new ObservableCollection<ProvinceFile>(); //Connected to the XAML

        [System.ComponentModel.Bindable(true)]

        private void SelectMasterFolder_Click(object _, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new(); //Open new dialogue
            dialog.SelectedPath = ProgramProperties.StoredOpener; //Start directory of dialogue. Stored opener is the filepath the opener begins upon

            if (dialog.ShowDialog() is System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show("You selected Filepath: " + dialog.SelectedPath); //State the filepath selected
                Console.WriteLine(ProgramProperties.ProvinceDirectory);
                ProgramProperties.ProvinceDirectory = dialog.SelectedPath;
                ProgramProperties.StoredOpener = dialog.SelectedPath;
                Console.ReadLine();
            }
            else { }

            FilePaths.Clear(); //Clear all Rows as a "Refresh"

            var masterFolder = Directory.GetFiles(ProgramProperties.ProvinceDirectory, "*.txt", SearchOption.AllDirectories);

            var vic2ProvinceFilePath = Directory.GetFiles(Path.Combine(dialog.SelectedPath, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            //string[] vic2MapFilePath = Directory.GetFiles(Path.Combine(dialog.SelectedPath, "map"), "*.*", SearchOption.TopDirectoryOnly);

            List<String> vic2MapFilePath = new(Directory.GetFiles(Path.Combine(dialog.SelectedPath, "map"), "*.*", SearchOption.TopDirectoryOnly));

            /*    //This is strictly code to find if a file is contained in the path
            bool inList = vic2MapFilePath.Contains(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp"));
            Console.WriteLine(inList);
            Debug.WriteLine(inList);
            */

            ImageSourceConverter imgs = new ImageSourceConverter(); //Create instance of the image converter
            ScaleTransform flipTrans = new ScaleTransform(); //creates instance for scale
            mapCanvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            FlipTranslate = flipTrans.ScaleY;
            mapCanvas.RenderTransform = flipTrans; //Actually render the changes



            mapBackground.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(dialog.SelectedPath, "map", "provinces.bmp")));

            foreach (string fileEntry in vic2ProvinceFilePath) //"For each file in the path list"
                                                               //Masterfolder temporary until a provincefolder can be acquired
            {
                string fileName = Path.GetFileName(fileEntry);
                string[] SplitName = fileName.Split('-');
                if (int.TryParse(SplitName[0], out int IDValue))
                {
                    FilePaths.Add(new ProvinceFile() { ID = IDValue, ProvinceName = SplitName[1], FilePath = fileEntry }); //Add the respective province data into the FilePaths source
                }
                else
                {
                    //Console.WriteLine("Problem File(s)" + " " + SplitName[0]);
                    //Debug.WriteLine("Problem File(s)" + " " + SplitName[0]);
                }


            }
            Console.ReadLine();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Process fileopener = new(); //Start a new process under the variable of fileopener
            fileopener.StartInfo.FileName = "explorer"; //Open the windows explorer/files; no other program
            fileopener.StartInfo.Arguments = SelectedItem.FilePath; //Open the file with respective filepath
            fileopener.Start(); //Open file
        }
        //public ICommand ClickMeCommand { get; set; } //Currently unused. Valuable in place of events.


        //To do later:
        /* - Path selection for map files
         * - Load Map
         * - Load Map ID Text file
         *      o Read ID's & equate to colors [or other way around]
         * ?Seperation of map modes; colors are displayed based on history files over what provinces have ownership tag
         * 
         * 
         */

    }
}
