using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.Extensions.Assets;

public class Interface_AssetSet
{
    public BitmapImage AddIcon { get; set; } = null!;
    public BitmapImage RemoveIcon { get; set; } = null!;
    public BitmapImage ResetIcon { get; set; } = null!;
    public BitmapImage ExitIcon { get; set; } = null!;
    public BitmapImage InterfaceBackground { get; set; } = null!;
    public Brush TextColor { get; set; } = null!;
    public Brush BoxColor { get; set; } = null!;

}