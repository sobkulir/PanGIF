using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using PanGIF.Models;
using System;

namespace PanGIF.Controls
{
    class KeyFrameSlider : UserControl
    {
        public static readonly DirectProperty<KeyFrameSlider, GIFInfo> CurGIFProperty =
           AvaloniaProperty.RegisterDirect<KeyFrameSlider, GIFInfo>(
               nameof(CurGIF),
               p => p.CurGIF,
               (o, v) => o.CurGIF = v);

        public static readonly DirectProperty<UniformRects, double> ValueProperty =
            AvaloniaProperty.RegisterDirect<UniformRects, double>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v,
                defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static readonly DirectProperty<KeyFrameSlider, double> TrackWidthProperty =
            AvaloniaProperty.RegisterDirect<KeyFrameSlider, double>(
                nameof(TrackWidth),
                p => p.TrackWidth,
                (o, v) => o.TrackWidth = v);

        public static readonly DirectProperty<KeyFrameSlider, double> ThumbOffsetProperty =
            AvaloniaProperty.RegisterDirect<KeyFrameSlider, double>(
                nameof(ThumbOffset),
                p => p.ThumbOffset);

        public static readonly DirectProperty<KeyFrameSlider, double> TickOnTrackWidthProperty =
            AvaloniaProperty.RegisterDirect<KeyFrameSlider, double>(
                nameof(TickOnTrackWidth),
                p => p.TickOnTrackWidth);

        private GIFInfo _curGIF;
        private double _value;
        private double _trackWidth;

        private Grid _track;
        private bool _isDraggingThumb = false;
        private Point _oldMousePosition;
        private double _oldThumbOffset;

        public KeyFrameSlider()
        {
            InitializeComponent();
            _track = this.FindControl<Grid>("Track");
            _track.PropertyChanged += (s, e) =>
            {
                TrackWidth = _track.Bounds.Width;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public GIFInfo CurGIF
        {
            get => _curGIF;
            set
            {
                var oldTickOnTrackWidth = TickOnTrackWidth;
                var oldThumbOffset = ThumbOffset;
                SetAndRaise(CurGIFProperty, ref _curGIF, value);
                RaisePropertyChanged(TickOnTrackWidthProperty, oldTickOnTrackWidth, TickOnTrackWidth);
                RaisePropertyChanged(ThumbOffsetProperty, oldThumbOffset, oldThumbOffset);
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                var oldThumbOffset = ThumbOffset;
                SetAndRaise(ValueProperty, ref _value, value);
                RaisePropertyChanged(ThumbOffsetProperty, oldThumbOffset, ThumbOffset);
            }
        }

        public double TrackWidth
        {
            get => _trackWidth;
            set
            {
                var oldTickOnTrackWidth = TickOnTrackWidth;
                var oldThumbOffset = ThumbOffset;
                SetAndRaise(TrackWidthProperty, ref _trackWidth, value);
                RaisePropertyChanged(TickOnTrackWidthProperty, oldTickOnTrackWidth, TickOnTrackWidth);
                RaisePropertyChanged(ThumbOffsetProperty, oldThumbOffset, oldThumbOffset);
            }
        }

        public double ThumbOffset => Value * TickOnTrackWidth;

        public double TickOnTrackWidth => (CurGIF == null || CurGIF.FrameCount == 1) ? 0 : TrackWidth / (CurGIF.FrameCount - 1);

        public void ThumbPressed(object sender, PointerPressedEventArgs e)
        {
            e.Handled = true;
            e.Device.Capture((IInputElement)sender);

            _isDraggingThumb = true;
            _oldMousePosition = e.GetPosition(VisualRoot);
            _oldThumbOffset = ThumbOffset;
        }

        public void ThumbMoved(object sender, PointerEventArgs e)
        {
            e.Handled = true;
            if (!_isDraggingThumb) return;

            var newpos = e.GetPosition(VisualRoot);
            var moveX = newpos.X - _oldMousePosition.X;
            MoveThumb(_oldThumbOffset + moveX);
        }

        public void ThumbReleased(object sender, PointerReleasedEventArgs e)
        {
            e.Device.Capture(null);
            _isDraggingThumb = false;
            e.Handled = true;
        }

        private void MoveThumb(double newOffset)
        {
            newOffset = Math.Max(newOffset, 0);
            newOffset = Math.Min(newOffset, TrackWidth);
            Value = Math.Round(newOffset / TickOnTrackWidth);
        }

        // Copy-paste from Avalonia.Input.Gestures, because Tapped event does not
        // carry pointer location yet.
        private static WeakReference s_lastPress;

        public void TrackTappedPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount <= 1)
                s_lastPress = new WeakReference(e.Source);
        }

        public void TrackTappedReleased(object sender, PointerReleasedEventArgs e)
        {
            if (s_lastPress?.IsAlive == true && s_lastPress.Target == e.Source)
            {
                // Tapped
                MoveThumb(e.GetPosition(_track).X);
            }
        }
    }
}
