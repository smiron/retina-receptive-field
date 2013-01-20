using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace RetinaReceptiveFieldFilter
{
    public partial class Prezentation : Form
    {
        private readonly FastSquareRetinaRfFilter _fastRf = new FastSquareRetinaRfFilter();
        private readonly RoundRetinaRfFilter _roundRf = new RoundRetinaRfFilter();
        private readonly SquareRetinaRfFilter _squareRf = new SquareRetinaRfFilter();
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
            timer.Start();
            _videoSource.Start();
        }

        private void VideoSourceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!_processing)
            {
                lock (this)
                {
                    _processing = true;
                    _grayImage = Grayscale.CommonAlgorithms.BT709.Apply(eventArgs.Frame);
                    _fastRf.ApplyInPlace(_grayImage, new Rectangle(325, 225, 200, 200));
                    drawArea.Image = _grayImage;
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