using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageTo3mfTopograph_Application.Sources
{
    public static class ImageLoader
    {
        public static BitmapImage? inputImage { get; set; }

        public static List<bool[,]>? colorMasks { get; set; }
        public static double inputHeight { get; set; } = 30.0;
        public static double inputScale { get; set; } = 1.0;
        public static int maxLayerCount { get; set; } = 4;

        public static void SaveMasksToDirectory(List<bool[,]> masks, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < masks.Count; i++)
            {
                bool[,] mask = masks[i];
                int width = mask.GetLength(0);
                int height = mask.GetLength(1);

                WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                byte[] pixels = new byte[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = mask[x, y] ? (byte)255 : (byte)0;
                    }
                }

                wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, width, 0);

                string filePath = Path.Combine(folderPath, $"mask_{i}.png");

                using FileStream fs = new FileStream(filePath, FileMode.Create);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wb));
                encoder.Save(fs);
            }
        }

    }
}
