using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageTo3mfTopograph_Application.Sources
{
    public static class ImageAnalyzer
    {
        public static List<bool[,]> GetColorMasks(int maxGroups = 4, int tolerance = 40)
        {
            if (ImageLoader.inputImage == null)
                return new List<bool[,]>();

            var image = ImageLoader.inputImage;
            WriteableBitmap wb = new WriteableBitmap(image);
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            wb.CopyPixels(pixels, stride, 0);

            List<Color> clusters = new();
            List<bool[,]> masks = new();

            // Prepare masks for each cluster
            for (int i = 0; i < maxGroups; i++)
                masks.Add(new bool[width, height]);

            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int index = rowOffset + x * 4;

                    byte b = pixels[index + 0];
                    byte g = pixels[index + 1];
                    byte r = pixels[index + 2];
                    byte a = pixels[index + 3];

                    if (a == 0) continue; // Skip transparent

                    Color current = Color.FromArgb(255, r, g, b);

                    // Check against existing clusters
                    int clusterIndex = -1;
                    for (int i = 0; i < clusters.Count; i++)
                    {
                        if (AreColorsSimilar(current, clusters[i], tolerance))
                        {
                            clusterIndex = i;
                            break;
                        }
                    }

                    if (clusterIndex == -1 && clusters.Count < maxGroups)
                    {
                        clusters.Add(current);
                        clusterIndex = clusters.Count - 1;
                    }

                    if (clusterIndex != -1)
                        masks[clusterIndex][x, y] = true;
                }
            }

            // Trim unused masks
            return masks.Take(clusters.Count).ToList();
        }

        private static bool AreColorsSimilar(Color c1, Color c2, int tolerance)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            return dr * dr + dg * dg + db * db <= tolerance * tolerance;
        }
    }
}
