using Avalonia;
using SkiaSharp;
using System;
using System.Diagnostics;

namespace PanGIF.Models
{
    class Frame : ModelBase
    {
        private SKBitmap _bitmap;
        private Rect? _crop;

        public Frame(SKBitmap bitmap, Rect? crop)
        {
            Bitmap = bitmap;
            Crop = crop;
        }

        public Frame(SKBitmap bitmap) : this(bitmap, null)
        {
        }

        public SKBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                OnPropertyChanged();
            }
        }

        public Rect? Crop
        {
            get => _crop;
            set
            {
                _crop = value;
                OnPropertyChanged();
            }
        }

        public void EnforceCropAspectRatio(double ratio)
        {
            Debug.Assert(ratio != 0D);

            if (Crop == null) return;
            if (Math.Abs(Crop.Value.Width / Crop.Value.Height - ratio) < 0.001D) return;

            var crop = Crop.Value;
            Rect bounds = new Rect(0, 0, Bitmap.Width, Bitmap.Height);
            Vector delta;
            double newWidth, newHeight;

            // Stretch width based on current height.
            delta = new Vector((crop.Width - ratio * crop.Height) / 2, 0);
            delta = ClipRectMoveToBounds(bounds, crop, delta);
            newWidth = Math.Floor(crop.Width - 2 * delta.X);
            newHeight = Math.Floor(newWidth / ratio);
            var widthStretchSize = new Size(newWidth, newHeight);

            // Stretch height based on current width.
            delta = new Vector(0, (crop.Height - crop.Width / ratio) / 2);
            delta = ClipRectMoveToBounds(bounds, crop, delta);
            newHeight = Math.Floor(crop.Height - 2 * delta.Y);
            newWidth = Math.Floor(ratio * newHeight);
            var heightStretchSize = new Size(newWidth, newHeight);

            // Pick crop with bigger area.
            Size newSize;
            if (widthStretchSize.Width * widthStretchSize.Height < heightStretchSize.Width * heightStretchSize.Height)
                newSize = heightStretchSize;
            else
                newSize = widthStretchSize;

            delta = new Vector(
                Math.Truncate((crop.Width - newSize.Width) / 2),
                Math.Truncate((crop.Height - newSize.Height) / 2));

            delta = ClipRectMoveToBounds(bounds, crop, delta);
            Crop = new Rect(crop.TopLeft.X + delta.X, crop.TopLeft.Y + delta.Y, newSize.Width, newSize.Height);
        }

        private static Vector ClipRectMoveToBounds(Rect bounds, Rect movedObj, Vector delta)
        {
            if (movedObj.TopLeft.X + delta.X < bounds.TopLeft.X)
                delta = delta.WithX(bounds.TopLeft.X - movedObj.TopLeft.X);

            if (movedObj.BottomRight.X - delta.X > bounds.BottomRight.X)
                delta = delta.WithX(movedObj.BottomRight.X - bounds.BottomRight.X);

            if (movedObj.TopLeft.Y + delta.Y < bounds.TopLeft.Y)
                delta = delta.WithY(bounds.TopLeft.Y - movedObj.TopLeft.Y);

            if (movedObj.BottomRight.Y - delta.Y > bounds.BottomRight.Y)
                delta = delta.WithY(movedObj.BottomRight.Y - bounds.BottomRight.Y);

            return delta;
        }
    }
}
