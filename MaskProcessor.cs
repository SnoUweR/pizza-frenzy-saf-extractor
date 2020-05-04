using System.Drawing;
using System.Drawing.Imaging;

namespace PizzaFrenzySAFExtractor
{
    public abstract class MaskProcessor
    {
        public static void CreatePng(string jpegFilePath, string maskFilePath, string resultFilePath)
        {
            using (var jpegImage = new Bitmap(jpegFilePath)) 
            {
                using (var maskImage = new Bitmap(maskFilePath))
                {
                    using (var resultImage = new Bitmap(jpegImage.Width, jpegImage.Height, PixelFormat.Format32bppArgb))
                    {
                        for (int x = 0; x < jpegImage.Width; x++)
                        {
                            for (int y = 0; y < jpegImage.Height; y++)
                            {
                                var jpegPixel = jpegImage.GetPixel(x, y);
                                var maskPixel = maskImage.GetPixel(x, y);

                                var resultPixel = Color.FromArgb((int) (maskPixel.GetBrightness() * 255), jpegPixel.R, jpegPixel.G,
                                    jpegPixel.B);

                                resultImage.SetPixel(x, y, resultPixel);
                            }
                        }
                        resultImage.Save(resultFilePath, ImageFormat.Png);
                    }
                }
            }
        }
    }
}