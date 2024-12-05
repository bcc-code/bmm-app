namespace BMM.UI.Droid.Application.Constants;

public static class States
{
    public static int[] Pressed => new[]
    {
        Android.Resource.Attribute.StatePressed
    };

    public static int[] Disabled => new[]
    {
        -Android.Resource.Attribute.StateEnabled
    };

    public static int[] Default => new int[]
    {
    };
}