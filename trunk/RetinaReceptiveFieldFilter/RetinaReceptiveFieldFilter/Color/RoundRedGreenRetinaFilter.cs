using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;


namespace RetinaReceptiveFieldFilter.Color
{
    public class RoundRedGreenRetinaFilter : BaseUsingCopyPartialFilter
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
        /// Initializes a new instance of the <see cref="RoundRedGreenRetinaFilter"/> class.
        /// </summary>
        public RoundRedGreenRetinaFilter()
        {
            // initialize format translation dictionary
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
        }


        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            // get pixel size
            int srcPixelSize = Image.GetPixelFormatSize(source.PixelFormat) / 8;
            int dstPixelSize = Image.GetPixelFormatSize(destination.PixelFormat) / 8;

            int startX = rect.Left;
            int startY = rect.Top;
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;
            int srcOffset = source.Stride - rect.Width * srcPixelSize;
            int dstOffset = destination.Stride - rect.Width * dstPixelSize;

            // do the job
            var ptr = (byte*)source.ImageData.ToPointer();
            var dst = (byte*)destination.ImageData.ToPointer();

            // allign pointer to the first pixel to process
            ptr += (startY * source.Stride + startX * srcPixelSize);
            dst += (startY * destination.Stride + startX * dstPixelSize);

            int srcStride = source.Stride;

            const int largeRadius = 4;
            const int smallRadiusSquare = 4;
            const int largeRadiusSquare = largeRadius * largeRadius;


            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel in line
                for (int x = startX; x < stopX; x++, ptr += srcPixelSize, dst += dstPixelSize)
                {
                    var center = 0;
                    var outer = 0;
                    for (int i = -largeRadius; i < largeRadius; i++)
                    {
                        for (int j = -largeRadius; j < largeRadius; j++)
                        {
                            int r = (i * i + j * j);
                            if (r < (smallRadiusSquare))
                            {
                                center = (center + ptr[j * srcStride * srcPixelSize + i * srcPixelSize]) / 2;

                            }
                            else if (r < (largeRadiusSquare))
                            {
                                outer = (outer + ptr[j * srcStride * srcPixelSize + i * srcPixelSize]) / 2;
                            }
                        }
                    }

                    var res = (byte)Math.Max(0, Math.Min(255, (center - outer + 128)));
                    dst[RGB.R] = res;
                    dst[RGB.G] = res;
                    dst[RGB.B] = res;



                }
                ptr += srcOffset;
                dst += dstOffset;
            }

            Drawing.Rectangle(destination, rect, System.Drawing.Color.Black);
        }



    }
}
