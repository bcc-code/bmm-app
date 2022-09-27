namespace BMM.UI.iOS.NewMediaPlayer.Interfaces
{
    public interface ICacheAVPlayerItemLoaderFactory
    {
        ICacheAVPlayerItemLoader Create(string uniqueKey);
    }
}