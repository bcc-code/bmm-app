using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models;

public class ProjectRules
{
    public string PageTitle { get; set; }
    public IList<ProjectRulesSection> Sections { get; set; }
}