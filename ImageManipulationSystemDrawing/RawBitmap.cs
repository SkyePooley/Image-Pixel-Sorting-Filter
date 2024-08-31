using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ImageManipulationSystemDrawing;

public class RawBitmap
{
    private Bitmap image;
    private BitmapData imageData;
    private byte[] imageBytes;
    
    private bool _isOpen;
    public bool IsOpen => _isOpen;
    
    public readonly struct Pixel
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;

        public readonly float Luminance;

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            
            Luminance = ((R + G + B) / 3.0f) / 255.0f;
        }

        public static Pixel White()
        {
            return new Pixel(255, 255, 255);
        }

        public static Pixel Black()
        {
            return new Pixel(0, 0, 0);
        }
    }

    public RawBitmap(Bitmap image)
    {
        this.image = Clone<Bitmap>(image);
        Open();
    }
    
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// Read the pixel buffer of the bitmap into a byte array.
    /// </summary>
    private void Open()
    {
        // Get raw data from the bitmap image.
        // Returns a region of memory with all image pixels in order.
        // Each pixel colour component is one 8-bit byte, hence 24 bit per pixel rgb
        // Unfortunately the colour components are stores in order B, G, R.
        imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        
        // We could work with the imageData directly using pointer arithmetic but C# wouldn't like that.
        // It is better if we copy this data to an array which can be operated on safely.
        
        // Create a byte array. Stride is the length in bytes of a row. Needs length enough for all rows.
        imageBytes = new byte[Math.Abs(imageData.Stride) * image.Height];
        
        // Scan0 is the address of the first byte of the first pixel.
        // Copy all subsequent bytes into the array.
        Marshal.Copy(imageData.Scan0, imageBytes, 0, imageBytes.Length);

        _isOpen = true;
        image.UnlockBits(imageData);
    }

    /// <summary>
    /// Overwrite the bitmap pixel buffer with data from the byte array.
    /// </summary>
    /// <returns>Updated Bitmap</returns>
    public Bitmap CloseAndReturn()
    {
        imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        Marshal.Copy(imageBytes, 0, imageData.Scan0, imageBytes.Length);
        image.UnlockBits(imageData);
        _isOpen = false;
        return Clone<Bitmap>(image);
    }

    private int GetPixelAddress(int row, int col)
    {
        return ((row * imageData.Stride) + col*3);
    }

    public Pixel GetPixel(int row, int col)
    {
        if (!_isOpen)
            Open();
        int pixelAddress = GetPixelAddress(row, col);
        return new Pixel(
            imageBytes[pixelAddress + 2],
            imageBytes[pixelAddress + 1],
            imageBytes[pixelAddress]);
    }

    public void SetPixel(int row, int col, Pixel colour)
    {
        if (!_isOpen)
            Open();
        int pixelAddress = GetPixelAddress(row, col);
        imageBytes[pixelAddress + 2] = colour.R;
        imageBytes[pixelAddress + 1] = colour.G;
        imageBytes[pixelAddress + 0] = colour.B;
    }
}