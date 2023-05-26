using System;
using System.ComponentModel;
using BMM.Core.Implementations.Device;
using BMM.UI.iOS.Constants;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(DownloadButton)), DesignTimeVisible(true)]
    public class DownloadButton : UIButton
    {
        private CAShapeLayer _circleLayer;
        private DownloadButtonState _currentState = DownloadButtonState.NotDownloaded;
        private UIImage _downloadedImage;
        private float _downloadProgress;
        private bool _isDownloaded;
        private bool _isDownloading;
        private string _label;
        private UIImage _normalStateImage;
        private NSLayoutConstraint _heightConstraint;
        private NSLayoutConstraint _widthConstraint;

        public DownloadButton()
        {
            InitUi();
        }

        public DownloadButton(CGRect frame) : base(frame)
        {
            InitUi();
        }

        public DownloadButton(NSCoder coder) : base(coder)
        {
            InitUi();
        }

        protected internal DownloadButton(IntPtr handle) : base(handle)
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
                UpdateUiForState(_currentState);
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
                _isDownloaded = value;
                UpdateCurrentState();
            }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
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
                UpdateUiForState(_currentState);
            }
        }

        public UIImage NormalStateImage
        {
            get => _normalStateImage;
            set
            {
                _normalStateImage = value;
                UpdateUiForState(_currentState);
            }
        }

        private void InitUi()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            Layer.MasksToBounds = true;
            ClipsToBounds = false;
            _heightConstraint ??= HeightAnchor.ConstraintEqualTo(40);
            _widthConstraint ??= WidthAnchor.ConstraintEqualTo(40);
            UpdateUiForState(_currentState);
        }

        private void EnableSmallSizeConstraints(bool enabled)
        {
            if (_heightConstraint != null)
                _heightConstraint.Active = enabled;

            if (_widthConstraint != null)
                _widthConstraint.Active = enabled;
        }

        private void RenderDownloadState(UIImage stateImage)
        {
            SetImage(stateImage, UIControlState.Normal);
            SetTitle("", UIControlState.Normal);
            EnableSmallSizeConstraints(true);
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
            EnableSmallSizeConstraints(false);
            LayoutIfNeeded();
        }

        private void ShowProgress(bool show)
        {
            if (show)
            {
                _circleLayer?.RemoveFromSuperLayer();
                // the math behind: https://stackoverflow.com/questions/32165027/start-and-end-angle-of-uibezierpath
                const int lineWidth = 2;
                var startAngle = -new nfloat(Math.PI / 2);
                var endAngle = new nfloat(Math.PI * 2 + startAngle);
                var radius = (Frame.Width - lineWidth) / 2;
                var center = new CGPoint(Frame.Width / 2, Frame.Height / 2);
                var circlePath = UIBezierPath.FromArc(center, radius, startAngle, endAngle, true);

                var circleLayer = new CAShapeLayer
                {
                    Path = circlePath.CGPath,
                    FillColor = UIColor.Clear.CGColor,
                    StrokeColor = GetStrokeColor(),
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

        private static CGColor GetStrokeColor()
        {
            var featureSupportInfoService = Mvx.IoCProvider.Resolve<IFeatureSupportInfoService>();

            return featureSupportInfoService.SupportsDarkMode
                ? AppColors.LabelOneColor.GetResolvedColor(AppDelegate.MainWindow.TraitCollection).CGColor
                : UIColor.Black.CGColor;
        }

        public void UpdateCurrentState(bool forceUpdate = false)
        {
            var newState = (IsDownloaded, IsDownloading) switch
            {
                (true, true) => DownloadButtonState.Downloading,
                (false, true) => DownloadButtonState.Downloading,
                (true, false) => DownloadButtonState.Downloaded,
                (false, false) => DownloadButtonState.NotDownloaded
            };

            if (!forceUpdate && _currentState == newState)
                return;

            UpdateUiForState(newState);
            _currentState = newState;
        }

        private void UpdateUiForState(DownloadButtonState newState)
        {
            switch (newState)
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