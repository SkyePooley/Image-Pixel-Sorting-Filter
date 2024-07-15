using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class Span
{
    internal (int x, int y) StartPosition { get; }
    internal List<Color> Pixels { get; }

    public Span((int x, int y) startPosition, Color startPixel)
    {
        StartPosition = startPosition;
        Pixels = new List<Color> { startPixel };
    }

    public void AddPixel(Color pixel)
    {
        Pixels.Add(pixel);
    }

    public void LuminanceSort()
    {
        Pixels.Sort((x, y) => x.GetBrightness().CompareTo(y.GetBrightness()));
    }

    public void HueSort()
    {
        Pixels.Sort((x, y) => x.GetHue().CompareTo(y.GetHue()));
    }

    public void SaturationSort()
    {
        Pixels.Sort((x, y) => x.GetSaturation().CompareTo(y.GetSaturation()));
    }
}