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

            var filter = new BrightnessFilter();

            var image = (Bitmap)Image.FromFile(@"C:\Users\IBM_ADMIN\Desktop\RAW.jpg");

            var a = filter.Apply(image);

            //var gray = _grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);

            

            var hue = _hueFilter.Apply(image);
            var b = _fastRf.Apply(hue);

            drawArea.Image = b;
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
                    _roundredGreenRf.ApplyInPlace(image,new Rectangle(325, 225, 200, 200));
                    
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