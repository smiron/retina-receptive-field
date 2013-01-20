using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;


namespace RetinaReceptiveFieldFilter.Color
{
    public class HueFilter :  BaseInPlacePartialFilter
    {
           // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return _formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInPlacePartialFilter"/> class.
        /// </summary>
        public HueFilter()
        {
            // initialize format translation dictionary
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format8bppIndexed;
        }


        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            // get pixel size
            int pixelSize = Image.GetPixelFormatSize(image.PixelFormat)/8;

            int startX = rect.Left;
            int startY = rect.Top;  
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;
            int offset = image.Stride - rect.Width*pixelSize;


            // do the job
            var ptr = (byte*)image.ImageData.ToPointer();

            // allign pointer to the first pixel to process
            ptr += (startY * image.Stride + startX * pixelSize);

            for (int y = startY; y < stopY; y++)
            {
                // for each pixel in line
                for (int x = startX; x < stopX; x++, ptr += pixelSize)
                {
                    var hue = (byte)GetHue(ptr[RGB.R], ptr[RGB.G], ptr[RGB.B]);
                    ptr[RGB.R] = hue;
                    ptr[RGB.G] = hue;
                    ptr[RGB.B] = hue;
                }
                ptr += offset;
            }

        }


        public float GetHue(int r, int g, int b)
        {
            if (r == g && g == b)
            {
                return 0f;
            }
            float num = r / 255f;
            float num2 = g / 255f;
            float num3 = b / 255f;
            float num4 = 0f;
            float num5 = num;
            float num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }
            if (num3 > num5)
            {
                num5 = num3;
            }
            if (num2 < num6)
            {
                num6 = num2;
            }
            if (num3 < num6)
            {
                num6 = num3;
            }
            float num7 = num5 - num6;
            if (num == num5)
            {
                num4 = (num2 - num3) / num7;
            }
            else
            {
                if (num2 == num5)
                {
                    num4 = 2f + (num3 - num) / num7;
                }
                else
                {
                    if (num3 == num5)
                    {
                        num4 = 4f + (num - num2) / num7;
                    }
                }
            }
            num4 *= 60f;
            if (num4 < 0f)
            {
                num4 += 360f;
            }
            return num4;
        }
    }
}
