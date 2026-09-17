using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.ColorPickerControls;
using Paradox_Editor.DataAcquisition;
using Paradox_Editor.Handlers;

namespace Paradox_Editor.Interfaces;

[ToolboxItem(true)]
public partial class MapViewer
{
    private MainWindow MainWindow { get; set; } = (Application.Current.MainWindow as MainWindow)!;
    private Point start;

    public static Dictionary<MapMode, Image> MapModes { get; set; }
    public static bool IsMapLoaded;
    public static bool IsImageFlipped { get; set; } //Possibly may be useless

    WriteableBitmap ProvinceMap;

    
    public MapViewer()
    {
        InitializeComponent();
        DataContext = this;

        MapModes = new Dictionary<MapMode, Image>
        {
            { MapMode.POLITICAL, mapPolitical },
            { MapMode.PROVENCIAL, mapProvinces },
            { MapMode.TERRAIN, mapTerrain },
        };
        
    }

    public enum MapMode
    {
        POLITICAL,
        PROVENCIAL,
        TERRAIN
    }

    public static WriteableBitmap GetMap(MapMode mode)
        => BitmapFactory.ConvertToPbgra32Format((BitmapSource)MapModes[mode].Source);

    public static void SetMap(MapMode index, WriteableBitmap image) => MapModes[index].Source = image;

    public void LoadMaps()
    {
        //Load Province Map
        InvertCanvas(mapCanvas);
        var converter = new ImageSourceConverter(); //Create instance of the image converter

        ModInfoAcquisition instanceModData = ModData.Instance.MOD_DATA;
        var provincesFileName = Path.Combine("map", "provinces.bmp");
        string modProvincesFile = Path.Combine(instanceModData.GetModFolder(), provincesFileName);
        mapProvinces.SetValue(Image.SourceProperty,
            File.Exists(modProvincesFile)
                ? converter.ConvertFromString(modProvincesFile)
                // Quick n dirty fallback for map image to vanilla.
                : converter.ConvertFromString(Path.Combine(instanceModData.GetGameDirectory(), provincesFileName)));

        //Load Political Map
        WriteableBitmap provinceMapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapProvinces.Source);
        mapPolitical.Source = new MapRenderer().DrawPoliticalMap(provinceMapSource);

        ProvinceMap = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapProvinces.Source);
        
        //TODO: Load D_ Map

        IsMapLoaded = true;
    }

    #region Handling Mouse Inputs

    public void Map_MouseUp(object sender, MouseButtonEventArgs e)
    {
        mapCanvas.ReleaseMouseCapture();
        mapCanvas.Cursor = Cursors.Arrow;
    }

    public void Map_MouseLeave(object sender, MouseEventArgs e) => ReleaseMouseCapture();

    public new void MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (mapCanvas.IsMouseCaptured) return;
        mapCanvas.Cursor = Cursors.ScrollAll;
        start = e.MouseDevice.GetPosition(mapCanvas);
        mapCanvas.CaptureMouse();
    }

    public void Map_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton.Equals(MouseButtonState.Pressed))
            MouseDown(sender, e);
        else if (e.LeftButton.Equals(MouseButtonState.Pressed) && IsMapLoaded)
            MouseLeftClick(sender, e);
    }

    /// <summary>
    /// For alternate accesses to other maps.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void MouseLeftClick(object sender, MouseButtonEventArgs e)
    {
        switch (MainWindow.mapModeButtons.GetMapMode())
        {
            case MapMode.POLITICAL: //Political Map
                WriteableBitmap politicalMapSource =
                    BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapPolitical.Source);

                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Populate(mapPolitical, mapProvinces);
                }
                else
                {
                    SelectColor(politicalMapSource, mapPolitical.SelectedColor);
                    MainWindow.provinceInterface.Visibility = Visibility.Hidden;
                    Debug.WriteLine("COUNTRY EDITING NOT YET IMPLEMENTED"); //Implement opening of countries
                }

                break;
            case MapMode.PROVENCIAL: //Province Map
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    SelectColor(ProvinceMap, mapProvinces.SelectedColor);
                    Populate(mapProvinces, mapPolitical);
                    MainWindow.provinceInterface.Visibility = Visibility.Hidden;
                    Debug.WriteLine("COUNTRY EDITING NOT YET IMPLEMENTED"); //Implement opening of countries
                }
                else
                {
                    MainWindow.provinceInterface.PopulateInterface(mapProvinces.SelectedColor);
                    SelectColor(ProvinceMap, mapProvinces.SelectedColor);
                }

                break;
            case MapMode.TERRAIN: //Terrain Map
                break;
            default:
                throw new NotImplementedException("Map mode not implemented!");
        }


        SoundHandler.PlayClick();
    }

    private void Populate(ImageColorPicker image, ImageColorPicker colorImageSource)
    {
        Color pickedColor = image.PickColor(colorImageSource); //Split following code into perhaps its own method
        MainWindow.provinceInterface.PopulateInterface(pickedColor);
        SelectColor(BitmapFactory.ConvertToPbgra32Format((BitmapSource)colorImageSource.Source), pickedColor);
    }

    public void SelectColor(WriteableBitmap source, Color selectedColor)
    {
        WriteableBitmap singleProvince 
            = MapRenderer.DrawConnectedColors(source, MapRenderer.GetRawColor(selectedColor));
        flashingSelection.Source = singleProvince;
    }

    #region Map Navigation Controls

    public void Map_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        foreach (Image image in MapModes.Values)
        {
            Point p = e.MouseDevice.GetPosition(image);
            Matrix matrix = image.RenderTransform.Value;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Delta)
                {
                    case > 0:
                        matrix.Translate(Math.Abs(e.Delta), 0);
                        break;
                    default:
                        matrix.Translate(-Math.Abs(e.Delta), 0);
                        break;
                }

                image.RenderTransform = new MatrixTransform(matrix);
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (IsImageFlipped)
                {
                    switch (e.Delta)
                    {
                        case > 0:
                            matrix.Translate(0, -Math.Abs(e.Delta));
                            break;
                        default:
                            matrix.Translate(0, Math.Abs(e.Delta));
                            break;
                    }
                }
                else
                {
                    switch (e.Delta)
                    {
                        case > 0:
                            matrix.Translate(0, Math.Abs(e.Delta));
                            break;
                        default:
                            matrix.Translate(0, -Math.Abs(e.Delta));
                            break;
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

            flashingSelection.RenderTransform = image.RenderTransform;
        }
    }

    public void Map_MouseMove(object sender, MouseEventArgs e)
    {
        if (!mapCanvas.IsMouseCaptured) return;

        Point end = e.MouseDevice.GetPosition(mapCanvas);
        foreach (Image image in MapModes.Values)
        {
            Matrix m = image.RenderTransform.Value;
            m.OffsetX -= (start.X - end.X);
            m.OffsetY -= (start.Y - end.Y);
            image.RenderTransform = new MatrixTransform(m);
            flashingSelection.RenderTransform = image.RenderTransform;
        }

        start = e.MouseDevice.GetPosition(mapCanvas);
    }

    #endregion

    public void MoveTimerTick() //Add compatibility for alternate control mode
    {
        var velocity = /*(speed: pixels per second)*/ 2000 * /*(timer tick time in seconds)*/ 0.003;
        var flipCheck = 1;
        if (IsImageFlipped == true)
        {
            flipCheck = -1;
        }

        if (scrollViewer.IsFocused && MainWindow.ControlMode.SelectedItem.Equals(MainWindow.Mode_Modern))
        {
            foreach ((Image image, Matrix matrix) in from image in MapModes.Values
                     let matrix = image.RenderTransform.Value
                     select (image, matrix))
            {
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
                flashingSelection.RenderTransform = new MatrixTransform(matrix);
            }
        }
    }

    #endregion

    private static void InvertCanvas(Canvas canvas)
    {
        var flipTrans = new ScaleTransform(); //creates instance for scale
        canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
        flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
        canvas.RenderTransform = flipTrans; //Actually render the changes
        IsImageFlipped = true;
    }
}