﻿using System;
using System.Drawing;

namespace ImageManipulationSystemDrawing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var imageProcessor = new ImageProcessor(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\Properties\demoSmall.png");
                //imageProcessor.ContrastMask(0.6f);
                imageProcessor.ContrastSort(0.6f, ImageProcessor.SortingKey.Saturation);
                imageProcessor.SaveTo(@"C:\Users\wmpoo\OneDrive\Programs\ImageManipulationSystemDrawing\ImageManipulationSystemDrawing\output.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}