using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using RetinaReceptiveFieldFilter.Color;

namespace RetinaReceptiveFieldFilter
{
    public partial class Prezentation : Form
    {
        private readonly FastSquareWhiteBlackRetinaFilter _fastRf = new FastSquareWhiteBlackRetinaFilter();
        private readonly RoundWhiteBlackRetinaFilter _roundRf = new RoundWhiteBlackRetinaFilter();
        private readonly SquareWhiteBalckRetinaFilter _squareRf = new SquareWhiteBalckRetinaFilter();
        private readonly RoundRedGreenRetinaFilter _roundredGreenRf = new RoundRedGreenRetinaFilter();
        private readonly HueFilter _hueFilter = new HueFilter();
        private int _frames;
        private Bitmap _grayImage;
        private bool _processing;
        private Stopwatch _stopWatch;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        public Prezentation()
        {
            InitializeComponent();
        }


        private void PrezentationLoad(object sender, EventArgs e)
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += VideoSourceNewFrame;
            _videoSource.DesiredFrameRate = 30;
            _videoSource.DesiredFrameSize = new Size(800, 600);
            //timer.Start();
            //_videoSource.Start();


            var patterns = GetPixelTemporalPattern(254);



            var filter = new BrightnessFilter();

            var image = (Bitmap)Image.FromFile(@"C:\Users\IBM_ADMIN\Desktop\CAR.jpg");

            var a = filter.Apply(image);
            var b = _fastRf.Apply(a);

            for (int i = 0; i < 256; i++)
            {
                var c = GetPictureTemporalPattern(i, b);    
                c.Save(@"C:\temp\Patt11\img"+i.ToString("D3")+".jpg");
            }

            


            //drawArea.Image = c;
        }

        private bool[] GetPixelTemporalPattern(int value)
        {
            var result = new bool[256];

            for (int i = 0; i < value; i++)
            {
                result[(int)((256 / (double)value) * i)] = true;
            }

            return result;
        }



        private Bitmap GetPictureTemporalPattern(int value, Bitmap image)
        {
            Bitmap bitmap = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    bitmap.SetPixel(i, j, GetPixelTemporalPattern(bitmap.GetPixel(i, j).B)[value] ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            }

            return bitmap;
        }



        private void VideoSourceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!_processing)
            {
                lock (this)
                {
                    _processing = true;

                    var image = (Bitmap)eventArgs.Frame.Clone();


                    //_hueFilter.ApplyInPlace(image, new Rectangle(325, 225, 200, 200));
                    _roundredGreenRf.ApplyInPlace(image, new Rectangle(325, 225, 200, 200));

                    //_grayImage = Grayscale.CommonAlgorithms.BT709.Apply(eventArgs.Frame);
                    //_fastRf.ApplyInPlace(_grayImage, new Rectangle(325, 225, 200, 200));
                    //drawArea.Image = _grayImage;

                    drawArea.Image = image;
                    _frames++;
                    _processing = false;
                }
            }
        }

        private void PrezentationFormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Stop();
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource = null;
            }
        }


        private void TimerTick(object sender, EventArgs e)
        {
            if (_videoSource == null)
            {
                return;
            }

            if (_stopWatch == null)
            {
                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }
            else
            {
                _stopWatch.Stop();
                float fps = 1000.0f * _frames / _stopWatch.ElapsedMilliseconds;
                cameraFpsLabel.Text = fps.ToString("F2") + " fps";
                _frames = 0;
                _stopWatch.Reset();
                _stopWatch.Start();
            }
        }
    }
}