using Paradox_Editor.C_Window_Functions;
using System;
using System.Collections.Generic;
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
        private Point start;

        public FileInterface()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(LeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LeftButtonUp);
            this.MouseMove += new MouseEventHandler(Grid_MouseMove);
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


        //


    }
}
