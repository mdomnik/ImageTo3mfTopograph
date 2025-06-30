using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;

namespace ImageTo3mfTopograph_Application.Sources
{
    public static class ImageLoader
    {
        public static BitmapImage? inputImage { get; set; }

        public static List<bool[,]>? colorMasks { get; set; }
        public static double inputHeight { get; set; } = 30.0;
        public static double inputScale { get; set; } = 1.0;

        public static List<bool[,]> GenerateColorMasks(BitmapImage image)
        {
            
        }
    }
}
