using MvvmCross.ViewModels;

namespace BMM.Core.Interactions.Base
{
    public interface IBmmInteraction : IMvxInteraction
    {
        void Raise();
    }

    public interface IBmmInteraction<T> : IMvxInteraction<T>
    {
        void Raise(T request);
    }
}