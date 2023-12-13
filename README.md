# ReadMe - Color Palette Tool

## Table of Contents

- Overview
- How to Use
  - Creating a Palette
  - Adding Color Options
  - Creating More Palettes
  - Deleting Colors from a Palette
  - Deleting a Palette
  - Using Palette Colors
  - Switching the Active Palette
  - Adding more Color Components
- How it Works
- Ideas for Extension
- License

## Overview

This tool arose from a need to create a single whitebox app that could be 
rebranded quickly and easily, after which the app could be rebuilt for 
a new company. Searching for tools online wasn't particularly fruitful,
and so this tool was born.

If you need to be able to create a color palette with multiple variations
that can be swapped between in 2 clicks, then this is the tool for you.

## How to Use

### Creating a Palette
The main interface of the Color Palette tool can be accessed using Unity's
Tool menu. This will bring up a new Editor window in which you can create
a new Color Palette.

### Adding Color Options
Once you've created a Color Palette, add Color Options to it using the 
appropriate button (it will only be displayed if the first/main Color 
Palette has been selected). You can rename the Color Palette and Color 
Options as you wish. Adding Color Options to the first/main Palette will
add them to all Palettes.

### Creating more Palettes
Clicking on the __Create Palette__ button again will create a variation
of the first Palette with the same Color Options. These colors can be
changed without affecting the first/main Palette's colors.

### Deleting Colors from a Palette
Deleting Color Options from any Palette will remove the Color Option from
__every__ Palette, so be careful!

### Deleting a Palette
Deleting a Palette will leave the Color Options intact, unless there is only
one Palette, in which case all the Color Options will be deleted as well.
Deleting the main Palette will lead to the second Palette in the window
becoming the main Palette (i.e. there will always be a main Palette).

### Using Palette Colors
Once you have a Palette set-up with some Color Options, you'll want certain
components within your application, like Images, to apply these colors. To
do so, you will need to apply the relevant script to each component you'd
like to be affected by the Palette. For example, if you need an Image
Component's Sprite Color to be affected, you need to attach an Image Color
Component to the same GameObject.

### Switching the Active Palette
If you want to change the Colors of your application on the fly, select the
checkbox next to the Palette you want to be set as active and click on the
__Update Color Components__ button. The Colors of all relevant components
should update fairly instantly, though it may take a second or two for them
to update their display in the Scene and Game View.

### Adding more Color Components
You may find that there isn't a Color Component that applies to your
specific use case (e.g. an SVG Image Color Component) - don't fret! In order
to get your custom component supported, all you need to do is create a class
that extends __'ColorComponent\<T>'__, replacing __\<T>__ with the class type
you wish the Color Component to affect (e.g. __'ColorComponent\<SVGImage>'__),
then override the __ApplyColor__ method and assigning the __Target__ color
to the __ActiveColor__.

For example:
````
public class SVGColorComponent : ColorComponent<SVGImage>
{
    protected override void ApplyColor()
    {
        base.ApplyColor();
        Target.color = ActiveColor; // This will change based on the class type
    }
}
````

## How it Works
The Color Palette system is built around the use of Scriptable Objects. Each 
Palette is a Scriptable Object, as well as each Color Option, and the Palette
List which stores all the Palettes and thus Color Options. You shouldn't need
to interact directly with these Scriptable Objects, but the choice is there should
you need it.

By default, when creating a Palette, the Scriptable Object gets stored in:
`Assets/Plugins/True_Outlaw/ColorPalette/ColorPalettes/`

By default, when creating a Color Option, the Scriptable Object gets stored in:
`Assets/Plugins/True_Outlaw/ColorPalette/ColorOptions/`

The PaletteList Scriptable Object gets stored in:
`Assets/Plugins/True_Outlaw/ColorPalette/`

## Ideas for Extension
This asset is provided under the MIT license and is made publicly available in 
order for those who wish to contribute and enhance this tool to do so. There
are some areas in which we feel the tool can be improved:
- General bug-fixing
- Usability and user experience enhancements
- Functionality to apply updates to prefabs with ColorComponent-related scripts\
so that these don't have to be changed manually or at runtime

## License

__The MIT License (MIT)__

Copyright © 2023 True Outlaw

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the “Software”), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
THE SOFTWARE.
