using CarPlay;
using Foundation;
using UIKit;
using System.Collections.Generic;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.UI.iOS.Extensions;
using ObjCRuntime;

namespace BMM.UI.iOS
{
    [Register("CarPlaySceneDelegate")]
    public class CarPlaySceneDelegate : CPTemplateApplicationSceneDelegate
    {
        private CPInterfaceController _interfaceController;

        public override void DidConnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = interfaceController;

            // Tab 1: List Template
            var item1 = new CPListItem("Item 1", "Detail 1");
            item1.Handler = (item, block) =>
            {
                Console.WriteLine("Item 1 tapped");
            };

            var section1 = new CPListSection(item1.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
            var listTemplate1 = new CPListTemplate("Tab 1", new[] { section1 });
            listTemplate1.TabTitle = "Home";
            listTemplate1.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());

            // Tab 2: Another List Template
            var item2 = new CPListItem("Item 2", "Detail 2");
            item2.Handler = async (item, block) =>
            {
                Console.WriteLine("Item 2 tapped");
                await Task.Delay(2000);
                block();
            };

            var section2 = new CPListSection(item2.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
            var listTemplate2 = new CPListTemplate("Tab 2", new[] { section2 });
            listTemplate2.TabTitle = "Browse";
            listTemplate2.TabImage = UIImage.FromBundle("icon_browse".ToNameWithExtension());

            // Wrap in CPTabBarTemplate
            var tabBarTemplate = new CPTabBarTemplate(new CPTemplate[] { listTemplate1, listTemplate2 });

            // Set it as the root template
            interfaceController.SetRootTemplate(tabBarTemplate, true);
        }

        public override void DidDisconnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
        {
            _interfaceController = null;
        }
    }
}
