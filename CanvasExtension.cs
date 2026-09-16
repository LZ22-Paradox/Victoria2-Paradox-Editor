using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paradox_Editor;

public static class CanvasExtension
{
    public static Canvas InvertCanvas(this Canvas canvas)
    {
        var flipTrans = new ScaleTransform(); //creates instance for scale
        canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
        flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
        canvas.RenderTransform = flipTrans; //Actually render the changes
        return canvas;
    }
}