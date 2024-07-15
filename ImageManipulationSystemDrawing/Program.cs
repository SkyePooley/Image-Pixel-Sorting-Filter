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
                var processor = new ImageProcessor(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\Properties\demoSmall.png");
                // processor.ContrastMask(0.4f);
                // processor.SortAll();
                processor.ContrastSort(0.3f);
                processor.SaveTo(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\output.png");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}