using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class ShareTrackCollectionViewModel : BaseViewModel, IMvxViewModel<ITrackCollectionParameter>
    {
        public void Prepare(ITrackCollectionParameter parameter)
        {
        }
    }
}