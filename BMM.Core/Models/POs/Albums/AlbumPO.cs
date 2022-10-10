using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Albums
{
    public class AlbumPO : DocumentPO, ITrackListHolderPO
    {
        public AlbumPO(Album album) : base(album)
        {
            Album = album;
        }
        
        public Album Album { get; }
        public string Title => Album.Title;
        public string Cover => Album.Cover;
    }
}