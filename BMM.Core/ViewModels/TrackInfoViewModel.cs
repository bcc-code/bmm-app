using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class TrackInfoViewModel : ItemListViewModel, IMvxViewModel<Track>
    {
        private readonly IBuildTrackInfoSectionsAction _buildTrackInfoSectionsAction;

        public TrackInfoViewModel(
            IBuildTrackInfoSectionsAction buildTrackInfoSectionsAction)
        {
            _buildTrackInfoSectionsAction = buildTrackInfoSectionsAction;
        }

        private Track _track;

        public Track Track
        {
            get => _track;
            set => SetProperty(ref _track, value);
        }

        public void Prepare(Track parameter)
        {
            Track = parameter;
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            var items = await _buildTrackInfoSectionsAction.ExecuteGuarded(Track);
            Items.AddRange(items);
        }

        protected override void SaveStateToBundle(IMvxBundle bundle)
        {
            base.SaveStateToBundle(bundle);
            bundle.SaveParameter(Track);
        }

        protected override void ReloadFromBundle(IMvxBundle state)
        {
            base.ReloadFromBundle(state);
            var param = state.RetrieveParameter<Track>();
            Prepare(param);
        }
    }
}