using System.Collections.Concurrent;
using System.Drawing;

namespace ImageManipulationSystemDrawing;

public class SpanSorter
{
    private ConcurrentQueue<SortingTask> _spansToSort;
    private ConcurrentQueue<SortingTask> _completedSpans;
    private volatile bool _cancel;

    public SpanSorter(ConcurrentQueue<SortingTask> spansToSort, ConcurrentQueue<SortingTask> completedSpans, bool cancelToken)
    {
        this._spansToSort = spansToSort;
        this._completedSpans = completedSpans;
        this._cancel = cancelToken;
    }
    
    public void SortSpans()
    {
        while (!_cancel)
        {
            if (!_spansToSort.TryDequeue(out SortingTask? task)) continue;
                
            // // Sort based on caller determined attribute
            // switch (task.SortingKey)
            // {
            //     case ImageProcessor.SortingKey.Hue: task.Span.HueSort(); break;
            //     case ImageProcessor.SortingKey.Saturation: task.Span.SaturationSort(); break;
            //     case ImageProcessor.SortingKey.Luminance: task.Span.LuminanceSort(); break;
            // }
            task.Span.LuminanceSort();
                
            //Place sorted pixels back into the image
            _completedSpans.Enqueue(task);
        }
    }
}