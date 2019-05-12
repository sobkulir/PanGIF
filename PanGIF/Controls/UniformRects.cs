using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using PanGIF.Models;
using System;

namespace PanGIF.Controls
{
    class UniformRects : Control
    {
        public static readonly DirectProperty<UniformRects, GIFInfo> CurGIFProperty =
            AvaloniaProperty.RegisterDirect<UniformRects, GIFInfo>(
                nameof(CurGIF),
                p => p.CurGIF,
                (o, v) => o.CurGIF = v);

        public static readonly DirectProperty<KeyFrameSlider, double> ValueProperty =
            KeyFrameSlider.ValueProperty.AddOwner<KeyFrameSlider>(o => o.Value, (o, v) => o.Value = v);

        public static readonly DirectProperty<UniformRects, Boolean> IsBigProperty =
            AvaloniaProperty.RegisterDirect<UniformRects, Boolean>(
                nameof(IsBig),
                p => p.IsBig,
                (o, v) => o.IsBig = v);

        private GIFInfo _curGIF;
        private double _value;
        private bool _isBig;

        public UniformRects()
        {
            AffectsRender<UniformRects>(CurGIFProperty, BoundsProperty, ValueProperty);
        }

        public GIFInfo CurGIF
        {
            get => _curGIF;
            set
            {
                SetAndRaise(CurGIFProperty, ref _curGIF, value);
                if (value != null)
                {
                    value.Frames.CollectionChanged += (o, e) =>
                    {
                        InvalidateVisual();
                    };
                }
            }
        }

        public double Value
        {
            get => _value;
            set => SetAndRaise(ValueProperty, ref _value, value);
        }

        public bool IsBig
        {
            get => _isBig;
            set => SetAndRaise(IsBigProperty, ref _isBig, value);
        }


        public double Offset => (CurGIF == null || CurGIF.FrameCount == 1) ? 0 : Bounds.Width / (CurGIF.FrameCount - 1);

        public override void Render(DrawingContext context)
        {
            if (CurGIF == null)
            {
                return;
            }

            var frames = CurGIF.Frames;
            for (int i = 0; i < CurGIF.FrameCount; ++i)
            {
                if (IsBig)
                {
                    var fillBrush = (i == Value) ? Brushes.Green : Brushes.Chocolate;
                    if (CurGIF.Frames[i].Crop != null)
                    {
                        context.FillRectangle(fillBrush, new Rect(Offset * i, 0, 10, 15));
                        context.DrawRectangle(new Pen(Brushes.Black, 1), new Rect(Offset * i, 0, 10, 15));
                    }
                }
                else
                {
                    context.FillRectangle(Brushes.Black, new Rect(Offset * i, 0, 2, 5));
                }
            }
        }

        // Copy-paste from Avalonia.Input.Gestures because Tapped event does not
        // carry pointer location yet.
        private static WeakReference s_lastPress;
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (!IsBig) return;

            if (e.ClickCount <= 1)
                s_lastPress = new WeakReference(e.Source);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (!IsBig) return;

            if (s_lastPress?.IsAlive == true && s_lastPress.Target == e.Source)
            {
                e.Handled = true;
                // Tapped event
                if (CurGIF == null) return;

                var clickOffset = e.GetPosition(this).X / Offset;
                var newValue = Math.Max(0, Math.Floor(clickOffset));
                newValue = Math.Min(newValue, CurGIF.FrameCount - 1);
                Value = newValue;
            }
        }
    }
}
