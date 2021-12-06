using Paradox_Editor.C_Window_Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paradox_Editor.A_Map_Functions
{
    public class ImageTransformation
    {
        public Canvas CanvasTransform { get; set; }

        public ImageTransformation(Canvas canvas)
        {
            CanvasTransform = canvas;
        }

        public void InvertCanvas(Canvas CanvasTransform)
        {
            var flipTrans = new ScaleTransform(); //creates instance for scale
            CanvasTransform.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            CanvasTransform.RenderTransform = flipTrans; //Actually render the changes
            MainWindow.IsImageFlipped = true;
        }
    }
}
