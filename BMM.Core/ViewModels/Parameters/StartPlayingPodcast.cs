namespace BMM.Core.ViewModels.Parameters
{
    public class StartPlayingPodcast
    {
        public int Id { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Id of the track that is supposed to start playing
        /// </summary>
        public int TrackId { get; set; }
    }
}
