using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models;

public class Achievement
{
    public Achievement(string id,
        bool hasAchieved,
        bool hasAcknowledged,
        string url,
        string title,
        string description,
        int? trackId,
        string actionUrl,
        string actionText)
    {
        Id = id;
        HasAchieved = hasAchieved;
        HasAcknowledged = hasAcknowledged;
        Url = url;
        Title = title;
        Description = description;
        TrackId = trackId;
        ActionUrl = actionUrl;
        ActionText = actionText;
    }

    public string Id { get; }
    public bool HasAchieved { get; }
    public bool HasAcknowledged { get; }
    public string Url { get; }
    public string Title { get; }
    public string Description { get; }
    public int? TrackId { get; }
    public string ActionUrl { get; }
    public string ActionText { get; }
}