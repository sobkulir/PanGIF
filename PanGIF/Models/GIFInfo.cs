using Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PanGIF.Models
{
    class GIFInfo : ModelBase
    {
        private ObservableCollection<Frame> _frames;
        public string InputFileName { get; set; }

        public GIFInfo(string inputFileName, List<Frame> frames)
        {
            Frames = new ObservableCollection<Frame>(frames);
            Frames.CollectionChanged += (s, e) =>
            {
                if (e != null && e.OldItems.Count != e.NewItems.Count)
                    OnPropertyChanged(nameof(FrameCount));
            };

            InputFileName = inputFileName;
        }

        public ObservableCollection<Frame> Frames
        {
            get => _frames;
            set
            {
                _frames = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FrameCount));
            }
        }

        public int FrameCount => Frames.Count;
        
        public Rect TransitionCropOf(int frameIndex)
        {
            int fromIndex, toIndex;

            fromIndex = frameIndex;
            // At least 0th frame has always a crop.
            while (fromIndex >= 0 && Frames[fromIndex].Crop == null) --fromIndex;

            toIndex = frameIndex;
            while (toIndex < FrameCount && Frames[toIndex].Crop == null) ++toIndex;
            if (toIndex >= FrameCount)
                toIndex = fromIndex;

            var fromCrop = Frames[fromIndex].Crop.Value;
            var toCrop = Frames[toIndex].Crop.Value;
            var fromTL = fromCrop.TopLeft;
            var toTL = toCrop.TopLeft;

            var coef = (fromIndex == toIndex) ? 0 : ((double)(frameIndex - fromIndex)) / (toIndex - fromIndex);
            var x = (int)Math.Floor(fromTL.X + coef * (toTL.X - fromTL.X));
            var y = (int)Math.Floor(fromTL.Y + coef * (toTL.Y - fromTL.Y));

            var fromWidth = fromCrop.Width;
            var fromHeight = fromCrop.Height;
            var toWidth = toCrop.Width;
            var toHeight = toCrop.Height;

            var width = (int)Math.Floor(fromWidth + coef * (toWidth - fromWidth));
            var height = (int)Math.Floor(fromHeight + coef * (toHeight - fromHeight));

            return new Rect(x, y, width, height);
        }

        public void EnforceCropAspectRatio(double ratio)
        {
            foreach (var frame in Frames)
            {
                frame.EnforceCropAspectRatio(ratio);
            }
        }
    }
}
