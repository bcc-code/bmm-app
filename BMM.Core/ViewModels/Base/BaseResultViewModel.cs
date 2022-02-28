using System.Threading.Tasks;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Base
{
    public abstract class BaseResultViewModel<TParam, TResult> 
        : BaseViewModel<TParam>,
          IMvxViewModel<TParam, TResult>
    {
        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing
                && CloseCompletionSource != null
                && !CloseCompletionSource.Task.IsCompleted
                && !CloseCompletionSource.Task.IsFaulted)
                CloseCompletionSource?.TrySetCanceled();

            base.ViewDestroy(viewFinishing);
        }
    }
}