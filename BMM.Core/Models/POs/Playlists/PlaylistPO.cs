using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Playlists
{
    public class PlaylistPO : DocumentPO, ITrackListHolderPO
    {
        public PlaylistPO(Playlist playlist) : base(playlist)
        {
            Playlist = playlist;
        }
        
        public Playlist Playlist { get; }
        public string Title => Playlist.Title;
        public string Cover => Playlist.Cover;
    }
}