using AndroidX.RecyclerView.Widget;
using FFImageLoading;

namespace BMM.UI.Droid.Application.Listeners
{
    public class ImageServiceRecyclerViewScrollListener : RecyclerView.OnScrollListener
    {
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);

            switch (newState)
            {
                case RecyclerView.ScrollStateDragging:
                    ImageService.Instance.SetPauseWork(true);
                    break;

                case RecyclerView.ScrollStateIdle:
                    ImageService.Instance.SetPauseWork(false);
                    break;
            }
        }
    }
}