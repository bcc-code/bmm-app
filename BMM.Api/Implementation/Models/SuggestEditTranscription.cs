using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models;

[JsonObject]
public class SuggestEditTranscription
{
    public int SegmentIndex { get; set; }
    public string OriginalText { get; set; }
    public string NewText { get; set; }
}