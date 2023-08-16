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
        private readonly IBuildExternalRelationsAction _buildExternalRelationsAction;

        public TrackInfoViewModel(
            IBuildTrackInfoSectionsAction buildTrackInfoSectionsAction,
            IBuildExternalRelationsAction buildExternalRelationsAction)
        {
            _buildTrackInfoSectionsAction = buildTrackInfoSectionsAction;
            _buildExternalRelationsAction = buildExternalRelationsAction;
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
            var externalRelations = await _buildExternalRelationsAction.ExecuteGuarded(Track);
            var items = await _buildTrackInfoSectionsAction.ExecuteGuarded(Track);
            Items.AddRange(externalRelations);
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