using Com.Airbnb.Lottie;

namespace BMM.UI.iOS.Utils
{
    public static class LottieUtils
    {
        /// <summary>
        /// Creates a view for the Lottie animation with the given file name (without ".json") in the main bundle.
        /// </summary>
        public static CompatibleAnimationView AnimationNamed(string name)
            => new CompatibleAnimationView(CompatibleAnimation.Named(name));

        public static void SetLooping(this CompatibleAnimationView animationView, bool loop)
            => animationView.SetLoopMode(loop ? CompatibleLottieLoopMode.Loop : CompatibleLottieLoopMode.PlayOnce, 0);
    }
}
