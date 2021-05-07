using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(DownloadButton)), DesignTimeVisible(true)]
    public class DownloadButton : UIButton
    {
        private CAShapeLayer _circleLayer;
        private DownloadButtonState _currentState;
        private UIImage _downloadedImage;
        private float _downloadProgress;
        private bool _isDownloaded;
        private bool _isDownloading;
        private string _label;
        private UIImage _normalStateImage;

        public DownloadButton()
        {
            InitUi();
        }

        public DownloadButton(NSCoder coder) : base(coder)
        {
            InitUi();
        }

        public DownloadButton(CGRect frame) : base(frame)
        {
            InitUi();
        }

        protected internal DownloadButton(IntPtr handle) : base(handle)
        {
            InitUi();
        }

        protected DownloadButton(NSObjectFlag t) : base(t)
        {
            InitUi();
        }

        private enum DownloadButtonState
        {
            NotDownloaded,
            Downloading,
            Downloaded
        }

        public UIImage DownloadedImage
        {
            get => _downloadedImage;
            set
            {
                _downloadedImage = value;
                UpdateCurrentState();
            }
        }

        public float DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                _downloadProgress = value;
                if (_circleLayer != null)
                    _circleLayer.StrokeEnd = value;
            }
        }

        public bool IsDownloaded
        {
            get => _isDownloaded;
            set
            {
                if (_isDownloaded == value)
                    return;

                _isDownloaded = value;
                UpdateCurrentState();
            }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                if (_isDownloading == value)
                    return;

                _isDownloading = value;
                UpdateCurrentState();
            }
        }

        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                UpdateCurrentState();
            }
        }

        public UIImage NormalStateImage
        {
            get => _normalStateImage;
            set
            {
                _normalStateImage = value;
                UpdateCurrentState();
            }
        }

        private void InitUi()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            Layer.MasksToBounds = true;
            ClipsToBounds = false;
            UpdateCurrentState();
        }

        private void RenderDownloadState(UIImage stateImage)
        {
            SetImage(stateImage, UIControlState.Normal);
            SetTitle("", UIControlState.Normal);
            RemoveConstraints(Constraints);
            HeightAnchor.ConstraintEqualTo(40).Active = true;
            WidthAnchor.ConstraintEqualTo(40).Active = true;
            ImageEdgeInsets = UIEdgeInsets.Zero;
            ContentEdgeInsets = UIEdgeInsets.Zero;
            LayoutIfNeeded();
            Layer.CornerRadius = Frame.Width / 2;
        }

        private void RenderNormalState()
        {
            SetImage(NormalStateImage, UIControlState.Normal);
            SetTitle(Label, UIControlState.Normal);
            ImageEdgeInsets = new UIEdgeInsets(0, -6, 0, 0);
            ContentEdgeInsets = new UIEdgeInsets(8, 16, 8, 16);
            RemoveConstraints(Constraints);
            LayoutIfNeeded();
        }

        private void ShowProgress(bool show)
        {
            if (show)
            {
                // the math behind: https://stackoverflow.com/questions/32165027/start-and-end-angle-of-uibezierpath
                const int lineWidth = 2;
                var startAngle = -new nfloat(Math.PI / 2);
                var endAngle = new nfloat(Math.PI * 2 + startAngle);
                var radius = (Frame.Width - lineWidth * 2) / 2;
                var center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
                var circlePath = UIBezierPath.FromArc(center, radius, startAngle, endAngle, true);

                var circleLayer = new CAShapeLayer
                {
                    Path = circlePath.CGPath,
                    FillColor = UIColor.Clear.CGColor,
                    StrokeColor = UIColor.Black.CGColor,
                    LineWidth = lineWidth,
                    StrokeEnd = 0f
                };
                Layer.AddSublayer(circleLayer);
                _circleLayer = circleLayer;
            }
            else
            {
                _circleLayer?.RemoveFromSuperLayer();
                _circleLayer = null;
            }
        }

        private void UpdateCurrentState()
        {
            var newState = (IsDownloaded, IsDownloading) switch
            {
                (true, true) => DownloadButtonState.Downloading,
                (false, true) => DownloadButtonState.Downloading,
                (true, false) => DownloadButtonState.Downloaded,
                (false, false) => DownloadButtonState.NotDownloaded
            };

            _currentState = newState;
            UpdateUiToState();
        }

        private void UpdateUiToState()
        {
            switch (_currentState)
            {
                case DownloadButtonState.NotDownloaded:
                    RenderNormalState();
                    ShowProgress(false);
                    break;
                case DownloadButtonState.Downloading:
                    RenderDownloadState(NormalStateImage);
                    ShowProgress(true);
                    break;
                case DownloadButtonState.Downloaded:
                    RenderDownloadState(DownloadedImage);
                    ShowProgress(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}