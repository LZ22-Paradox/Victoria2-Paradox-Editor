using Paradox_Editor.C_Window_Functions;
using Paradox_Editor.D_Class_Types;
using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paradox_Editor
{

    public partial class FileInterface : UserControl
    {
        public ObservableCollection<CoreData> CoreDataCollection { get; }

        private Point start;

        public FileInterface()
        {

            CoreDataCollection = new ObservableCollection<CoreData>()
            {
                new CoreData("FRA"),
                new CoreData("GER")
            };


            this.MouseLeftButtonDown += new MouseButtonEventHandler(LeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LeftButtonUp);
            this.MouseMove += new MouseEventHandler(Grid_MouseMove);
            InitializeComponent();
        }

        public void ExitClicked(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            GameSoundHandler.SoundHandler.PlayClickSound();
        }

        public void LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouse();
        }
        private void ReleaseMouse()
        {
            this.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }

        public void LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured) return;
            this.Cursor = Cursors.ScrollAll;
            start = e.MouseDevice.GetPosition(this);
            this.CaptureMouse();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsMouseCaptured) return;
            var end = e.MouseDevice.GetPosition(this);
            var m = this.RenderTransform.Value;
            m.OffsetX -= start.X - end.X;
            m.OffsetY -= start.Y - end.Y;
            this.RenderTransform = new MatrixTransform(m);
        }

        public void RemoveCoreRow(object sender, RoutedEventArgs e)
        {
            var selectedItem = COREGRID.SelectedItem;
            COREGRID.Items.Remove(selectedItem);


            Debug.WriteLine("Testremoved");
        }

        public void AddCoreRow(object sender, RoutedEventArgs e)
        {
            COREGRID.Items.Add("TESTADD"); //Doesn't work. See binding to Observeable.
            Debug.WriteLine("Testadded");
        }


        //


    }
}
