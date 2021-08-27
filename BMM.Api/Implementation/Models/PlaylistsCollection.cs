using System.Collections.ObjectModel;

namespace BMM.Api.Implementation.Models
{
    public class PlaylistsCollection : Document
    {
        public PlaylistsCollection(ObservableCollection<Document> playlists)
        {
            Playlists = playlists;
            DocumentType = DocumentType.PlaylistsCollection;
        }

        public ObservableCollection<Document> Playlists { get; }
    }
}