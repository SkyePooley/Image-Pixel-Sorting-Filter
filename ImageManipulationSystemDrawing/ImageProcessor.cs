using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;


namespace ImageManipulationSystemDrawing
{
    public class ImageProcessor
    {
        private string imagePath;
        private Bitmap _image;
        private RawBitmap _rawBitmap;
        public enum SortingKey {
            Hue,
            Saturation,
            Luminance
        }

        /// <summary>
        /// Construct an image processor using an image loaded from disk.
        /// </summary>
        /// <param name="path">Path to the image on file.</param>
        public ImageProcessor(string path)
        {
            imagePath = path;
            _image = new Bitmap(path);
            _rawBitmap = new RawBitmap(_image);
            Console.WriteLine("Width: " + _image.Width);
            Console.WriteLine("Height: " + _image.Height);
        }

        /// <summary>
        /// Construct an image processor using an existing Bitmap object.
        /// </summary>
        /// <param name="image">System.Drawing.Bitmap object containing a preloaded image.</param>
        private ImageProcessor(Bitmap image)
        {
            _image = (Bitmap) image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            // _image = Clone<Bitmap>(image);
            _rawBitmap = new RawBitmap(_image);
        }


        
        /// <summary>
        /// Saves the current state of the contained bitmap to disk.
        /// </summary>
        /// <param name="path">Save location for resulting image.</param>
        public void SaveTo(string path)
        {
            _image.Save(path);
        }
        
        /// <summary>
        /// Creates a black and white contrast mask from the contained bitmap.
        /// Pixels greater than the given brightness threshold will be set to white, all others will be black.
        /// This method is destructive, the result will overwrite the current bitmap.
        /// </summary>
        /// <param name="threshold">Brightness threshold for contrast mask. Must be between 0.0 (black) and 1.0 (white)</param>
        public void ContrastMask(float threshold)
        {
            Stopwatch timer = Stopwatch.StartNew();
            for (int row = 0; row < _image.Height; row++)
            {
                for (int col = 0; col < _image.Width; col++)
                {
                    RawBitmap.Pixel pixel = _rawBitmap.GetPixel(row, col);
                    RawBitmap.Pixel newPixel = ContrastMask(pixel, threshold);
                    _rawBitmap.SetPixel(row, col, newPixel);
                }
            }

            _image = _rawBitmap.CloseAndReturn();
            timer.Stop();
            Console.WriteLine($"Contrast mask done in {timer.ElapsedMilliseconds}ms");
        }
        
        private RawBitmap.Pixel ContrastMask(RawBitmap.Pixel pixel, float threshold)
        {
            float brightness = pixel.Luminance;
            if (brightness > threshold)
            {
                return RawBitmap.Pixel.White();
            }
            else
            {
                return RawBitmap.Pixel.Black();
            }
        }

        /// <summary>
        /// An image filter which sorts rows of pixels meeting the given brightness threshold.
        /// </summary>
        /// <param name="threshold">Brightness threshold for pixel to be included in sorted span. Range 0.0 -> 1.0 where 1.0 is white.</param>
        /// <param name="sortBy">Colour attribute to sort pixels by, either Hue, Saturation, or Luminance</param>
        public void ContrastSort(float threshold, SortingKey sortBy)
        {
            Console.WriteLine("Creating Contrast Mask");
            // Generate a contrast mask of the image for span generation.
            ImageProcessor subProcessor = new ImageProcessor(_image);
            subProcessor.ContrastMask(threshold);

            RawBitmap rawContrastMask = new RawBitmap(subProcessor._image);
            RawBitmap rawImage = new RawBitmap(_image);

            Stopwatch timer = Stopwatch.StartNew();
            
            // Create queues to store new and completed tasks for threads
            ConcurrentQueue<SortingTask> spansToSort = new ConcurrentQueue<SortingTask>();
            ConcurrentQueue<SortingTask> completedSpans = new ConcurrentQueue<SortingTask>();

            // Threads to sort spans
            SorterGroup sorterGroup = new SorterGroup(spansToSort, completedSpans, 1);

            // Thread to place sorted spans back into the image
            Assembler imageAssembler = new Assembler(completedSpans, _image);
            Thread imageAssemblerThread = new Thread(imageAssembler.AssembleImage);
            
            // Start threads
            sorterGroup.start();
            imageAssemblerThread.Start();

            // Search the image for contiguous lines of pixels whose corresponding contrast mask pixel is white
            int row = 0;
            while (row < _image.Height)
            {
                int col = 0;
                while (col < _image.Width)
                {
                    if ((int)rawContrastMask.GetPixel(row, col).Luminance == 1)
                    {
                        Span newSpan = new Span((col, row), _rawBitmap.GetPixel(row, col));
                        col++;
                        // Move along the row until the end of the span is found
                        while (col < _image.Width && rawContrastMask.GetPixel(row, col).Luminance != 0.0f)
                        {
                            newSpan.AddPixel(_rawBitmap.GetPixel(row, col));
                            col++;
                        }
                        // Send the span to be sorted
                        spansToSort.Enqueue(new SortingTask(newSpan, sortBy));
                    }
                    col++;
                }
                row++;
            }

            rawContrastMask.CloseAndReturn();
            Console.WriteLine($"Done finding spans {timer.ElapsedMilliseconds}");
            
            // Wait until the span sorter has finished, then terminate it.
            while (!spansToSort.IsEmpty) ;
            sorterGroup.stop();
            Console.WriteLine($"sort done {timer.ElapsedMilliseconds}ms");
            
            while (!completedSpans.IsEmpty) ;
            imageAssembler.cancel = true;
            _image = imageAssembler.LocalImage;
            Console.WriteLine($"assembly done {timer.ElapsedMilliseconds}ms");
            timer.Stop();
            Console.WriteLine($"Sort complete in {timer.ElapsedMilliseconds}ms");
        }
        
        /// <summary>
        /// Sorts all pixels in all rows of the image by their brightness.
        /// </summary>
        public void SortAll()
        {
            for (int row = 0; row < _image.Height; row++)
            {
                SortRow(_image, row);
            }
        }

        private void SortRow(Bitmap image, int rowIndex)
        {
            List<Color> rowPixels = new List<Color>(image.Width);
            for (int col = 0; col < _image.Width; col++)
            {
                rowPixels.Add(_image.GetPixel(col, rowIndex));
            }
            rowPixels.Sort((x, y) => x.GetBrightness().CompareTo(y.GetBrightness()));
            for (int col = 0; col < _image.Width; col++)
            {
                image.SetPixel(col, rowIndex, rowPixels[col]);
            }
        }
    }
}
