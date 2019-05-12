using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PanGIF.Controls
{
    class Highlighter : Control
    {
        public static readonly DirectProperty<Highlighter, Rect> RegionProperty =
            AvaloniaProperty.RegisterDirect<Highlighter, Rect>(
                nameof(Region),
                p => p.Region,
                (p, o) => p.Region = o);

        private Rect _region;

        static Highlighter()
        {
            AffectsRender<Highlighter>(BoundsProperty);
            AffectsRender<Highlighter>(RegionProperty);
        }

        public Highlighter()
        {

        }

        public Rect Region
        {
            get => _region;
            set
            {
                if (_region != value)
                {
                    SetAndRaise(RegionProperty, ref _region, value);
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            var w = Bounds.Width;
            var h = Bounds.Height;

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(new Point(0, 0), true);
                ctx.LineTo(new Point(w, 0));
                ctx.LineTo(new Point(w, h));
                ctx.LineTo(new Point(0, h));
                ctx.LineTo(new Point(0, 0));

                ctx.LineTo(Region.TopLeft);
                ctx.LineTo(Region.TopRight);
                ctx.LineTo(Region.BottomRight);
                ctx.LineTo(Region.BottomLeft);
                ctx.LineTo(Region.TopLeft);
            }

            using (context.PushGeometryClip(geo))
            {
                var shadow = new SolidColorBrush(Colors.Black, 0.5);
                context.FillRectangle(shadow, Bounds, 0);
            }
        }
    }
}