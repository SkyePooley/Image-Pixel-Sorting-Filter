using System.Collections.Concurrent;
using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class Assembler
{
    private RawBitmap _rawImage;

    public Bitmap LocalImage => _rawImage.CloseAndReturn();

    private ConcurrentQueue<SortingTask> sortedSpans;
    public volatile bool cancel;

    public Assembler(ConcurrentQueue<SortingTask> sortedSpans, Bitmap image)
    {
        _rawImage = new RawBitmap(image);
        this.sortedSpans = sortedSpans;
    }

    public void AssembleImage()
    {
        while (!cancel)
        {
            // Retrieve sorted span
            if (!sortedSpans.TryDequeue(out SortingTask? task)) continue;
            
            (int x, int y) position = task.Span.StartPosition;
            foreach (RawBitmap.Pixel pixel in task.Span.Pixels)
            {
                _rawImage.SetPixel(position.y, position.x, pixel);
                position.x++;
            }
        }
    }
}