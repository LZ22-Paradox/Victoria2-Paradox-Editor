using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Collections.Generic;
using Image = System.Windows.Controls.Image;
using Cursors = System.Windows.Input.Cursors;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Paradox_Editor.D_Types;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Diagnostics;
using System.Linq;

namespace Paradox_Editor.C_Window_Functions
{

    public class NavigationHandler
    {
        private Point start;
        private Canvas Canvas;
        private Dictionary<int, Image> Maps;

        public MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;


        public NavigationHandler(Canvas canvas)
        {
            Canvas = canvas;
        }

        public NavigationHandler(Canvas canvas, Dictionary<int, Image> maps)
        {
            Canvas = canvas;
            Maps = maps;
        }

        public void MouseLeave(object sender, MouseEventArgs e)
        {
            ReleaseMouseCapture();
        }

        public void ReleaseMouseCapture()
        {
            Canvas.ReleaseMouseCapture();
            Canvas.Cursor = Cursors.Arrow;
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Canvas.IsMouseCaptured) return;
            Canvas.Cursor = Cursors.ScrollAll;
            start = e.MouseDevice.GetPosition(Canvas);
            Canvas.CaptureMouse();
        }

        public void MoveTimerTick() //Add compatibility for alternate control mode
        {
            var velocity = /*(speed: pixels per second)*/ 2000 * /*(timer tick time in seconds)*/ 0.003;
            var flipCheck = 1;
            if (MainWindow.IsImageFlipped == true)
            {
                flipCheck = -1;
            }

            if (MainWindow.scrollViewer.IsFocused && MainWindow.ControlMode.SelectedItem.Equals(MainWindow.Mode_Modern))
            {
                foreach (var (image, matrix) in from image in Maps.Values let matrix = image.RenderTransform.Value select (image, matrix))
                {
                    if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up)) //UP
                    {
                        matrix.Translate(0, flipCheck * Math.Abs(velocity));
                    }

                    if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left)) //LEFT
                    {
                        matrix.Translate(Math.Abs(velocity), 0);
                    }

                    if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down)) //DOWN
                    {
                        matrix.Translate(0, -flipCheck * Math.Abs(velocity));
                    }

                    if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right)) //RIGHT
                    {
                        matrix.Translate(-Math.Abs(velocity), 0);
                    }

                    image.RenderTransform = new MatrixTransform(matrix);
                }
            }

        }

        public void MouseLeftClick(object sender, MouseButtonEventArgs e) //Fix up
        {
            _ = Maps.TryGetValue(MainWindow.mapModeButtons.GetMapMode(), out Image mapMode);
            var mapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapMode.Source);
            var image = (Image)sender;
            var mousePos = e.GetPosition(image);
            var pixelX = (int)(mousePos.X / image.ActualWidth * mapSource.PixelWidth - 0.1);
            var pixelY = (int)(mousePos.Y / image.ActualHeight * mapSource.PixelHeight - 0.1);
            var pixelColor = mapSource.GetPixel(pixelX, pixelY);

            Debug.WriteLine(pixelColor + " : Pixel Colour, which is " + pixelColor.ToString());

            ///!!!Bad Code Beware!!!!!
            var windowPos = e.GetPosition(MainWindow);
            //MainWindow.FileInterface.Margin = new Thickness(windowPos.X - (MainWindow.FileInterface.Width / 2), windowPos.Y - (MainWindow.FileInterface.Height + 40), 0, 0);
            //Get positioning right. Also add animation?


            //MainWindow.FileInterface.Visibility = Visibility.Visible;

            /*var boxBinding = new HistoryfileInterface(MainWindow, pixelColor,
                MainWindow.SelectMap.StoredProvinceColorToID,
                MainWindow.SelectMap.StoredProvinceIDToDataDictionaries,
                MainWindow.SelectMap.StoredTagToCountryName);

            boxBinding.PutTAGDataIntoInferface();
            boxBinding.PutOtherDataIntoInferface();
            MainWindow.FileInterface.AddCoresAndBuildings(sender, e, MainWindow, pixelColor); //Clicking ocean bad
            MainWindow.FileInterface.Set_Save_Icon_To_Saved();
            */
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {

            if (!Canvas.IsMouseCaptured) return;

            var end = e.MouseDevice.GetPosition(Canvas);
            foreach (var image in Maps.Values)
            {
                var m = image.RenderTransform.Value;
                m.OffsetX = m.OffsetX - (start.X - end.X);
                m.OffsetY = m.OffsetY - (start.Y - end.Y);
                image.RenderTransform = new MatrixTransform(m);
            }

            start = e.MouseDevice.GetPosition(Canvas);
        }

        public void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            foreach (var image in Maps.Values)
            {
                var p = e.MouseDevice.GetPosition(image);
                var matrix = image.RenderTransform.Value;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    switch (e.Delta)
                    {
                        case > 0:
                            matrix.Translate(Math.Abs(e.Delta), 0);
                            break;
                        default:
                            matrix.Translate(-Math.Abs(e.Delta), 0);
                            break;
                    }

                    image.RenderTransform = new MatrixTransform(matrix);
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (MainWindow.IsImageFlipped)
                    {
                        switch (e.Delta)
                        {
                            case > 0:
                                matrix.Translate(0, -Math.Abs(e.Delta));
                                break;
                            default:
                                matrix.Translate(0, Math.Abs(e.Delta));
                                break;
                        }
                    }
                    else
                    {
                        switch (e.Delta)
                        {
                            case > 0:
                                matrix.Translate(0, Math.Abs(e.Delta));
                                break;
                            default:
                                matrix.Translate(0, -Math.Abs(e.Delta));
                                break;
                        }
                    }
                    image.RenderTransform = new MatrixTransform(matrix);
                }
                else
                {
                    if (e.Delta > 0) //adjusting scaling factor
                    {
                        matrix.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                    }
                    else
                    {
                        matrix.ScaleAtPrepend(0.9, 0.9, p.X, p.Y); //m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                    }
                    image.RenderTransform = new MatrixTransform(matrix);
                }
            }
        }

    }
}
