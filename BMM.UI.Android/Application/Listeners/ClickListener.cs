using Android.Views;

namespace BMM.UI.Droid.Application.Listeners;

public class ClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly Action _onClickAction;
    
    public ClickListener(Action onClickAction)
    {
        _onClickAction = onClickAction;
    }
    
    public void OnClick(View v)
    {
        _onClickAction?.Invoke();
    }
}