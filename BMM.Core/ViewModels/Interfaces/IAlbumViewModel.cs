using BMM.Api.Implementation.Models;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IAlbumViewModel : ITrackListViewModel, IDocumentsViewModel
    {
        Album Album { get; set; }
    }
}