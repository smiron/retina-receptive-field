using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace RetinaReceptiveFieldFilter
{
    public partial class Prezentation : Form
    {
        public Prezentation()
        {
            InitializeComponent();
        }


        private Bitmap _grayImage;
        private readonly RoundRetinaRfFilter _roundRf = new RoundRetinaRfFilter();
        private readonly FastSquareRetinaRfFilter _fastRf = new FastSquareRetinaRfFilter();
        private readonly SquareRetinaRfFilter _squareRf = new SquareRetinaRfFilter();
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;
        private Stopwatch _stopWatch;


        private void PrezentationLoad(object sender, EventArgs e)
        {
            //var image = (Bitmap)Image.FromFile(@"C:\Users\IBM_ADMIN\Desktop\1.JPG");
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += VideoSourceNewFrame;
            _videoSource.DesiredFrameRate = 30;
            _videoSource.DesiredFrameSize = new Size(800, 600);
            timer.Start();
            _videoSource.Start();

        }


        private bool _processing;

        void VideoSourceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!_processing)
            {
                lock (this)
                {
                    _processing = true;
                    _grayImage = Grayscale.CommonAlgorithms.BT709.Apply(eventArgs.Frame);
                    var rfImage = _fastRf.Apply(_grayImage);
                    drawArea.Image = rfImage;
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
                float fps = 1000.0f * _videoSource.FramesReceived / _stopWatch.ElapsedMilliseconds;
                cameraFpsLabel.Text = fps.ToString("F2") + " fps";
                _stopWatch.Reset();
            }
        }


    }
}
