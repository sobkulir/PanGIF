# Implementation
## Dependencies
I picked Avalonia over WPF for GUI framework because while WPF is Windows-only Avalonia supports all major platforms. Avalonia's documentation
is rather brief (almost nonexistant), however the developers were very responsive from the very beginning so I went for it. (14 Apr 2019)

For the actual editting of the GIFs I used Magick.NET which is an ImageMagick port for C#. I used ImageMagick in the past and liked it
so I just decided to use it for this project as well.

## MVVM
I loosely follow the pattern of MVVM. Each Model and ViewModel derived from the ModelBase and ViewModelBase respectively which implement
`INotifyPropertyChanged` inteface. I opted for View-first approach where MainWindow creates an instance of MainWindowViewModel and assignes
it to its DataContext. It's an easier option to implement.

## GIFInfo Model
This model holds an `ObservableCollection<Frame>` that contains individual frames' bitmaps and crops. It can therefore compute the
intermediate crops for frames that do not have their crops set. This is what `TransitionCropOf(int frameIndex)` does. It is also used
inside the Panner.

## Panner ("backend")
The resulting GIF is generated in the `Panner` class that uses Magick.NET. When its `SavePanned` method is called, it loads the GIF
from disk again as `MagickImageCollection`. This is because I didn't find an easy way to convert from the UI bitmap format (`SKBitmap`)
to the backend one or vice versa. After that the collection is *coalesced* (if the various frames
have transparent areas and build upon each other, this method will transform the frames into their true "displayed" bitmaps).

## Frame Model
