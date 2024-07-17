namespace ImageManipulationSystemDrawing;

/// <summary>
/// Holds a span of pixels to sort and the attribute to sort them by.
/// Intended to be added to a queue of work to be consumed by the SpanSorter
/// </summary>
public class SortingTask
{
    internal Span Span { get; }
    internal ImageProcessor.SortingKey SortingKey { get; }

    /// <summary>
    /// Holds a span of pixels to sort and the attribute to sort them by.
    /// Intended to be added to a queue of work to be consumed by the SpanSorter
    /// </summary>
    /// <param name="span">Span containing a contiguous row of pixels from the ImageProcessor image.</param>
    /// <param name="sortingKey">Attribute which pixels should be sorted by.</param>
    public SortingTask(Span span, ImageProcessor.SortingKey sortingKey)
    {
        Span = span;
        SortingKey = sortingKey;
    }
}