using Avalonia;
using ImageMagick;
using PanGIF.Models;
using System;

namespace PanGIF
{
    static class Panner
    {
        public static void SavePanned(
            GIFInfo curGIF,
            string outputFilename,
            int targetWidth,
            int targetHeight,
            IProgress<int> progressValue,
            IProgress<int> progressTotal)
        {
            progressTotal.Report(4);
            progressValue.Report(1);

            var imageFrames = new MagickImageCollection(curGIF.InputFileName);
            progressValue.Report(2);
            using (MagickImageCollection collection = new MagickImageCollection())
            {
                imageFrames.Coalesce();
                for (int i = 0; i < imageFrames.Count; ++i)
                {
                    imageFrames[i].Crop(RectToMagickGeometry(curGIF.TransitionCropOf(i)));
                    imageFrames[i].RePage();
                    var geo = new MagickGeometry(targetWidth, targetHeight);
                    geo.IgnoreAspectRatio = true;
                    imageFrames[i].Resize(geo);
                    collection.Add(imageFrames[i]);
                }

                progressValue.Report(3);
                collection.Optimize();
                collection.Write(outputFilename);
                progressValue.Report(4);
            }
        }

        private static MagickGeometry RectToMagickGeometry(Rect rect)
        {
            return new MagickGeometry((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}
