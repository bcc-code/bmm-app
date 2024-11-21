namespace BMM.Core.Models.Player.Lyrics;

public class LyricsLink
{
    public LyricsLink(LyricsLinkType lyricsLinkType, string link)
    {
        LyricsLinkType = lyricsLinkType;
        Link = link;
    }
    
    public LyricsLinkType LyricsLinkType { get; }
    public string Link { get; }

    public static LyricsLink Empty => new(LyricsLinkType.None, string.Empty);
}