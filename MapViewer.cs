using Paradox_Editor;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Paradox_Editor
{

    public partial class MainWindow : Window
    {
        private Point start;

        public void MapViewer()
        {
            InitializeComponent();
        }

        private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mapBackground.IsMouseCaptured) return;
            mapBackground.Cursor = Cursors.ScrollAll;
            mapBackground.CaptureMouse();
            start = e.MouseDevice.GetPosition(mapCanvas);
        }

        private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mapBackground.ReleaseMouseCapture();
            mapBackground.Cursor = Cursors.Arrow;
        }

        private void map_MouseMove(object sender, MouseEventArgs e) //THE MOUSE UP-DOWN MOVEMENT IS INVERTED WHEN CONVERTING MAPS (FlipTranslate is -1 when flipped)
        {
            if (!mapBackground.IsMouseCaptured) return;

            var end = e.MouseDevice.GetPosition(mapCanvas);
            var m = mapBackground.RenderTransform.Value;

            m.OffsetX = m.OffsetX - (start.X - end.X);
            m.OffsetY = m.OffsetY - (start.Y - end.Y);

            start = e.MouseDevice.GetPosition(mapCanvas);

            mapBackground.RenderTransform = new MatrixTransform(m);
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            Point p = e.MouseDevice.GetPosition(mapBackground);
            var m = mapBackground.RenderTransform.Value;

            //make a translation matrix that matches the translation STATE of the image, apply current scaling
            //factor and apply scaling factor to the translation. Apply scale to current translation
            //m.gettranslation or something similar | bump down or so the translation state

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Delta > 0)
                    m.Translate(Math.Abs(e.Delta), 0);
                else
                    m.Translate(-Math.Abs(e.Delta), 0);
                mapBackground.RenderTransform = new MatrixTransform(m);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    m.Translate(0, Math.Abs(e.Delta));
                else
                    m.Translate(0, -Math.Abs(e.Delta));
                mapBackground.RenderTransform = new MatrixTransform(m);
            }
            else

            {
                if (e.Delta > 0) //adjusting scaling factor
                {
                    m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                }
                //a translate may need to be included in order to get scale in order to match
                //m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                //the 1.1 values and hardcoded scale X&Y may need ot be changed at a later time

                else
                {
                    m.ScaleAtPrepend(0.9, 0.9, p.X, p.Y); //m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                }

                //ZacharyPatten#7432
                //ZBAGI#7539
                mapBackground.RenderTransform = new MatrixTransform(m);

            }
        }



        //The following buttons are events for the top two zoom in and out buttons. They aren't worth much, but should be replaced with key commands.
        private void btnZoomin_Click(object sender, RoutedEventArgs e)
        {
            mapBackground.Height *= 1.1;
            mapBackground.Width *= 1.1;
        }

        private void btnZoomout_Click(object sender, RoutedEventArgs e)
        {
            mapBackground.Height /= 1.1;
            mapBackground.Width /= 1.1;
        }

    }


}
