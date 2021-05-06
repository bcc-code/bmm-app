using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(DownloadButton)), DesignTimeVisible(true)]
    public class DownloadButton : UIButton
    {
        private ButtonState _currentState;
        private bool _isDownloading;
        private bool _isDownloaded;

        public string Label { get; set; }

        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                _isDownloading = value;
                UpdateCurrentState();
                UpdateUiToState();
            }
        }

        public bool IsDownloaded
        {
            get => _isDownloaded;
            set
            {
                _isDownloaded = value;
                UpdateCurrentState();
                UpdateUiToState();
            }
        }

        public UIImage NormalStateImage { get; set; }
        public UIImage DownloadedImage { get; set; }

        private void UpdateCurrentState()
        {
            if (IsDownloaded)
                _currentState = ButtonState.Downloaded;
            else if (IsDownloading)
                _currentState = ButtonState.Downloading;
            else
                _currentState = ButtonState.NotDownloaded;
        }
        private void UpdateUiToState()
        {
            switch (_currentState)
            {
                case ButtonState.NotDownloaded:
                    SetImage(NormalStateImage, UIControlState.Normal);
                    SetTitle(Label, UIControlState.Normal);
                    ImageEdgeInsets = new UIEdgeInsets(0, -6, 0, 0);
                    ContentEdgeInsets = new UIEdgeInsets(8, 16, 8, 16);
                    RemoveConstraints(Constraints);
                    LayoutIfNeeded();
                    break;
                case ButtonState.Downloading:
                    SetImage(DownloadedImage, UIControlState.Normal);
                    SetTitle("", UIControlState.Normal);
                    RemoveConstraints(Constraints);
                    HeightAnchor.ConstraintEqualTo(40).Active = true;
                    WidthAnchor.ConstraintEqualTo(40).Active = true;
                    ImageEdgeInsets = UIEdgeInsets.Zero;
                    ContentEdgeInsets = UIEdgeInsets.Zero;
                    LayoutIfNeeded();
                    Layer.CornerRadius = Frame.Width / 2;
                    break;
                case ButtonState.Downloaded:
                    SetImage(DownloadedImage, UIControlState.Normal);
                    SetTitle("", UIControlState.Normal);
                    RemoveConstraints(Constraints);
                    HeightAnchor.ConstraintEqualTo(40).Active = true;
                    WidthAnchor.ConstraintEqualTo(40).Active = true;
                    ImageEdgeInsets = UIEdgeInsets.Zero;
                    ContentEdgeInsets = UIEdgeInsets.Zero;
                    LayoutIfNeeded();
                    Layer.CornerRadius = Frame.Width / 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public DownloadButton()
        {
            InitUi();
        }

        public DownloadButton(NSCoder coder) : base(coder)
        {
            InitUi();
        }

        protected DownloadButton(NSObjectFlag t) : base(t)
        {
            InitUi();
        }

        protected internal DownloadButton(IntPtr handle) : base(handle)
        {
            InitUi();
        }

        public DownloadButton(CGRect frame) : base(frame)
        {
            InitUi();
        }

        private void InitUi()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            Layer.MasksToBounds = true;
            ClipsToBounds = false;
        }

        private enum ButtonState
        {
            NotDownloaded,
            Downloading,
            Downloaded
        }
    }
}