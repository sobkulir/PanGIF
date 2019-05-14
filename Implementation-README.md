# Implementation
## Dependencies
I picked Avalonia over WPF for GUI framework because while WPF is Windows-only Avalonia supports all major platforms. Avalonia's documentation
is rather brief (almost nonexistant), however the developers were very responsive from the very beginning so I went for it. (14 Apr 2019)

For the actual editting of the GIFs I used Magick.NET which is an ImageMagick port for C#. I used ImageMagick in the past and liked it
so I just decided to use it for this project as well.

## MVVM
I loosely follow the MVVM pattern. Each Model and ViewModel derive from ModelBase and ViewModelBase, respectively, which implement
`INotifyPropertyChanged` inteface. I opted for View-first approach where MainWindow creates an instance of MainWindowViewModel and assignes
it to its DataContext. It's an easier option to implement.

## Frame Model
Frame model holds an `SKBitmap Bitmap` and `Rect? Crop`. It also contains `EnforceCropAspectRatio(double ratio)` that changes `Crop` so that its `Crop.Width / Crop.Height` equals to `ratio`. I used a technique where I first try to keep `Crop.Height` intact and just change `Crop.Width` according to the ratio. If this causes `Crop` to flow outside of the `Bitmap` I shrink the width to just touch the edge and then shrink the height accordingly. Then I do the same for the `Crop.Width`, i.e. I try to keep it intact and adjust the height according to the ratio. Afterwards I pick a resulting crop based on which of these 2 computed crops have a bigger area. This means that the center of the crop stays the same after enforcing the aspect ratio.

## GIFInfo Model
This model holds an `ObservableCollection<Frame>` that contains individual frames' bitmaps and crops. It can therefore compute the
intermediate crops for frames that do not have their crops set. This is what `TransitionCropOf(int frameIndex)` does. This static method is
also used inside the Panner.

## Panner ("backend")
The resulting GIF is generated in the `Panner` class that uses Magick.NET. When its `SavePanned` method is called, it loads the GIF
from disk again as `MagickImageCollection`. This is because I didn't find an easy way to convert from the UI bitmap format (`SKBitmap`)
to the backend one or vice versa. After that the collection is *coalesced*, i.e. if various frames
have transparent areas and build upon each other, this method will transform the frames into their true "displayed" bitmaps. The rest of
the function just picks the right crop from each frame, resizes it and the newly obtained collection is then saved.

## Controls
I had to create quite a few custom controls because Avalonia does not have a rich library like WPF, yet. I also derived from `UserControl` instead of `TemplatedControl` which means that I hard-coded styles of the controls. However, creating `TemplatedControl` is more complex and I didn't see any benefits in not hardcoding the styles of the controls (YAGNI).

### ImageCrop
This control has an Image source, Crop and some other properties. It displays the Image using SkiaSharp to allow disabling of antialiasing when scaling. I think it's important to show true pixels when scaling the image in an image editting software. This functionality is encapsulated in `ScalableImage` Control.

`ImageCrop` has also handles for crop resizing. Coming up with the right formulas for generalizing the movement of multiple handles into one general formula was surprisingly time-consuming. However I managed to do so and thus the handles' movement handlers are DRY.

### KeyFrameSlider
This is the slider on top of the frame. I didn't use Avalonia's slider, because there's currently a bug in its implementation of "SnapToTick" version. It doesn't snap to ticks. (14 Apr 2019) Creating the slider itself is trivial. The hardest part was figuring out how to display `n` uniformly positioned rectangles in a rectangle of given width. Yeah... I had to implement custom control for it `UniformRects`. This control displays ticks and also the keyframe markers on the slider.
