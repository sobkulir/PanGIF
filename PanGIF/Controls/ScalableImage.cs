using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace PanGIF.Controls
{
    class ScalableImage : Control
    {
        public static readonly DirectProperty<ScalableImage, SKBitmap> ImageProperty =
            AvaloniaProperty.RegisterDirect<ScalableImage, SKBitmap>(
                nameof(Image),
                p => p.Image,
                (p, o) => p.Image = o);

        public static readonly DirectProperty<ScalableImage, float> ScaleProperty =
            AvaloniaProperty.RegisterDirect<ScalableImage, float>(
                nameof(Scale),
                p => p.Scale,
                (p, o) => p.Scale = o,
                1);

        private SKBitmap _image;
        private float _scale;


        static ScalableImage()
        {
            AffectsRender<ScalableImage>(BoundsProperty);
            AffectsMeasure<ScalableImage>(ImageProperty, ScaleProperty);
        }

        public ScalableImage()
        {
            ClipToBounds = true;
        }

        public SKBitmap Image
        {
            get => _image;
            set => SetAndRaise(ImageProperty, ref _image, value);
        }

        public float Scale
        {
            get => _scale;
            set => SetAndRaise(ScaleProperty, ref _scale, value);
        }
        class CustomDrawOp : ICustomDrawOperation
        {
            private readonly FormattedText _noSkia;
            private readonly SKBitmap _image;
            private readonly float _scale;

            public CustomDrawOp(SKBitmap image, float scale, FormattedText noSkia, Rect bounds)
            {
                _image = image;
                _scale = scale;
                _noSkia = noSkia;
                Bounds = bounds;
            }

            public void Dispose()
            {
                // No-op
            }

            public Rect Bounds { get; }
            public bool HitTest(Point p) => false;
            public bool Equals(ICustomDrawOperation other) => false;
            public void Render(IDrawingContextImpl context)
            {
                var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
                if (canvas == null)
                {
                    context.DrawText(Brushes.Black, new Point(), _noSkia.PlatformImpl);
                    return;
                }
                if (_image == null)
                {
                    return;
                }

                canvas.Save();
                var rect1 = SKRect.Create(0, 0, _image.Width, _image.Height);
                var rect2 = SKRect.Create(0, 0, _image.Width * _scale, _image.Height * _scale);
                canvas.DrawBitmap(_image, rect1, rect2, new SKPaint { IsAntialias = false });
                canvas.Restore();
            }
        }

        public override void Render(DrawingContext context)
        {
            var noSkia = new FormattedText()
            {
                Text = "Current rendering API is not Skia"
            };
            context.Custom(new CustomDrawOp(Image, Scale, noSkia, new Rect(0, 0, Bounds.Width, Bounds.Height)));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var image = Image;

            if (image != null)
            {
                Size scaledSourceSize = new Size(Scale * image.Width, Scale * image.Height);
                return scaledSourceSize;
            }
            else
            {
                return new Size();
            }
        }
    }
}
