using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paradox_Editor.DataAcquisition;
using Paradox_Editor.Handlers;

namespace Paradox_Editor.Interfaces;

[ToolboxItem(true)]
public partial class MapViewer
{
    private readonly MainWindow _mainWindow = (Application.Current.MainWindow as MainWindow)!;

    private Point _dragStart;

    private WriteableBitmap _provinceMap = null!;

    public static Dictionary<MapMode, Image> MapModes { get; private set; } = [];

    public static bool IsMapLoaded { get; private set; }

    public static bool IsImageFlipped { get; private set; }

    public MapViewer()
    {
        InitializeComponent();
        DataContext = this;

        MapModes = new Dictionary<MapMode, Image>
        {
            [MapMode.Political] = mapPolitical,
            [MapMode.Provincial] = mapProvinces,
            [MapMode.Terrain] = mapTerrain
        };
    }

    public enum MapMode
    {
        Political,
        Provincial,
        Terrain
    }

    public static WriteableBitmap GetMap(MapMode mode)
    {
        return BitmapFactory.ConvertToPbgra32Format(
            (BitmapSource)MapModes[mode].Source);
    }

    public static void SetMap(MapMode mode, WriteableBitmap image)
    {
        MapModes[mode].Source = image;
    }

    public void LoadMaps()
    {
        InvertCanvas(mapCanvas);

        ModInfoAcquisition modData = ModData.Instance.MOD_DATA;
        var provinceMapPath = Path.Combine("map", "provinces.bmp");

        var modPath = Path.Combine(
            modData.GetModFolder(),
            provinceMapPath);

        var vanillaPath = Path.Combine(
            modData.GetGameDirectory(),
            provinceMapPath);

        var path = File.Exists(modPath) ? modPath : vanillaPath;

        // Load the original province map.
        mapProvinces.Source = new BitmapImage(new Uri(Path.GetFullPath(path), UriKind.Absolute));

        // Keep a private copy for province lookups/selections.
        _provinceMap = GetMap(MapMode.Provincial);

        // Generate the visible political map from the province map.
        mapPolitical.Source =
            new MapRenderer().DrawPoliticalMap(_provinceMap);

        IsMapLoaded = true;
    }

    #region Mouse Input

    public void Map_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            BeginPan(e);
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed && IsMapLoaded)
            HandleLeftClick();
    }

    public void Map_MouseUp(object sender, MouseButtonEventArgs e)
    {
        ReleaseMouseCapture();
    }

    public void Map_MouseLeave(object sender, MouseEventArgs e)
    {
        ReleaseMouseCapture();
    }

    private void BeginPan(MouseButtonEventArgs e)
    {
        if (mapCanvas.IsMouseCaptured)
            return;

        mapCanvas.Cursor = Cursors.ScrollAll;
        _dragStart = e.MouseDevice.GetPosition(mapCanvas);
        mapCanvas.CaptureMouse();
    }

    private new void ReleaseMouseCapture()
    {
        mapCanvas.ReleaseMouseCapture();
        mapCanvas.Cursor = Cursors.Arrow;
        base.ReleaseMouseCapture();
    }

    private void HandleLeftClick()
    {
        MapMode mode = _mainWindow.mapModeButtons.GetMapMode();
        switch (mode)
        {
            case MapMode.Political:
                HandlePoliticalClick();
                break;

            case MapMode.Provincial:
                HandleProvincialClick();
                break;

            case MapMode.Terrain:
            default:
                throw new NotImplementedException($"Map mode '{mode}' is not implemented.");
        }

        SoundHandler.PlayClick();
    }

    private void HandleProvincialClick()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            _mainWindow.provinceInterface.Visibility = Visibility.Hidden;
            HandlePoliticalClick(); // TODO: Handle pop edit.
            return;
        }

        Color provinceColor = MapRenderer.GetProvinceColorAt(
            _provinceMap,
            mapPolitical.Position,
            new Size(mapPolitical.ActualWidth, mapPolitical.ActualHeight)
        );

        _mainWindow.provinceInterface.PopulateInterface(provinceColor);

        SelectColor(_provinceMap, mapProvinces.SelectedColor);
    }

    private void HandlePoliticalClick()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            SelectProvinceFromPoliticalMap();
            return;
        }

        SelectCountryFromPoliticalMap();

        _mainWindow.provinceInterface.Visibility = Visibility.Hidden;

        Debug.WriteLine("COUNTRY EDITING NOT YET IMPLEMENTED");
    }

    private void SelectCountryFromPoliticalMap()
    {
        SelectColor(GetMap(MapMode.Political), mapPolitical.SelectedColor);
    }

    private void SelectProvinceFromPoliticalMap()
    {
        SelectColor(_provinceMap, MapRenderer.GetProvinceColorAt(
            _provinceMap,
            mapPolitical.Position,
            new Size(mapPolitical.ActualWidth, mapPolitical.ActualHeight)
        ));
    }

    private void SelectColor(WriteableBitmap source, Color selectedColor)
        => flashingSelection.Source = MapRenderer.DrawConnectedColors(source, MapRenderer.GetRawColor(selectedColor));

    #endregion

    #region Map Navigation & Utility

    private void Map_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        foreach (Image image in MapModes.Values)
        {
            Point position = e.MouseDevice.GetPosition(image);
            Matrix matrix = image.RenderTransform.Value;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {
                TranslateHorizontal(ref matrix, e.Delta);
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) ||
                     Keyboard.IsKeyDown(Key.RightShift))
            {
                TranslateVertical(ref matrix, e.Delta);
            }
            else
            {
                Scale(ref matrix, e.Delta, position);
            }

            SetTransform(image, matrix);
        }
    }

    public void Map_MouseMove(object sender, MouseEventArgs e)
    {
        if (!mapCanvas.IsMouseCaptured)
            return;

        Point currentPosition = e.MouseDevice.GetPosition(mapCanvas);

        var deltaX = _dragStart.X - currentPosition.X;
        var deltaY = _dragStart.Y - currentPosition.Y;

        foreach (Image image in MapModes.Values)
        {
            Matrix matrix = image.RenderTransform.Value;

            matrix.OffsetX -= deltaX;
            matrix.OffsetY -= deltaY;

            SetTransform(image, matrix);
        }

        _dragStart = currentPosition;
    }

    public void MoveTimerTick()
    {
        const double pixelsPerSecond = 2000;
        const double tickSeconds = 0.003;
        const double velocity = pixelsPerSecond * tickSeconds;

        if (!scrollViewer.IsFocused ||
            !_mainWindow.ControlMode.SelectedItem.Equals(
                _mainWindow.Mode_Modern))
        {
            return;
        }

        var verticalDirection = IsImageFlipped ? -1 : 1;

        foreach (Image image in MapModes.Values)
        {
            Matrix matrix = image.RenderTransform.Value;

            if (Keyboard.IsKeyDown(Key.W) ||
                Keyboard.IsKeyDown(Key.Up))
            {
                matrix.Translate(0, verticalDirection * velocity);
            }

            if (Keyboard.IsKeyDown(Key.A) ||
                Keyboard.IsKeyDown(Key.Left))
            {
                matrix.Translate(velocity, 0);
            }

            if (Keyboard.IsKeyDown(Key.S) ||
                Keyboard.IsKeyDown(Key.Down))
            {
                matrix.Translate(0, -verticalDirection * velocity);
            }

            if (Keyboard.IsKeyDown(Key.D) ||
                Keyboard.IsKeyDown(Key.Right))
            {
                matrix.Translate(-velocity, 0);
            }

            SetTransform(image, matrix);
        }
    }

    private void TranslateHorizontal(ref Matrix matrix, int delta)
        => matrix.Translate(Math.Abs(delta), 0);

    private void TranslateVertical(ref Matrix matrix, int delta)
    {
        var direction = IsImageFlipped ? -1 : 1;
        matrix.Translate(0, direction * Math.Abs(delta));
    }

    private static void Scale(ref Matrix matrix, int delta, Point origin)
    {
        var factor = delta > 0 ? 1.1 : 0.9;
        matrix.ScaleAtPrepend(factor, factor, origin.X, origin.Y);
    }

    private void SetTransform(Image image, Matrix matrix)
    {
        var transform = new MatrixTransform(matrix);
        image.RenderTransform = transform;
        flashingSelection.RenderTransform = transform;
    }

    private static void InvertCanvas(Canvas canvas)
    {
        canvas.RenderTransformOrigin = new Point(0.5, 0.5);
        canvas.RenderTransform = new ScaleTransform { ScaleY = -1 };
        IsImageFlipped = true;
    }

    #endregion
}