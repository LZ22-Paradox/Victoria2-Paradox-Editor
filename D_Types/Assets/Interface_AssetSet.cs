using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.D_Types.Asset_Types
{
    public class Interface_AssetSet
    {
        public BitmapImage Add_Icon { get; set; }
        public BitmapImage Remove_Icon { get; set; }
        public BitmapImage Reset_Icon { get; set; }
        public BitmapImage Exit_Icon { get; set; }
        public BitmapImage Interface_Background { get; set; }
        public Brush TextColor { get; set; }
        public Brush BoxColor { get; set; }

    }
}
