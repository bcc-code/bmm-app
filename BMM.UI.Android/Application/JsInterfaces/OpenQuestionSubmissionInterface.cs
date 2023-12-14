using Android.Content;
using Android.Webkit;
using BMM.Core.Constants;
using Java.Interop;

namespace BMM.UI.Droid.Application.JsInterfaces;

public class OpenQuestionSubmissionInterface : Java.Lang.Object
{
    private readonly Action<string> _actionToInvoke;

    public OpenQuestionSubmissionInterface(Action<string> actionToInvoke)
    {
        _actionToInvoke = actionToInvoke;
    }

    [JavascriptInterface]
    [Export(JSConstants.OpenQuestionSubmission)]
    public void OpenQuestionSubmission()
    {
        _actionToInvoke.Invoke(string.Empty);
    }
}