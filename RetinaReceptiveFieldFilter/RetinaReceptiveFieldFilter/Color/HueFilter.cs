﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;


namespace RetinaReceptiveFieldFilter.Color
{
    public class HueFilter : BaseFilter
    {
        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

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
            _formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
            _formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }


        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;
            PixelFormat srcPixelFormat = sourceData.PixelFormat;

            if (
                (srcPixelFormat == PixelFormat.Format24bppRgb) ||
                (srcPixelFormat == PixelFormat.Format32bppRgb) ||
                (srcPixelFormat == PixelFormat.Format32bppArgb))
            {
                int pixelSize = (srcPixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                int srcOffset = sourceData.Stride - width * pixelSize;
                int dstOffset = destinationData.Stride - width;

                // do the job
                var src = (byte*)sourceData.ImageData.ToPointer();
                var dst = (byte*)destinationData.ImageData.ToPointer();


                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src += pixelSize, dst++)
                    {
                        *dst = (byte)(GetHue(src[RGB.R], src[RGB.G], src[RGB.B]) );
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                int pixelSize = (srcPixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
                var srcBase = (byte*)sourceData.ImageData.ToPointer();
                var dstBase = (byte*)destinationData.ImageData.ToPointer();
                int srcStride = sourceData.Stride;
                int dstStride = destinationData.Stride;

                // for each line
                for (int y = 0; y < height; y++)
                {
                    var src = (ushort*)(srcBase + y * srcStride);
                    var dst = (ushort*)(dstBase + y * dstStride);

                    // for each pixel
                    for (int x = 0; x < width; x++, src += pixelSize, dst++)
                    {
                        *dst = (ushort)GetHue(src[RGB.R], src[RGB.G], src[RGB.B]);
                    }
                }
            }

        }


        public byte GetHue(int r, int g, int b)
        {
            if (r == g && g == b)
            {
                return 0;
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
            return (byte)(num4 / 360 * 255);
            
        }
    }
}
