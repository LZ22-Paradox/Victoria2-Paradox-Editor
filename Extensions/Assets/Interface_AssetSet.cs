using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.Extensions.Assets;

public class Interface_AssetSet
{
    public BitmapImage AddIcon { get; set; }
    public BitmapImage RemoveIcon { get; set; }
    public BitmapImage ResetIcon { get; set; }
    public BitmapImage ExitIcon { get; set; }
    public BitmapImage InterfaceBackground { get; set; }
    public Brush TextColor { get; set; }
    public Brush BoxColor { get; set; }

}