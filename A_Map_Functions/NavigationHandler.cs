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
using Control = System.Windows.Forms.Control;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Paradox_Editor
{
    public class NavigationHandler
    {
        private Point start;
        private List<Image> Images = new List<Image>();
        private Canvas Canvas;

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

        private bool nonNumberEntered = false;

        public void KeyPressed(object sender, KeyEventArgs e) ///WORKING ON KEYBOARD CONTROLS || NOT WORKING
        {
            nonNumberEntered = false;
            if (e.Key is < Key.D0 or > Key.D9)
            {
                if (e.Key is < Key.NumPad0 or > Key.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.Key != Key.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                nonNumberEntered = true;
            }

            if (nonNumberEntered == true)
            {
                e.Handled = true;
            }
        }

        public void MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            var MainWindow = (MainWindow)Application.Current.MainWindow;
            var image = (Image)sender;
            var source = (BitmapSource)MainWindow.mapProvinces.Source;  //make as WriteableBitmap
            var mousePos = e.GetPosition(image);
            var pixelX = (int)((mousePos.X / image.ActualWidth * source.PixelWidth) - 0.1);
            var pixelY = (int)((mousePos.Y / image.ActualHeight * source.PixelHeight) - 0.1);
            var bitmap = BitmapFromSource(source);
            var pixelColor = bitmap.GetPixel(pixelX, pixelY);

            if (FolderSelect.IsMapLoaded)
            {
                var windowPos = e.GetPosition(MainWindow);
                MainWindow.FileInterface.Margin = new Thickness(windowPos.X - (MainWindow.FileInterface.Width / 2), windowPos.Y - (MainWindow.FileInterface.Height + 40), 0, 0);
                //Get positioning right. Also add animation?

                MainWindow.FileInterface.Visibility = Visibility.Visible;
                MainWindow.FileInterface.HorizontalAlignment = HorizontalAlignment.Left;

                MainWindow.FileInterface.COLORRGB.Text = Convert.ToString(pixelColor);

                var fart = MainWindow.SelectMap.StoredGameDirectory;
                var dunt = MainWindow.SelectMap.StoredProvinceColorToID;

                var poop = MainWindow.ProvinceData;
                var cipple = MainWindow.SelectMap.StoredTagToCountryName;
                //MAKE NEW INSTANCE OF CLASS OR USE OLD CLASS FOR RECOVERING DATA FROM THIS INFORMATION?

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

                    image.RenderTransform = new MatrixTransform(matrix);
                }
            });
        }
    }
}
