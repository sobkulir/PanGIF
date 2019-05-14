# PanGIF
A desktop app for panning and zooming GIF images. You simply define keyframes as rectangular selections of the original frames and PanGIF creates linear transitions between these keyframes. The app can be run on all major OSs: Windows, MacOS, Linux. PanGIF uses [Avalonia](https://github.com/AvaloniaUI/Avalonia) for the GUI and [Magick.NET](https://github.com/dlemstra/Magick.NET) for manipulating GIFs.

![PanGIF UI with a loaded image.](https://lh3.googleusercontent.com/lxAQJyfSMo5QqcQsHeGvyeYQqm1CG11fjtOzi7OHhxxfv12lp0rFS5R0qg0GOKhzjS2DT2P6pLg4DPS2FTR9xw2y191of7RNnBWvZFg8I4HqQx-8FGLD1ethovCr7f95T69HDkw_padVsSn_Nxgvv6b6QdFPHWK2JAB5YkzecXPh0_YZM9TZGaJNuLk3pFNFNX1J1U-esPtq24qjDPKo_LcnKPIYdLoQLzB9k_1v5IKwPvTYIS7U9gknSZjMycGW3N42dtGSmBEz4cKqlQ662hjkjmzsPfEAo_DRshvx1K0W27RdySxTn48gAPOkaYBZtRksWimXa6fjgqMCd7JQ7PP2lcQur4cCWucM5gOTpooeDYk15eodc9DXYW_a5MpFSEWT__4KpA_h9MU7kDHRWJqDQkrtEePL1otlLALZdDQU0EGR3oj-6-WDM8mqvvJBRMtJfq0uezquBYb3P2LwNt9Ly9shybho0Mr2Zo939pPF6NEHyd84ZTybC1a04WXK2L6HRf96GjS7SXTDOBKhU-K4E0NZaIYiCbHbguLOqYPFG7VFR5lVeI8YwkoPKfpowR5VqnsSMlpFb1Sk55Qzymo52bNPrUrrW5__vWo=w1842-h892)

## Example
| Original | Transformed |
|---|---|
| ![Original GIF](https://lh3.googleusercontent.com/o1cV5z4H-ViBuyuv8Az0EFq3ldRDlBfWsKv7ZKgfnyynfSyJuKGXB-Q89JMzJjEcAwWn5xIkAtcMduDXqrEuhVrGj4dvb132O3NEDw1WiukaXZOj0KcLzcVpQrRq3zVLddqIfwLcHr9CXAJ5MFki3nhsBv6VM3G4KoaZbysgt_QVnD9PNgRSnWw510Nd4cUz1waj5CfllrSE21vmEew-1tbMw5xPXNJUZ6kM4vIZvfCLRUtJ1WojzKgH1wbCFnbnSLx3JhVQOiwxgAsA_klObBgdgrtPkYFPzjwNQ9dgalp5tAPOrmu7ks-bxiY5HL4M2xHdcR0M6VJ_EC9AWXQPmOnF0e3y5cUkd6ve9PwpfnppH3MOGHW_97wl6salAu_pxleZxm9S7HzN13dhs1tt3s5eHdfDcfHcN9kAr6ndXIJOU0ezMr4MmuBCY1wrakUrBwhwZaY1tuR1u0so38-Bm6GwWfsUJ6P1zPjK2X_VvmFphzEAdBdXzOBTES1y_vccvR6_Pd3Wy_H47UfU-bWYoqpOmLvaiHzesVEGyP4blsVUFIsC_swtKQgYq3NhCTA8MfpTjU63jnqfowp_2H6AhhlqsPk3T7f5yjC0TuA=w1842-h892) | ![Transformed GIF](https://lh3.googleusercontent.com/KdKGqgmo77gX_1l0K7Xp60gEAqECj_yBE---uS9ksIULcAtSzqpE4GSZqwDwZkXnWnPEad3fR4rZOs79T3zbx_6efLke3xtqNsKm1tdGr6aPHr-Kaipo6jxAKHS1yX8i7zSS4iFexi99VHyLOn1evMbDyNtMdmo1uF-DjRm1pY1oGOOHwy-tKKCy_YIJIi6xkQShzHpzRchzZpWRJdy1fGftAQBpxw_cQWuM3FL2ChrVjhG-uCI2GVgKig_njHnF8c4VGtBE4bCM_wWIbhH-rEwvJAXnV5rWWeDbsEAY5Pmjch2-Ubzrad7t_JSu63jF4to1wd9PEhNuBROhuCtVIlRex4YfvqkRsOU3JvgSge-BuaU_1KHoNE-b3SpGKeW6wPOLXXlpoKrZA_TU1IVnBVlyyOc04QlBPEJdzWBn04IDb9IFSdGxzQa3P1Kdx1smXP0QWwdF6x4i_qz0010A7MYc2_QcceLHTQXGFQeQzgKUqrQZmjsB503qLw0KpM8W2kxI2GCrX8USl65leJW_SQOWscOZS6Taw2FxPJLZRyZTyFZlklfBvMNh9pbzmU1U9O2MWDekPw_UhnIq4GBNSef2c341PEnsWxH5vvI=w1842-h892) |

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
