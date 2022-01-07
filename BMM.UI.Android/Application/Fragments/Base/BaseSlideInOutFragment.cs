using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.ViewModels.Base;

namespace BMM.UI.Droid.Application.Fragments.Base
{
    public abstract class BaseSlideInOutFragment<TViewModel> : BaseDialogFragment<TViewModel>
        where TViewModel : BaseViewModel
    {
        protected abstract int RootViewId { get; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            FragmentView.Background = new ColorDrawable(Color.Transparent);
            AnimateSlide(true).FireAndForget();
            return FragmentView;
        }

        protected async Task AnimateSlide(bool visible)
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = AnimationUtils.LoadAnimation(Context, visible ? Resource.Animation.slide_in_bottom : Resource.Animation.slide_out_bottom);
            animation!.Duration = ViewConstants.DefaultAnimationDurationInMilliseconds;
            animation.AnimationEnd += AnimationOnAnimationEnd;

            var rootView = FragmentView.FindViewById<View>(RootViewId);
            rootView!.StartAnimation(animation);

            await tcs.Task;

            void AnimationOnAnimationEnd(object sender, Animation.AnimationEndEventArgs e)
            {
                animation = null;
                tcs.SetResult(true);
            }
        }
    }
}