using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CA1416

namespace ImageManipulationSystemDrawing
{
    internal class ImageProcessor
    {
        private Bitmap Image { get; }

        public ImageProcessor(String path)
        {
            Image = new Bitmap(path);
            Console.WriteLine("Width: " + Image.Width);
            Console.WriteLine("Height: " + Image.Height);
        }

        public ImageProcessor(Bitmap image)
        {
            this.Image = (Bitmap) image.Clone();
        }

        public void SaveTo(String path)
        {
            Image.Save(path);
        }

        public void Divide(int division)
        {
            for (int row = 0; row < Image.Height; row++)
            {
                for (int col = 0; col < Image.Width; col++)
                {
                    Color pixel = Image.GetPixel(col, row);
                    Color newPixel = this.Divide(pixel, division);
                    Image.SetPixel(col, row, newPixel);
                }
            }
        }

        private Color Divide(Color pixel, int division)
        {
            int red = pixel.R / 2;
            int green = pixel.G / 2;
            int blue = pixel.B / 2;
            return Color.FromArgb(red, green, blue);
        }
        
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

        public void ContrastSort(float threshold)
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
                // span.HueSort();
                // span.SaturationSort();
                span.LuminanceSort();
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
