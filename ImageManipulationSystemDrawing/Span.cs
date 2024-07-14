using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class Span
{
    private (int x, int y) StartPosition { get; }
    private List<Color> Pixels { get; }

    public Span((int x, int y) startPosition, List<Color> pixels)
    {
        StartPosition = startPosition;
        Pixels = pixels;
        Pixels.TrimExcess();
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