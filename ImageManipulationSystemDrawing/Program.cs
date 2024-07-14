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
                var processor = new ImageProcessor(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\Properties\demo.jpg");
                // processor.ContrastMask(0.4f);
                processor.ContrastSort(0.5f);
                processor.SaveTo(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\output.jpg");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}