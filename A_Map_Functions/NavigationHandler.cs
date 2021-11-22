using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using Image = System.Windows.Controls.Image;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Paradox_Editor
{
    public class MapNavigation
    {
        private Point start;
        private List<Image> Images = new List<Image>();
        private Canvas Canvas;

        public MapNavigation(Canvas canvas)
        {
            Canvas = canvas;
        }

        public MapNavigation AddImage(Image image)
        {
            Images.Add(image);
            return this;
        }

        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }
        public void MouseLeave(object sender, MouseEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void ReleaseMouseCapture()
        {
            Canvas.ReleaseMouseCapture();
            Canvas.Cursor = Cursors.Arrow;
        }

        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Canvas.IsMouseCaptured) return;
            Canvas.Cursor = Cursors.ScrollAll;
            start = e.MouseDevice.GetPosition(Canvas);
            Canvas.CaptureMouse();

            ///----------FUNKY CODE FOR COLOUR PICKING BELOW----------!
            ///
           //make writable bitmap
            var MainWindow = (MainWindow)Application.Current.MainWindow;

            var image = (Canvas)sender;
            var source = (BitmapSource)MainWindow.mapProvinces.Source;
            var mousePos = e.GetPosition(image);

            var pixelX = (int)(mousePos.X / image.ActualWidth * source.PixelWidth);
            var pixelY = (int)(mousePos.Y / image.ActualHeight * source.PixelHeight);



            var bitmap = BitmapFromSource(source);

            var pixelColor = bitmap.GetPixel(pixelX, pixelY);




            var newFunk = new Bitmap(BitmapFromSource(source));
            newFunk.SetPixel(pixelX, pixelY, System.Drawing.Color.Red);

            var sourcey = BitmapToImageSource(newFunk);
            MainWindow.mapProvinces.Source = sourcey;
             

            MainWindow.TestText0.Text = Convert.ToString(mousePos);
            MainWindow.TestText1.Text = Convert.ToString(pixelX + ", " + pixelY);
            MainWindow.TestText2.Text = Convert.ToString(pixelColor);


            Debug.WriteLine(Mouse.GetPosition(Mouse.DirectlyOver) + " Reading " + pixelColor);



        }



        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
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



        ///----------FUNKY CODE FOR COLOUR PICKING ABOVE---------!

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
                Point p = e.MouseDevice.GetPosition(image);
                var matrix = image.RenderTransform.Value;

                //make a translation matrix that matches the translation STATE of the image, apply current scaling
                //factor and apply scaling factor to the translation. Apply scale to current translation
                //m.gettranslation or something similar | bump down or so the translation state
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
                    if (e.Delta > 0)
                    {
                        matrix.Translate(0, -Math.Abs(e.Delta));
                    }
                    else
                    {
                        matrix.Translate(0, Math.Abs(e.Delta));
                    }

                    image.RenderTransform = new MatrixTransform(matrix);
                }
                else

                {
                    if (e.Delta > 0) //adjusting scaling factor
                    {
                        matrix.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);

                    }
                    //a translate may need to be included in order to get scale in order to match
                    //m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                    //the 1.1 values and hardcoded scale X&Y may need ot be changed at a later time

                    else
                    {
                        matrix.ScaleAtPrepend(0.9, 0.9, p.X, p.Y); //m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                    }

                    //ZBAGI#7539 is the best

                    image.RenderTransform = new MatrixTransform(matrix);
                }
            });
        }
    }
}
