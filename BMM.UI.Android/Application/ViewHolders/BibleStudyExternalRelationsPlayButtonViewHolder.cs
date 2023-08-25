using Android.Runtime;
using Android.Views;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using Com.Airbnb.Lottie;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class BibleStudyExternalRelationsPlayButtonViewHolder : MvxRecyclerViewHolder
    {
        private LottieAnimationView _playAnimationView;
        private bool _shouldShouldShowPlayAnimation;
        private ImageView _playIcon;

        public BibleStudyExternalRelationsPlayButtonViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected BibleStudyExternalRelationsPlayButtonViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        private void Bind()
        {
            _playAnimationView = ItemView.FindViewById<LottieAnimationView>(Resource.Id.PlayAnimationView);
            _playIcon = ItemView.FindViewById<ImageView>(Resource.Id.PlayIcon);

            var set = this.CreateBindingSet<BibleStudyExternalRelationsPlayButtonViewHolder, IBibleStudyExternalRelationPO>();
            
            set.Bind(this)
                .For(v => v.ShouldShowPlayAnimation)
                .To(v => v.ShouldShowPlayAnimation);
                
            set.Apply();
        }

        public bool ShouldShowPlayAnimation
        {
            get => _shouldShouldShowPlayAnimation;
            set
            {
                _shouldShouldShowPlayAnimation = value;
                
                if (_shouldShouldShowPlayAnimation)
                    ShowPlayAnimation();
            }
        }

        private void ShowPlayAnimation()
        {
            _playAnimationView.Visibility = ViewStates.Visible;
            _playIcon.Visibility = ViewStates.Invisible;
        }
    }
}