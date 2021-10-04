using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using System.Collections.Generic;

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
            Canvas.ReleaseMouseCapture();
            Canvas.Cursor = Cursors.Arrow;
        }

        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Canvas.IsMouseCaptured) return;
            Canvas.Cursor = Cursors.ScrollAll;
            Canvas.CaptureMouse();
            start = e.MouseDevice.GetPosition(Canvas);
        }

        public void MouseMove(object sender, MouseEventArgs e) //THE MOUSE UP-DOWN MOVEMENT IS INVERTED WHEN CONVERTING MAPS (FlipTranslate is -1 when flipped)
        {
            if (!Canvas.IsMouseCaptured) return;

            var end = e.MouseDevice.GetPosition(Canvas);

            Images.ForEach(image =>
            {
                var m = image.RenderTransform.Value;
                m.OffsetX = m.OffsetX - (start.X - end.X);
                m.OffsetY = m.OffsetY - (start.Y - end.Y);

                start = e.MouseDevice.GetPosition(Canvas);
                image.RenderTransform = new MatrixTransform(m);
            });
        }

        public void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Images.ForEach(image =>
            {
                Point p = e.MouseDevice.GetPosition(image);
                var m = image.RenderTransform.Value;


                //make a translation matrix that matches the translation STATE of the image, apply current scaling
                //factor and apply scaling factor to the translation. Apply scale to current translation
                //m.gettranslation or something similar | bump down or so the translation state
                //
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (e.Delta > 0)
                    {
                        m.Translate(Math.Abs(e.Delta), 0);
                    }
                    else
                    {
                        m.Translate(-Math.Abs(e.Delta), 0);
                    }

                    image.RenderTransform = new MatrixTransform(m);
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e.Delta > 0)
                    {
                        m.Translate(0, -Math.Abs(e.Delta));
                    }
                    else
                    {
                        m.Translate(0, Math.Abs(e.Delta));
                    }

                    image.RenderTransform = new MatrixTransform(m);
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

                    //ZBAGI#7539 is the best

                    image.RenderTransform = new MatrixTransform(m);
                }
            });
        }
    }
}
