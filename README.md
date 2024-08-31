# Pixel Sorting Image Filter
Sorts pixels in photos for artistic effect.

![Image of result from filter](https://github.com/SkyePooley/Image-Pixel-Sorting-Filter/blob/main/ImageManipulationSystemDrawing/Saved%20Results/readMe%20example.png?raw=true)  
*Result from the filter using hue sorting and a 60% brightness threshold.*

This program generates a contrast mask from the given image where pixels meeting a specified brightness threshold are white and all others are black.
Pixels matching white regions on the contrast mask are placed into groups and sorted by either their hue, saturation, or brightness.
The result is saved into the root of the project as output.png

### Current Issues
- No cross platform support, System.Drawing used for bitmap editing is Windows only.
- No user interface. Connfiguration must be made from the program main method.
