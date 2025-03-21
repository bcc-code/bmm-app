using CarPlay;

namespace BMM.UI.iOS;

[Register(nameof(CarPlaySceneDelegate))]
public class CarPlaySceneDelegate : CPTemplateApplicationSceneDelegate
{
    private CPInterfaceController _interfaceController;

    public override void DidConnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
    {
        _interfaceController = interfaceController;
    }

    public override void DidDisconnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
    {
        _interfaceController.Dispose();
    }
}