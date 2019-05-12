using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.ComponentModel;
using System.Linq;

namespace PanGIF.Controls
{
    class ImageCrop : UserControl
    {
        private enum HandleType
        {
            Top, Right, Bottom, Left, TopLeft, TopRight, BottomRight, BottomLeft
        }

        public static readonly DirectProperty<ImageCrop, SKBitmap> ImageProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, SKBitmap>(
                nameof(Image),
                p => p.Image,
                (p, o) => p.Image = o);

        public static readonly DirectProperty<ImageCrop, double> ScaleProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, double>(
                nameof(Scale),
                p => p.Scale,
                (p, o) => p.Scale = o,
                1);

        public static readonly DirectProperty<ImageCrop, Rect> CropProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, Rect>(
                nameof(Crop),
                p => p.Crop,
                (p, o) => p.Crop = o,
                defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static readonly DirectProperty<ImageCrop, Rect> ScaledCropProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, Rect>(
                nameof(ScaledCrop),
                p => p.ScaledCrop);

        public static readonly DirectProperty<ImageCrop, Boolean> KeepAspectRatioProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, Boolean>(
                nameof(KeepAspectRatio),
                p => p.KeepAspectRatio,
                (p, o) => p.KeepAspectRatio = o);

        public static readonly DirectProperty<ImageCrop, int> RatioWidthProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, int>(
                nameof(RatioWidth),
                p => p.RatioWidth,
                (p, o) => p.RatioWidth = o);

        public static readonly DirectProperty<ImageCrop, int> RatioHeightProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, int>(
                nameof(RatioHeight),
                p => p.RatioHeight,
                (p, o) => p.RatioHeight = o);

        public static readonly DirectProperty<ImageCrop, Thickness> CropMarginProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, Thickness>(
                nameof(CropMargin),
                p => p.CropMargin);

        public static readonly DirectProperty<ImageCrop, double> CropHeightProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, double>(
                nameof(CropHeight),
                p => p.CropHeight);

        public static readonly DirectProperty<ImageCrop, double> CropWidthProperty =
            AvaloniaProperty.RegisterDirect<ImageCrop, double>(
                nameof(CropWidth),
                p => p.CropWidth);

        private SKBitmap _image;
        private double _scale;
        private Rect _crop;
        private bool _keepAspectRatio;
        private int _ratioWidth;
        private int _ratioHeight;

        private HandleType? _pressedHandle = null;
        private bool _isDraggingCrop = false;
        private Rect _oldCrop;
        private Point _oldMousePosition;

        public ImageCrop()
        {
            this.InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            foreach (HandleType handleType in Enum.GetValues(typeof(HandleType)))
            {
                this.FindControl<Polygon>(handleType.ToString()).PointerPressed += HandlePressedFactory(handleType);
                this.FindControl<Polygon>(handleType.ToString()).PointerMoved += HandleMovedFactory(handleType);
                this.FindControl<Polygon>(handleType.ToString()).PointerReleased += HandleReleased;
            }
        }

        public Rect ScaledCrop => new Rect(Crop.TopLeft.X * Scale, Crop.TopLeft.Y * Scale, Crop.Width * Scale, Crop.Height * Scale);
        public Thickness CropMargin => new Thickness(Crop.TopLeft.X * Scale, Crop.TopLeft.Y * Scale, 0, 0);
        public double CropHeight => Crop.Height * Scale;
        public double CropWidth => Crop.Width * Scale;

        public SKBitmap Image
        {
            get => _image;
            set => SetAndRaise(ImageProperty, ref _image, value);
        }

        public double Scale
        {
            get => _scale;
            set
            {
                var oldScaledCrop = ScaledCrop;
                var oldCropMargin = CropMargin;
                var oldCropHeight = CropHeight;
                var oldCropWidth = CropWidth;

                SetAndRaise(ScaleProperty, ref _scale, value);
                RaisePropertyChanged(ScaledCropProperty, oldScaledCrop, ScaledCrop);
                RaisePropertyChanged(CropMarginProperty, oldCropMargin, CropMargin);
                RaisePropertyChanged(CropHeightProperty, oldCropHeight, CropHeight);
                RaisePropertyChanged(CropWidthProperty, oldCropWidth, CropWidth);
            }
        }

        public Rect Crop
        {
            get => _crop;
            set
            {
                var oldScaledCrop = ScaledCrop;
                var oldCropMargin = CropMargin;
                var oldCropHeight = CropHeight;
                var oldCropWidth = CropWidth;

                SetAndRaise(CropProperty, ref _crop, value);
                RaisePropertyChanged(ScaledCropProperty, oldScaledCrop, ScaledCrop);
                RaisePropertyChanged(CropMarginProperty, oldCropMargin, CropMargin);
                RaisePropertyChanged(CropHeightProperty, oldCropHeight, CropHeight);
                RaisePropertyChanged(CropWidthProperty, oldCropWidth, CropWidth);
            }
        }

        public Boolean KeepAspectRatio
        {
            get => _keepAspectRatio;
            set => SetAndRaise(KeepAspectRatioProperty, ref _keepAspectRatio, value);
        }

        public int RatioWidth
        {
            get => _ratioWidth;
            set => SetAndRaise(RatioWidthProperty, ref _ratioWidth, value);
        }

        public int RatioHeight
        {
            get => _ratioHeight;
            set => SetAndRaise(RatioHeightProperty, ref _ratioHeight, value);
        }

        private EventHandler<PointerPressedEventArgs> HandlePressedFactory(HandleType type)
        {
            return delegate (object sender, PointerPressedEventArgs e)
            {
                e.Handled = true;
                e.Device.Capture((IInputElement)sender);
                _pressedHandle = type;
                _oldCrop = Crop;
                _oldMousePosition = e.GetPosition(this.VisualRoot);
            };
        }

        private EventHandler<PointerEventArgs> HandleMovedFactory(HandleType type)
        {
            return delegate (object sender, PointerEventArgs e)
            {
                e.Handled = true;
                if (_pressedHandle != type) return;

                var newpos = e.GetPosition(this.VisualRoot);
                var delta = new Vector(
                    Math.Floor((newpos.X - _oldMousePosition.X) / Scale),
                    Math.Floor((newpos.Y - _oldMousePosition.Y) / Scale));
                var bounds = new Rect(0, 0, Image.Width, Image.Height);

                if (KeepAspectRatio)
                    Crop = ResizeCrop(type, bounds, _oldCrop, delta, (double)RatioWidth / RatioHeight);
                else
                    Crop = ResizeCrop(type, bounds, _oldCrop, delta);
            };
        }

        private void HandleReleased(object sender, PointerReleasedEventArgs e)
        {
            e.Handled = true;
            e.Device.Capture(null);
            _pressedHandle = null;
        }

        public void CropPressed(object sender, PointerPressedEventArgs e)
        {
            e.Handled = true;
            e.Device.Capture((IInputElement)sender);

            _isDraggingCrop = true;
            _oldMousePosition = e.GetPosition(this.VisualRoot);
            _oldCrop = Crop;
        }
        public void CropMoved(object sender, PointerEventArgs e)
        {
            e.Handled = true;
            if (!_isDraggingCrop) return;

            var newpos = e.GetPosition(this.VisualRoot);
            var pixelMoveX = Math.Floor((newpos.X - _oldMousePosition.X) / Scale);
            var pixelMoveY = Math.Floor((newpos.Y - _oldMousePosition.Y) / Scale);
            
            if (_oldCrop.TopLeft.X + pixelMoveX < 0)
                pixelMoveX = (-1) * _oldCrop.TopLeft.X;

            if (_oldCrop.BottomRight.X + pixelMoveX >= Image.Width)
                pixelMoveX = Image.Width - _oldCrop.BottomRight.X;

            if (_oldCrop.Y + pixelMoveY < 0)
                pixelMoveY = (-1) * _oldCrop.Y;

            if (_oldCrop.BottomRight.Y + pixelMoveY >= Image.Height)
                pixelMoveY = Image.Height - _oldCrop.BottomRight.Y;

            Crop = _oldCrop.Translate(new Vector(pixelMoveX, pixelMoveY));
        }

        public void CropReleased(object sender, PointerReleasedEventArgs e)
        {
            e.Handled = true;
            e.Device.Capture(null);
            _isDraggingCrop = false;
        }

        private static Rect ResizeCrop(HandleType type, Rect bounds, Rect oldCrop, Vector delta)
        {
            var topLeft = oldCrop.TopLeft;
            var bottomRight = oldCrop.BottomRight;

            if (new[] { HandleType.Top, HandleType.TopLeft, HandleType.TopRight }.Contains(type))
                topLeft = topLeft.WithY(Math.Min(Math.Max(topLeft.Y + delta.Y, 0), bottomRight.Y));

            if (new[] { HandleType.Right, HandleType.TopRight, HandleType.BottomRight }.Contains(type))
                bottomRight = bottomRight.WithX(Math.Max(Math.Min(bottomRight.X + delta.X, bounds.Width), topLeft.X));

            if (new[] { HandleType.Bottom, HandleType.BottomLeft, HandleType.BottomRight }.Contains(type))
                bottomRight = bottomRight.WithY(Math.Max(Math.Min(bottomRight.Y + delta.Y, bounds.Height), topLeft.Y));

            if (new[] { HandleType.Left, HandleType.TopLeft, HandleType.BottomLeft }.Contains(type))
                topLeft = topLeft.WithX(Math.Min(Math.Max(topLeft.X + delta.X, 0), bottomRight.X));

            return new Rect(topLeft, bottomRight);
        }

        private static Rect ResizeCrop(HandleType type, Rect bounds, Rect oldCrop, Vector delta, double ratio)
        {
            Point? cornerCode = null;
            if (type == HandleType.TopLeft)
                cornerCode = new Point(0, 0);
            else if (type == HandleType.TopRight)
                cornerCode = new Point(1, 0);
            else if (type == HandleType.BottomRight)
                cornerCode = new Point(1, 1);
            else if (type == HandleType.BottomLeft)
                cornerCode = new Point(0, 1);

            if (cornerCode != null)
                return ResizeByCornerKeepAspectRatio(bounds, oldCrop, delta, ratio, cornerCode.Value);
            else
                return oldCrop;
        }

        private static Rect ResizeByCornerKeepAspectRatio(Rect bounds, Rect oldCrop, Vector delta, double ratio, Point cornerCode)
        {
            var cornerCoef = new Point(
                (cornerCode.X == 0) ? -1 : 1,
                (cornerCode.Y == 0) ? -1 : 1);

            var slackLeft = oldCrop.TopLeft.X - bounds.TopLeft.X;
            var slackTop = oldCrop.TopLeft.Y - bounds.TopLeft.Y;
            var slackRight = bounds.BottomRight.X - oldCrop.BottomRight.X;
            var slackBottom = bounds.BottomRight.Y - oldCrop.BottomRight.Y;
        
            var availableRect = new Rect(
                new Point(
                    oldCrop.TopLeft.X + (cornerCode.X - 1) * slackLeft,
                    oldCrop.TopLeft.Y + (cornerCode.Y - 1) * slackTop),
                new Point(
                    oldCrop.BottomRight.X + cornerCode.X * slackRight,
                    oldCrop.BottomRight.Y + cornerCode.Y * slackBottom));

            var cornerPoint = new Point(
                    oldCrop.TopLeft.X + cornerCode.X * oldCrop.Width,
                    oldCrop.TopLeft.Y + cornerCode.Y * oldCrop.Height);

            delta = ClipPointMoveToBounds(availableRect, cornerPoint, delta);
            var newWidth = oldCrop.Width + cornerCoef.X * delta.X;
            var newHeight = Math.Floor(newWidth / ratio);
            delta = delta.WithY(cornerCoef.Y * (newHeight - oldCrop.Height));
            delta = ClipPointMoveToBounds(availableRect, cornerPoint, delta);
            var newHeightClipped = oldCrop.Height + cornerCoef.Y * delta.Y;
            newWidth = (newHeightClipped == newHeight) ? newWidth : Math.Floor(newHeightClipped * ratio);
            delta = delta.WithX(cornerCoef.X * (newWidth - oldCrop.Width));
            return new Rect(
                new Point(
                    oldCrop.TopLeft.X + (1 - cornerCode.X) * delta.X,
                    oldCrop.TopLeft.Y + (1 - cornerCode.Y) * delta.Y),
                new Point(
                    oldCrop.BottomRight.X + cornerCode.X * delta.X,
                    oldCrop.BottomRight.Y + cornerCode.Y * delta.Y));
        }

        private static Vector ClipPointMoveToBounds(Rect bounds, Point point, Vector delta)
        {
            if (point.X + delta.X < bounds.TopLeft.X)
                delta = delta.WithX(bounds.TopLeft.X - point.X);
           
            if (point.X + delta.X > bounds.BottomRight.X)
                delta = delta.WithX(bounds.BottomRight.X - point.X);

            if (point.Y + delta.Y < bounds.TopLeft.Y)
                delta = delta.WithY(bounds.TopLeft.Y - point.Y);

            if (point.Y + delta.Y > bounds.BottomRight.Y)
                delta = delta.WithY(bounds.BottomRight.Y - point.Y);

            return delta;
        }
    }
}
