using System.Collections.Generic;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace RetinaReceptiveFieldFilter.Color
{
    public class BrightnessFilter : BaseFilter
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

        public BrightnessFilter()
        {
            // initialize format translation dictionary
            _formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format8bppIndexed;
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
                        *dst = (byte)( GetBrightness(src[RGB.R], src[RGB.G], src[RGB.B]) );
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
                        *dst = (ushort) GetBrightness(src[RGB.R], src[RGB.G], src[RGB.B]);
                    }
                }
            }
        }


        public byte GetBrightness(int r, int g, int b)
        {
            float num = r / 255f;
            float num2 = g/ 255f;
            float num3 = b / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            return (byte) (((num4 + num5) / 2f)*255);
        }
    }
}
