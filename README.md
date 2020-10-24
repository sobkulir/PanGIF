# PanGIF
A desktop app for panning and zooming GIF images. You simply define keyframes as rectangular selections of the original frames and PanGIF creates linear transitions between these keyframes. The app can be run on all major OSs: Windows, MacOS, Linux. PanGIF uses [Avalonia](https://github.com/AvaloniaUI/Avalonia) for the GUI and [Magick.NET](https://github.com/dlemstra/Magick.NET) for manipulating GIFs.

![PanGIF UI with a loaded image.](https://raw.githubusercontent.com/sobkulir/PanGIF/master/readme_images/app.png?raw=true)

## Example
| Original | Transformed |
|---|---|
| ![Original GIF](https://raw.githubusercontent.com/sobkulir/PanGIF/master/readme_images/borat_orig.gif?raw=true) | ![Transformed GIF](https://raw.githubusercontent.com/sobkulir/PanGIF/master/readme_images/borat_after.gif?raw=true) |

## Features
* Zooming
* Panning
* Support for semi-transparent frames
* Defining arbitrary target width and height
* Keeping aspect ratio of target width and height
* Showing computed transition selections

## Current release
[PanGIF-0.9-win-x64](https://sourceforge.net/projects/pangif/files/)

[PanGIF-0.9-win-x86](https://sourceforge.net/projects/pangif/files/)

*MacOS (TBA)*

*Linux (TBA)*

## Usage
### Loading GIF
Firstly, you should navigate to `File -> Open...` and select a GIF you'd like to pan/zoom. After the GIF loads you'll see its fully selected first frame. You can now start editting -- pick different frames using a slider above the current frame and select different portions of them using selection handles.

### Bottom panel
#### Aspect ratio
You can enable keeping the aspect ratio of target size by ticking a checkbox in the bottom right corner.

#### Target size
To define resulting size you can use `W(idth):` and `H(eight):` input boxes in the bottom right corner. By default dimensions of the original GIF are used. In general it's good to keep this value equal to the size of your biggest keyframe, i.e. keyframe with the biggest selection.

#### Selection size
Size of the current selection can be found in the bottom left corner, right next to zoom factor. There are 4 numbers `(X, Y, W, H)` where `X, Y` represents location of the upper left corner of the selection relative to the upper left corner of the original image. `W, H` are width and height of the selection.

#### Zoom factor
If the GIF is too small or too big you can zoom in or zoom out using zoom factor found in the bottom left corner.

### Top panel
Top panel contains main slider used for choosing a frame to edit. When you change selection of the current frame for the first time you turned it into a keyframe. Keyframes are marked with orange rectangles above the slider.

*Tip: If you want to quickly switch current frame to a keyframe you can click on its orange rectangle.*

*Tip: If you want to pick a frame based on its number you can do so using an input box to the right of the main slider.* 

To delete the current frame selection you should use a `Delete Keyframe` button found on the very right of the top panel.

### Saving GIF
To export your work go to `File -> Export GIF...` and choose location of the output GIF.

## What's missing
* Showing a preview of editted GIF.
* Keyboard/mouse shortcuts
* Undo/Redo
* Saving progress
