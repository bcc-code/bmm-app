using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.QueueFragment")]
    public class QueueFragment : BaseFragment<QueueViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_queue;

        protected override Color FragmentBaseColor => Resources.GetColor(Resource.Color.player_statusbar);

        private PlayerFragment PlayerFragment => FragmentManager.FindFragmentById(Resource.Id.player_frame) as PlayerFragment;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            view.SetBackgroundColor(Color.White);
            base.OnViewCreated(view, savedInstanceState);
            PlayerFragment?.HidePlayer();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.queue, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_queue_player:
                    CloseQueue();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        internal void CloseQueue()
        {
            var playerFragment = PlayerFragment;
            FragmentManager.PopBackStackImmediate();
            playerFragment.ShowPlayer();
        }
    }
}