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

        public void ContrastSort(float threshold)
        {
            ImageProcessor subProcessor = new ImageProcessor(this.Image);
            subProcessor.ContrastMask(threshold);
            Bitmap contrastMask = subProcessor.Image;

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
    }
}
