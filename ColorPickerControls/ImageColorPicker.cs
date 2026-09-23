using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paradox_Editor.ColorPickerControls;

/// <summary>
/// An Image control that allows a user to select a pixel color from its source.
/// </summary>
public class ImageColorPicker : Image
{
    private Point _position;
    private RenderTargetBitmap? _cachedTargetBitmap;

    #region Dependency Properties

    private static readonly DependencyPropertyKey SelectedColorPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ImageColorPicker),
            new FrameworkPropertyMetadata(
                Colors.Transparent,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SelectorProperty =
        DependencyProperty.Register(
            nameof(Selector),
            typeof(Drawing),
            typeof(ImageColorPicker),
            new FrameworkPropertyMetadata(
                CreateDefaultSelector(),
                FrameworkPropertyMetadataOptions.AffectsRender),
            value => value is not null);

    /// <summary>
    /// Gets the currently selected color.
    /// </summary>
    public Color SelectedColor =>
        (Color)GetValue(SelectedColorPropertyKey.DependencyProperty);

    /// <summary>
    /// Gets or sets the drawing used to indicate the selected pixel.
    /// </summary>
    public Drawing Selector
    {
        get => (Drawing)GetValue(SelectorProperty);
        set => SetValue(SelectorProperty, value);
    }

    private static Drawing CreateDefaultSelector() =>
        new GeometryDrawing(
            Brushes.White,
            new Pen(Brushes.Black, 0.1),
            new RectangleGeometry(new Rect(-0.1, -0.1, 0.2, 0.2)));

    #endregion

    #region Rendering

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (ActualWidth <= 0 || ActualHeight <= 0)
            return;

        drawingContext.PushTransform(
            new TranslateTransform(_position.X, _position.Y));

        drawingContext.DrawDrawing(Selector);

        drawingContext.Pop();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        _cachedTargetBitmap = null;

        if (sizeInfo.PreviousSize.Width <= 0 ||
            sizeInfo.PreviousSize.Height <= 0)
        {
            return;
        }

        Position = new Point(
            _position.X * sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width,
            _position.Y * sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == SourceProperty)
        {
            _cachedTargetBitmap = null;
            Position = new Point();
        }

        base.OnPropertyChanged(e);
    }

    #endregion

    #region Position

    internal Point Position
    {
        get => _position;
        set
        {
            Point newPosition = ClampPosition(value);

            if (_position == newPosition)
                return;

            _position = newPosition;

            Color color = PickColor(_position.X, _position.Y);

            if (color == SelectedColor)
                InvalidateVisual();

            SetValue(SelectedColorPropertyKey, color);
        }
    }

    private Point ClampPosition(Point point)
    {
        return new Point(
            Math.Clamp(point.X, 0, ActualWidth),
            Math.Clamp(point.Y, 0, ActualHeight));
    }

    private void SetPositionIfInBounds(Point point)
    {
        if (point.X < 0 ||
            point.X > ActualWidth ||
            point.Y < 0 ||
            point.Y > ActualHeight)
        {
            return;
        }

        Position = point;
    }

    #endregion

    #region Target Bitmap

    private RenderTargetBitmap? TargetBitmap
    {
        get
        {
            if (_cachedTargetBitmap != null)
                return _cachedTargetBitmap;

            if (Source is not DrawingImage drawingImage)
                return null;

            if (ActualWidth <= 0 || ActualHeight <= 0)
                return null;

            var drawingVisual = new DrawingVisual();

            using (DrawingContext? context = drawingVisual.RenderOpen())
            {
                context.DrawDrawing(drawingImage.Drawing);
            }

            Rect bounds = drawingVisual.ContentBounds;

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return null;

            drawingVisual.Transform = new ScaleTransform(
                ActualWidth / bounds.Width,
                ActualHeight / bounds.Height);

            _cachedTargetBitmap = new RenderTargetBitmap(
                Math.Max(1, (int)ActualWidth),
                Math.Max(1, (int)ActualHeight),
                96,
                96,
                PixelFormats.Pbgra32);

            _cachedTargetBitmap.Render(drawingVisual);

            return _cachedTargetBitmap;
        }
    }

    #endregion

    #region Mouse Handling

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        SetPositionIfInBounds(e.GetPosition(this));
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        if (e.LeftButton == MouseButtonState.Pressed)
            SetPositionIfInBounds(e.GetPosition(this));
    }

    #endregion

    #region Color Picking

    private Color PickColor(double x, double y)
    {
        if (Source == null)
            throw new InvalidOperationException("Image source has not been set.");

        return Source switch
        {
            BitmapSource bitmap => PickBitmapColor(bitmap, x, y),
            DrawingImage => PickDrawingColor(x, y),
            _ => throw new InvalidOperationException(
                $"Unsupported image source type: {Source.GetType().Name}")
        };
    }

    /// <summary>
    /// Gets the color at the current selector position from another Image.
    /// </summary>
    public Color PickColor(Image image)
    {
        if (image.Source == null)
            throw new InvalidOperationException("Image source has not been set.");

        if (image.Source is not BitmapSource bitmap)
        {
            if (image.Source is DrawingImage)
            {
                return PickDrawingColor(
                    _position.X,
                    _position.Y);
            }

            throw new InvalidOperationException(
                $"Unsupported image source type: {image.Source.GetType().Name}");
        }

        if (image.ActualWidth <= 0 || image.ActualHeight <= 0)
            throw new InvalidOperationException("Image has no valid dimensions.");

        Point point = ScaleToBitmap(
            _position,
            image.ActualWidth,
            image.ActualHeight,
            bitmap.PixelWidth,
            bitmap.PixelHeight);

        return ReadBitmapPixel(bitmap, point.X, point.Y);
    }

    private Color PickBitmapColor(
        BitmapSource bitmap,
        double x,
        double y)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0)
            throw new InvalidOperationException("Image has no valid dimensions.");

        Point point = ScaleToBitmap(
            new Point(x, y),
            ActualWidth,
            ActualHeight,
            bitmap.PixelWidth,
            bitmap.PixelHeight);

        return ReadBitmapPixel(bitmap, point.X, point.Y);
    }

    private Color PickDrawingColor(double x, double y)
    {
        RenderTargetBitmap? targetBitmap = TargetBitmap;

        if (targetBitmap == null)
            throw new InvalidOperationException(
                "Unable to create a bitmap from the drawing source.");

        Point point = ScaleToBitmap(
            new Point(x, y),
            ActualWidth,
            ActualHeight,
            targetBitmap.PixelWidth,
            targetBitmap.PixelHeight);

        return ReadBitmapPixel(targetBitmap, point.X, point.Y);
    }

    private static Point ScaleToBitmap(
        Point point,
        double sourceWidth,
        double sourceHeight,
        int bitmapWidth,
        int bitmapHeight)
    {
        if (sourceWidth <= 0 || sourceHeight <= 0)
            throw new InvalidOperationException(
                "Source dimensions must be greater than zero.");

        var x = point.X * bitmapWidth / sourceWidth;
        var y = point.Y * bitmapHeight / sourceHeight;

        return new Point(
            Math.Clamp(x, 0, bitmapWidth - 1),
            Math.Clamp(y, 0, bitmapHeight - 1));
    }

    private static Color ReadBitmapPixel(
        BitmapSource bitmap,
        double x,
        double y)
    {
        int pixelX = (int)x;
        int pixelY = (int)y;

        return bitmap.Format switch
        {
            var format when format == PixelFormats.Indexed4 =>
                ReadIndexed4Pixel(bitmap, pixelX, pixelY),

            var format when format == PixelFormats.Indexed8 =>
                ReadIndexed8Pixel(bitmap, pixelX, pixelY),

            _ => ReadBgraPixel(bitmap, pixelX, pixelY)
        };
    }

    private static Color ReadIndexed4Pixel(
        BitmapSource bitmap,
        int x,
        int y)
    {
        if (bitmap.Palette == null)
            throw new InvalidOperationException(
                "Indexed bitmap has no palette.");

        byte[] pixel = new byte[1];

        int stride =
            (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 3) / 4;

        bitmap.CopyPixels(
            new Int32Rect(x, y, 1, 1),
            pixel,
            stride,
            0);

        return bitmap.Palette.Colors[pixel[0] >> 4];
    }

    private static Color ReadIndexed8Pixel(
        BitmapSource bitmap,
        int x,
        int y)
    {
        if (bitmap.Palette == null)
            throw new InvalidOperationException(
                "Indexed bitmap has no palette.");

        byte[] pixel = new byte[1];

        int stride =
            (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;

        bitmap.CopyPixels(
            new Int32Rect(x, y, 1, 1),
            pixel,
            stride,
            0);

        return bitmap.Palette.Colors[pixel[0]];
    }

    private static Color ReadBgraPixel(
        BitmapSource bitmap,
        int x,
        int y)
    {
        byte[] pixel = new byte[4];

        int stride =
            (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;

        bitmap.CopyPixels(
            new Int32Rect(x, y, 1, 1),
            pixel,
            stride,
            0);

        return Color.FromArgb(
            pixel[3],
            pixel[2],
            pixel[1],
            pixel[0]);
    }

    #endregion
}