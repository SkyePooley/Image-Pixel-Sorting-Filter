using System.Drawing;

namespace ImageManipulationSystemDrawing;

/// <summary>
/// A contiguous span of pixels taken from a bitmap image.
/// Pixels are stored as Color objects in a list.
/// Provides methods to sort contained pixels by their colour attributes.
/// </summary>
public class Span
{
    internal (int x, int y) StartPosition { get; }
    internal List<RawBitmap.Pixel> Pixels { get; }

    /// <summary>
    /// Instantiate a span. One starting pixel and its location must be passed.
    /// </summary>
    /// <param name="startPosition">Position of the first pixel in the span.</param>
    /// <param name="startPixel">Color data of the first pixel.</param>
    public Span((int x, int y) startPosition, RawBitmap.Pixel startPixel)
    {
        StartPosition = startPosition;
        Pixels = new List<RawBitmap.Pixel> { startPixel };
    }

    public void AddPixel(RawBitmap.Pixel pixel)
    {
        Pixels.Add(pixel);
    }

    public void LuminanceSort()
    {
        Pixels.Sort((x, y) => x.Luminance.CompareTo(y.Luminance));
    }

    // public void HueSort()
    // {
    //     Pixels.Sort((x, y) => x.GetHue().CompareTo(y.GetHue()));
    // }
    //
    // public void SaturationSort()
    // {
    //     Pixels.Sort((x, y) => x.GetSaturation().CompareTo(y.GetSaturation()));
    // }
}