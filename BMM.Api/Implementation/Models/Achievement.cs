using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models;

public class Achievement
{
    public string Id { get; set; }
    public bool HasAchieved { get; set; }
    public bool HasAcknowledged { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}