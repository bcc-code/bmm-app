using System.ComponentModel;
using FFImageLoading.Args;
using FFImageLoading.Cross;
using ErrorEventArgs = FFImageLoading.Args.ErrorEventArgs;

namespace BMM.UI.iOS
{
    [Register(nameof(BmmCachedImageView)), DesignTimeVisible(true)]
    public class BmmCachedImageView : MvxCachedImageView
    {
        public BmmCachedImageView()
        {
        }

        public BmmCachedImageView(IntPtr handle) : base(handle)
        {
        }

        public BmmCachedImageView(CGRect frame) : base(frame)
        {
        }
        
        public event EventHandler ImageChanged;
        public bool IsError { get; private set; }

        public override UIImage? Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                ImageChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public override void WillMoveToWindow(UIWindow? window)
        {
            base.WillMoveToWindow(window);
            
            if (window is null)
            {
                OnError -= HandleError;
                OnSuccess -= HandleSuccess;
            }
            else
            {
                OnError += HandleError;
                OnSuccess += HandleSuccess;
            }
        }

        private void HandleSuccess(object? sender, SuccessEventArgs e) => IsError = false;
        private void HandleError(object? sender, ErrorEventArgs e) => IsError = true;
    }
}