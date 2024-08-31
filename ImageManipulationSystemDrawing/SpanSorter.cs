using System.Collections.Concurrent;
using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class SpanSorter
{
    private ConcurrentQueue<SortingTask> spansToSort;
    private ConcurrentQueue<SortingTask> completedSpans;
    private volatile bool cancel;

    public SpanSorter(ConcurrentQueue<SortingTask> spansToSort, ConcurrentQueue<SortingTask> completedSpans, bool cancelToken)
    {
        this.spansToSort = spansToSort;
        this.completedSpans = completedSpans;
        this.cancel = cancelToken;
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
            completedSpans.Enqueue(task);
        }
    }
}