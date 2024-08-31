using System.Collections.Concurrent;
using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class Assembler
{
    public Bitmap LocalImage { get; }
    private ConcurrentQueue<SortingTask> sortedSpans;
    public volatile bool cancel;

    public Assembler(ConcurrentQueue<SortingTask> sortedSpans, Bitmap image)
    {
        LocalImage = (Bitmap)image.Clone();
        this.sortedSpans = sortedSpans;
    }

    public void AssembleImage()
    {
        while (!cancel)
        {
            // Retrieve sorted span
            if (!sortedSpans.TryDequeue(out SortingTask? task)) continue;
            
            (int x, int y) position = task.Span.StartPosition;
            foreach (Color pixel in task.Span.Pixels)
            {
                LocalImage.SetPixel(position.x, position.y, pixel);
                position.x++;
            }
        }
    }
}