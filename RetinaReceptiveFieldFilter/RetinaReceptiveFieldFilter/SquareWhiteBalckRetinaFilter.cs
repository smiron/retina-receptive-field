﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace RetinaReceptiveFieldFilter
{
    public class SquareWhiteBalckRetinaFilter : BaseUsingCopyPartialFilter
    {
         #region Fields

        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        #endregion

        #region Properties

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return _formatTranslations; }
        }

        public int SmallRadius { get; set; }

        public int largeRadius { get; set; }


        #endregion

        #region Instance

        public SquareWhiteBalckRetinaFilter()
        {
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            SmallRadius = 2;
        }

        #endregion

        #region Methods

        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            // processing start and stop X,Y positions
            int startX = rect.Left + 1;
            int startY = rect.Top + 1;
            int stopX = startX + rect.Width - 2;
            int stopY = startY + rect.Height - 2;

            int dstStride = destination.Stride;
            int srcStride = source.Stride;

            int dstOffset = dstStride - rect.Width + 2;
            int srcOffset = srcStride - rect.Width + 2;

            // data pointers
            var src = (byte*)source.ImageData.ToPointer();
            var dst = (byte*)destination.ImageData.ToPointer();

            // allign pointers
            src += srcStride * startY + startX;
            dst += dstStride * startY + startX;


            SmallRadius = 2;
            largeRadius = 4;
         
            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, src++, dst++)
                {
                    if (y > largeRadius && y < (stopY - largeRadius) && x > largeRadius && x < (stopX - largeRadius))
                    {
                        var center = 0;
                        var outer = 0;
                        for (int i = -largeRadius; i < largeRadius; i++)
                        {
                            for (int j = -largeRadius; j < largeRadius; j++)
                            {
                                if ( Math.Abs(i) < SmallRadius && Math.Abs(j) < SmallRadius)
                                {
                                    center = (center + src[j * srcStride + i]) / 2;

                                }
                                else if ( Math.Abs(i) < largeRadius && Math.Abs(j) < largeRadius)
                                {
                                    outer = (outer + src[j * srcStride + i]) / 2;
                                }
                            }
                        }
                        *dst = (byte)Math.Max(0, Math.Min(255, (center - outer + 128)));
                    }
                }
                src += srcOffset;
                dst += dstOffset;
            }

            // draw black rectangle to remove those pixels, which were not processed
            // (this needs to be done for those cases, when filter is applied "in place" -
            // source image is modified instead of creating new copy)
            Drawing.Rectangle(destination, rect, System.Drawing.Color.Black);
        }

        #endregion
    }
}
