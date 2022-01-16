using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;
using Image = System.Windows.Controls.Image;
using System.IO;
using Paradox_Editor.C_Window_Functions;
using Cursors = System.Windows.Input.Cursors;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Diagnostics;


namespace Paradox_Editor
{

    public class NavigationHandler
    {
        private Point start;
        private List<Image> Images = new List<Image>();
        private Canvas Canvas;
        public MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;


        public NavigationHandler(Canvas canvas)
        {
            Canvas = canvas;
        }

        public NavigationHandler AddImage(Image image)
        {
            Images.Add(image);
            return this;
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

        public void MoveTimerTick(object sender, EventArgs e) //Add compatibility for alternate control mode
        {
            //Debug.WriteLine(Keyboard.FocusedElement + " is Focused");
            double velocity = /*(speed: pixels per second)*/ 2000 * /*(timer tick time in seconds)*/ 0.003;
            var flipCheck = 1;
            if (MainWindow.IsImageFlipped == true)
            {
                flipCheck = -1;
            }

            if (MainWindow.scrollViewer.IsFocused && MainWindow.ControlMode.SelectedItem.Equals(MainWindow.Mode_Modern))
            {
                Images.ForEach(image =>
                {
                    var matrix = image.RenderTransform.Value;

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
                });

            }

        }

        public void MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;
            var source = (BitmapSource)MainWindow.mapProvinces.Source;  //make as WriteableBitmap
            var mousePos = e.GetPosition(image);
            var pixelX = (int)((mousePos.X / image.ActualWidth * source.PixelWidth) - 0.1);
            var pixelY = (int)((mousePos.Y / image.ActualHeight * source.PixelHeight) - 0.1);
            var bitmap = BitmapFromSource(source);
            var pixelColor = bitmap.GetPixel(pixelX, pixelY);

            var windowPos = e.GetPosition(MainWindow);
            MainWindow.FileInterface.Margin = new Thickness(windowPos.X - (MainWindow.FileInterface.Width / 2), windowPos.Y - (MainWindow.FileInterface.Height + 40), 0, 0);
            //Get positioning right. Also add animation?

            MainWindow.FileInterface.Visibility = Visibility.Visible;
            MainWindow.FileInterface.HorizontalAlignment = HorizontalAlignment.Left;

            var boxBinding = new HistoryfileInterface(MainWindow, pixelColor,
                MainWindow.SelectMap.StoredProvinceColorToID,
                MainWindow.SelectMap.StoredProvinceIDToDataDictionaries,
                MainWindow.SelectMap.StoredTagToCountryName);
            boxBinding.PutTAGDataIntoInferface();
            boxBinding.PutOtherDataIntoInferface();
            MainWindow.FileInterface.AddExistingCores(sender, e, MainWindow, pixelColor); //Clicking ocean bad
            MainWindow.FileInterface.Set_Save_Icon_To_Saved();
        }

        public Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {

            if (!Canvas.IsMouseCaptured) return;

            var end = e.MouseDevice.GetPosition(Canvas);

            Images.ForEach(image =>
            {
                var m = image.RenderTransform.Value;
                m.OffsetX = m.OffsetX - (start.X - end.X);
                m.OffsetY = m.OffsetY - (start.Y - end.Y);


                image.RenderTransform = new MatrixTransform(m);
            });
            start = e.MouseDevice.GetPosition(Canvas);

        }

        public void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Images.ForEach(image =>
            {
                var p = e.MouseDevice.GetPosition(image);
                var matrix = image.RenderTransform.Value;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (e.Delta > 0)
                    {
                        matrix.Translate(Math.Abs(e.Delta), 0);
                    }
                    else
                    {
                        matrix.Translate(-Math.Abs(e.Delta), 0);
                    }

                    image.RenderTransform = new MatrixTransform(matrix);
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (MainWindow.IsImageFlipped)
                    {
                        if (e.Delta > 0)
                        {
                            matrix.Translate(0, -Math.Abs(e.Delta));
                        }
                        else
                        {
                            matrix.Translate(0, Math.Abs(e.Delta));
                        }
                    }
                    else
                    {
                        if (e.Delta > 0)
                        {
                            matrix.Translate(0, Math.Abs(e.Delta));
                        }
                        else
                        {
                            matrix.Translate(0, -Math.Abs(e.Delta));
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

            });
        }
    }
}
