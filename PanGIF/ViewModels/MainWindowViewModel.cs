using Avalonia;
using PanGIF.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PanGIF.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private GIFInfo _curGIF;
        private int _selectedFrameIndex;
        private double _scale;
        private bool _keepAspectRatio;
        private int _targetWidth;
        private int _targetHeight;
        public LoadSaveProgress CurProgress { get; set; }

        public MainWindowViewModel()
        {
            CurProgress = new LoadSaveProgress(true, 0, 1);
            SelectedFrameIndex = 0;
            Scale = 1F;
        }

        public bool HasCrop => (SelectedFrameIndex != 0 && CurGIF != null && CurGIF.Frames[SelectedFrameIndex].Crop != null);
        public SKBitmap SelectedFrameBitmap => CurGIF?.Frames[SelectedFrameIndex].Bitmap ?? null;

        public GIFInfo CurGIF
        {
            get => _curGIF;
            set
            {
                _curGIF = value;
                OnPropertyChanged();
            }
        }

        public int SelectedFrameIndex
        {
            get => _selectedFrameIndex;
            set
            {
                if (value != _selectedFrameIndex)
                {
                    _selectedFrameIndex = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedFrameBitmap));
                    OnPropertyChanged(nameof(SelectedFrameCrop));
                    OnPropertyChanged(nameof(HasCrop));
                }
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (value != _scale)
                {
                    _scale = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool KeepAspectRatio
        {
            get => _keepAspectRatio;
            set
            {
                if (value != _keepAspectRatio)
                {
                    _keepAspectRatio = value;
                    if (value == true) CurGIF.EnforceCropAspectRatio((double)TargetWidth / TargetHeight);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedFrameCrop));
                }
            }
        }

        public int TargetWidth
        {
            get => _targetWidth;
            set
            {
                if (value != _targetWidth)
                {

                    _targetWidth = value;
                    if (KeepAspectRatio)
                    {
                        CurGIF.EnforceCropAspectRatio((double)TargetWidth / TargetHeight);
                        OnPropertyChanged(nameof(SelectedFrameCrop));
                    }
                    OnPropertyChanged();
                }
            }
        }

        public int TargetHeight
        {
            get => _targetHeight;
            set
            {
                if (value != _targetHeight)
                {
                    _targetHeight = value;
                    if (KeepAspectRatio)
                    {
                        CurGIF.EnforceCropAspectRatio((double)TargetWidth / TargetHeight);
                        OnPropertyChanged(nameof(SelectedFrameCrop));
                    }
                    OnPropertyChanged();
                }
            }
        }

        public Rect? SelectedFrameCrop
        {
            get
            {
                if (CurGIF == null)
                    return default(Rect);
                else if (CurGIF.Frames[SelectedFrameIndex].Crop == null)
                    return CurGIF.TransitionCropOf(SelectedFrameIndex);
                else
                    return CurGIF.Frames[SelectedFrameIndex].Crop.Value;
            }
            set
            {
                if (CurGIF != null)
                    CurGIF.Frames[SelectedFrameIndex] =
                        new Frame(CurGIF.Frames[SelectedFrameIndex].Bitmap, value);

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCrop));
            }
        }

        public async Task ChangeSourceFile(string inputFilename)
        {
            var progressValue = new Progress<int>((i) => CurProgress.Value = i);
            var progressTotal = new Progress<int>((i) => CurProgress.Total = i);

            CurProgress.IsActive = true;
            CurGIF = await Task.Run(() => ReadGIF(inputFilename, progressValue, progressTotal));
            CurProgress.IsActive = false;

            if (CurGIF.FrameCount > 0)
            {
                TargetWidth = CurGIF.Frames[0].Bitmap.Width;
                TargetHeight = CurGIF.Frames[0].Bitmap.Height;
            }

            SelectedFrameIndex = 0;
            // SelectedFrameIndex might have already been 0. This triggers explicit rerender.
            OnPropertyChanged(nameof(SelectedFrameBitmap));
            OnPropertyChanged(nameof(SelectedFrameCrop));
        }

        public async Task SavePanned(string outputFilename)
        {
            var progressValue = new Progress<int>((i) => CurProgress.Value = i);
            var progressTotal = new Progress<int>((i) => CurProgress.Total = i);

            CurProgress.Total = 4;
            CurProgress.IsActive = true;
            await Task.Run(() => 
                Panner.SavePanned(CurGIF, outputFilename, TargetWidth, TargetHeight, progressValue, progressTotal));
            CurProgress.IsActive = false;
        }

        public void DeleteCurrentCrop()
        {
            Debug.Assert(CurGIF.Frames[SelectedFrameIndex].Crop != null);
            SelectedFrameCrop = null;
        }

        private GIFInfo ReadGIF(string inputFilename, IProgress<int> progressValue, IProgress<int> progressTotal)
        {
            List<Frame> frames;
            GIFInfo gif;

            using (Stream stream = File.OpenRead(inputFilename))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            using (SKCodec codec = SKCodec.Create(skStream))
            {
                frames = new List<Frame>(codec.FrameCount);
                progressTotal.Report(codec.FrameCount);

                for (int frame = 0; frame < codec.FrameCount; frame++)
                {
                    SKImageInfo imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                    var bitmap = new SKBitmap(imageInfo);

                    // Get the address of the pixels in that bitmap.
                    IntPtr pointer = bitmap.GetPixels();

                    // Create an SKCodecOptions value to specify the frame.
                    SKCodecOptions codecOptions = new SKCodecOptions(frame);

                    // Copy pixels from the frame into the bitmap.
                    codec.GetPixels(imageInfo, pointer, codecOptions);
                    frames.Add(new Frame(bitmap));
                    progressValue.Report(frame);
                }

                gif = new GIFInfo(inputFilename, frames);
                // First frame has a hardcoded crop.
                gif.Frames[0].Crop = new Rect(0, 0, codec.Info.Width, codec.Info.Height);
            }

            return gif;
        }
    }
}
