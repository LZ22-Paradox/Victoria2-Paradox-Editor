using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.D_Types
{
    public static class BitmapFromSource
    {
        public static Bitmap BmpFromSource(BitmapSource bitmapsource)
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
    }
}
