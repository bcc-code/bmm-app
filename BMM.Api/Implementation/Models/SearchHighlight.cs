namespace BMM.Api.Implementation.Models;

public class SearchHighlight
{
    public string Id { get; set; }
    public string Text { get; set; }
    public int StartPositionInSeconds { get; set; }
    public int FirstSegmentIndex { get; set; }
    public int LastSegmentIndex { get; set; }
}