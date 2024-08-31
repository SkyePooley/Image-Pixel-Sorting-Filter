using System;
using System.Drawing;

namespace ImageManipulationSystemDrawing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var imageProcessor = new ImageProcessor(@"C:\Users\wmpoo\Sync\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\Properties\demoSmall.png");
                imageProcessor.ContrastMask(0.6f);
                // imageProcessor.ContrastSort(0.6f, ImageProcessor.SortingKey.Hue);
                imageProcessor.SaveTo(@"C:\Users\wmpoo\Sync\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\output.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}