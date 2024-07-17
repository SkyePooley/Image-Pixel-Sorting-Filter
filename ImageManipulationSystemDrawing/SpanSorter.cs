using System.Collections.Concurrent;
using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class SpanSorter
{
    public Bitmap LocalImage { get; }
    private ConcurrentQueue<SortingTask> spansToSort;
    public volatile bool cancel;

    public SpanSorter(Bitmap image, ConcurrentQueue<SortingTask> spansToSort)
    {
        LocalImage = (Bitmap)image.Clone();
        this.spansToSort = spansToSort;
    }
    
    public void SortSpans()
    {
        while (!cancel)
        {
            if (!spansToSort.TryDequeue(out SortingTask? task)) continue;
                
            // Sort based on caller determined attribute
            switch (task.SortingKey)
            {
                case ImageProcessor.SortingKey.Hue: task.Span.HueSort(); break;
                case ImageProcessor.SortingKey.Saturation: task.Span.SaturationSort(); break;
                case ImageProcessor.SortingKey.Luminance: task.Span.LuminanceSort(); break;
            }
                
            //Place sorted pixels back into the image
            (int x, int y) position = task.Span.StartPosition;
            foreach (Color pixel in task.Span.Pixels)
            {
                LocalImage.SetPixel(position.x, position.y, pixel);
                position.x++;
            }
        }
    }
}