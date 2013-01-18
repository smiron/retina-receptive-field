using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace RetinaReceptiveFieldFilter
{
    public class FastSquareRetinaRfFilter : BaseUsingCopyPartialFilter
    {
        #region Fields

        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        private int[] _centerArray;
        private int[] _outerArray;

        #endregion

        #region Properties

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return _formatTranslations; }
        }

        public int SmallRadius { get; set; }

        public int LargeRadius { get; set; }


        #endregion

        #region Instance

        public FastSquareRetinaRfFilter()
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
            
            if (_centerArray == null)
            {
                var list = GetPositions(srcStride, 1);
                list.AddRange(GetPositions(srcStride, 2));
                _centerArray = list.ToArray();
            }

            if (_outerArray == null)
            {
                var list = GetPositions(srcStride, 3);
                list.AddRange(GetPositions(srcStride, 4));
                list.AddRange(GetPositions(srcStride, 5));
                _outerArray = list.ToArray();
            }
            
             SmallRadius = 2;
            LargeRadius = 10;

            
            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, src++, dst++)
                {
                    if (y > LargeRadius && y < (stopY - LargeRadius) && x > LargeRadius && x < (stopX - LargeRadius))
                    {
                        int center = 0;
                        for (int i = 0; i < _centerArray.Length; i++)
                        {
                            center += src[_centerArray[i]];
                        }
                        
                        int outer = 0;
                        for (int i = 0; i < _outerArray.Length; i++)
                        {
                            outer += src[_outerArray[i]];
                        }

                        *dst = (byte)Math.Max(0, Math.Min(255, (center / _centerArray.Length - outer / _outerArray.Length + 128)));
                    }
                }
                src += srcOffset;
                dst += dstOffset;
            }

            // draw black rectangle to remove those pixels, which were not processed
            // (this needs to be done for those cases, when filter is applied "in place" -
            // source image is modified instead of creating new copy)
            Drawing.Rectangle(destination, rect, Color.Black);
        }

        private List<int> GetPositions(int stride, int row)
        {

            var result = new List<int>();
            int lRadius = row;
            int sRadius = row-1;


            for (int i = -lRadius; i < lRadius; i++)
            {
                for (int j = -lRadius; j < lRadius; j++)
                {
                    if (Math.Abs(i) < sRadius && Math.Abs(j) < sRadius)
                    {

                    }
                    else if (Math.Abs(i) < lRadius && Math.Abs(j) < lRadius)
                    {
                        result.Add(stride * j + i);
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
