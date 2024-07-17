using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;


namespace ImageManipulationSystemDrawing
{
    internal class ImageProcessor
    {
        private Bitmap Image { get; }
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
            Image = new Bitmap(path);
            Console.WriteLine("Width: " + Image.Width);
            Console.WriteLine("Height: " + Image.Height);
        }

        /// <summary>
        /// Construct an image processor using an existing Bitmap object.
        /// </summary>
        /// <param name="image">System.Drawing.Bitmap object containing a preloaded image.</param>
        public ImageProcessor(Bitmap image)
        {
            this.Image = (Bitmap) image.Clone();
        }

        /// <summary>
        /// Saves the current state of the contained bitmap to disk.
        /// </summary>
        /// <param name="path">Save location for resulting image.</param>
        public void SaveTo(string path)
        {
            Image.Save(path);
        }
        
        /// <summary>
        /// Creates a black and white contrast mask from the contained bitmap.
        /// Pixels greater than the given brightness threshold will be set to white, all others will be black.
        /// This method is destructive, the result will overwrite the current bitmap.
        /// </summary>
        /// <param name="threshold">Brightness threshold for contrast mask. Must be between 0.0 (black) and 1.0 (white)</param>
        public void ContrastMask(float threshold)
        {
            for (int row = 0; row < Image.Height; row++)
            {
                for (int col = 0; col < Image.Width; col++)
                {
                    Color pixel = Image.GetPixel(col, row);
                    Color newPixel = this.ContrastMask(pixel, threshold);
                    Image.SetPixel(col, row, newPixel);
                }
            }
        }
        
        private Color ContrastMask(Color pixel, float threshold)
        {
            float brightness = pixel.GetBrightness();
            if (brightness > threshold)
            {
                return Color.White;
            }
            else
            {
                return Color.Black;
            }
        }

        /// <summary>
        /// Sorts all pixels in all rows of the image by their brightness.
        /// </summary>
        public void SortAll()
        {
            for (int row = 0; row < Image.Height; row++)
            {
                SortRow(Image, row);
            }
        }

        private void SortRow(Bitmap image, int rowIndex)
        {
            List<Color> rowPixels = new List<Color>(image.Width);
            for (int col = 0; col < Image.Width; col++)
            {
                rowPixels.Add(Image.GetPixel(col, rowIndex));
            }
            rowPixels.Sort((x, y) => x.GetBrightness().CompareTo(y.GetBrightness()));
            for (int col = 0; col < Image.Width; col++)
            {
                image.SetPixel(col, rowIndex, rowPixels[col]);
            }
        }

        public void ContrastSort(float threshold, SortingKey sortBy)
        {
            // Generate a contrast mask of the image for span generation.
            Console.WriteLine("Generating Contrast Mask");
            ImageProcessor subProcessor = new ImageProcessor(this.Image);
            subProcessor.ContrastMask(threshold);
            Bitmap contrastMask = subProcessor.Image;
            
            // Find all the contiguous areas of white in the contrast mask.
            // Place the corresponding image pixels into a span object.
            Console.WriteLine("Generating spans");
            List<Span> spans = new List<Span>();
            int y = 0;
            while (y < Image.Height)
            {
                int x = 0;
                while (x < Image.Width)
                {
                    if (contrastMask.GetPixel(x, y).GetBrightness().Equals(1))
                    {
                        Span newSpan = new Span((x, y), Image.GetPixel(x, y));
                        x++;
                        while (x < Image.Width && contrastMask.GetPixel(x, y).GetBrightness() != 0)
                        {
                            newSpan.AddPixel(Image.GetPixel(x, y));
                            x++;
                        }
                        spans.Add(newSpan);
                    }

                    x++;
                }

                y++;
            }
            
            // Sort the spans that were generated and place them back into the image.
            Console.WriteLine("spans " + spans.Count);
            Console.WriteLine("Sorting Spans");
            foreach (Span span in spans)
            {
                // Sort based on caller determined attribute
                switch (sortBy)
                {
                    case SortingKey.Hue: span.HueSort(); break;
                    case SortingKey.Saturation: span.SaturationSort(); break;
                    case SortingKey.Luminance: span.LuminanceSort(); break;
                }

                //Place sorted pixels back into the image
                (int x, int y) position = span.StartPosition;
                foreach (Color pixel in span.Pixels)
                {
                    Image.SetPixel(position.x, position.y, pixel);
                    position.x++;
                }
            }

            Console.WriteLine("Sort Complete");
        }
    }
}
