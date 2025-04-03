using System.Diagnostics.CodeAnalysis;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class BrowseLayoutCreator : IBrowseLayoutCreator
{
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        await Task.CompletedTask;
        var browseListTemplate = new CPListTemplate("Browse", []);
        browseListTemplate.TabTitle = "Browse";
        browseListTemplate.TabImage = UIImage.FromBundle("icon_browse".ToNameWithExtension());
        return browseListTemplate;
    }
}