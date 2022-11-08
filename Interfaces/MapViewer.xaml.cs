using ColorPickerControls;
using Paradox_Editor.Data_Handling;
using Paradox_Editor.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor
{
    [ToolboxItem(true)]
    public partial class MapViewer : UserControl
    {
        private MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;
        private Point start;

        public static Dictionary<int, Image> MapModes { get; set; }
        public static bool IsMapLoaded;
        public static bool IsImageFlipped { get; set; } //Possibly may be useless

        public MapViewer()
        {
            InitializeComponent();
            DataContext = this;

            MapModes = new Dictionary<int, Image>
            {
                { 0, mapPolitical},
                { 1, mapProvinces },
                { 2, mapTerrain },
            };
        }

        public static WriteableBitmap GetMap(int index)
        {
            var mapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)MapModes[index].Source);
            return mapSource;
        }

        public static void SetMap(int index, WriteableBitmap image)
        {
            MapModes[index].Source = image;
        }

        public void LoadMaps()
        {
            //Load Province Map
            InvertCanvas(mapCanvas);
            var imgs = new ImageSourceConverter(); //Create instance of the image converter
            if (File.Exists(Path.Combine(ModData.IO_DATA.GetModFolder(), "map", "provinces.bmp")))
            {
                mapProvinces.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(ModData.IO_DATA.GetModFolder(), "map", "provinces.bmp")));
            } else
            {
                mapProvinces.SetValue(Image.SourceProperty, imgs.ConvertFromString(Path.Combine(ModData.IO_DATA.GetGameDirectory(), "map", "provinces.bmp")));
            }

            //Load Political Map
            var provinceMapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapProvinces.Source);
            mapPolitical.Source = new MapRenderer(ModData.PROVINCE_DATA).DrawPoliticalMap(provinceMapSource);

            //Load D_ Map


            //Load D_ Map

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
            {
                MouseDown(sender, e);
            }
            else if (e.LeftButton.Equals(MouseButtonState.Pressed) && IsMapLoaded)
            {
                MouseLeftClick(sender, e);
            }
        }

        /// <summary>
        /// For alternate accesses to other maps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            _ = MapModes.TryGetValue(MainWindow.mapModeButtons.GetMapMode(), out Image mapMode);
            //var mapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapMode.Source); //Map source maybe used for later

            var politicalMapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapPolitical.Source);
            var provinceMapSource = BitmapFactory.ConvertToPbgra32Format((BitmapSource)mapProvinces.Source);

            switch (MainWindow.mapModeButtons.GetMapMode())
            {
                case 0: //Political Map
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        Populate(mapPolitical, mapProvinces);
                    }
                    else
                    {
                        SelectColor(politicalMapSource, mapPolitical.SelectedColor);
                        MainWindow.provinceInterface.Visibility = Visibility.Hidden;
                        MessageBox.Show("COUNTRY EDITING NOT YET IMPLEMENTED"); //Implement opening of countries
                    }
                    break;
                case 1: //Province Map
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        Populate(mapProvinces, mapPolitical);
                        MainWindow.provinceInterface.Visibility = Visibility.Hidden;
                        MessageBox.Show("COUNTRY EDITING NOT YET IMPLEMENTED"); //Implement opening of countries
                    }
                    else
                    {
                        MainWindow.provinceInterface.PopulateInterface(mapProvinces.SelectedColor);
                        SelectColor(provinceMapSource, mapProvinces.SelectedColor);
                    }
                    
                    break;
                case 2: //Terrain Map
                    break;
            }


            SoundHandler.PlayClick();
        }

        void Populate(ImageColorPicker image, ImageColorPicker colorImageSource)
        {
            var pickedColor = image.PickColor(colorImageSource); //Split following code into perhaps its own method
            MainWindow.provinceInterface.PopulateInterface(pickedColor);
            SelectColor(BitmapFactory.ConvertToPbgra32Format((BitmapSource)colorImageSource.Source), pickedColor);
        }

        public void SelectColor(WriteableBitmap source, Color selectedColor)
        {
            var tempMapRenderer = new MapRenderer();
            var singleProvince = tempMapRenderer.DrawSelectedProvince(source,
                tempMapRenderer.GetRawColor(selectedColor));
            flashingSelection.Source = singleProvince;
        }

        public void Map_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            foreach (var image in MapModes.Values)
            {
                var p = e.MouseDevice.GetPosition(image);
                var matrix = image.RenderTransform.Value;

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

            var end = e.MouseDevice.GetPosition(mapCanvas);
            foreach (var image in MapModes.Values)
            {
                var m = image.RenderTransform.Value;
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
                foreach (var (image, matrix) in from image in MapModes.Values let matrix = image.RenderTransform.Value select (image, matrix))
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

        public void InvertCanvas(Canvas canvas)
        {
            var flipTrans = new ScaleTransform(); //creates instance for scale
            canvas.RenderTransformOrigin = new Point(0.5, 0.5); //Sets the origin/middle point of the new image
            flipTrans.ScaleY = -1; //flip the scale of the Y (horizontal) so it is the right side up
            canvas.RenderTransform = flipTrans; //Actually render the changes
            IsImageFlipped = true;
        }
    }
}
