using MvvmCross.ViewModels;

namespace BMM.Core.Interactions.Base
{
    public class BmmInteraction<T> : MvxInteraction<T>, IBmmInteraction<T>
    {
    }

    public class BmmInteraction : MvxInteraction, IBmmInteraction
    {
    }
}