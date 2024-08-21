using System.Collections.Concurrent;
using System.Diagnostics;
using BMM.Core.Extensions;
using WebKit;

namespace BMM.UI.iOS.CustomViews
{
    public class BmmWebView : WKWebView
    {
        private readonly ConcurrentQueue<string> _scriptsToExecute = new ConcurrentQueue<string>();
        private readonly SemaphoreSlim _evaluationSemaphore = new SemaphoreSlim(1);

        public BmmWebView(NSCoder coder) : base(coder)
        {
        }

        protected BmmWebView(NSObjectFlag t) : base(t)
        {
        }

        protected internal BmmWebView(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        public BmmWebView(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration)
        {
        }

        public override UIEdgeInsets SafeAreaInsets => UIEdgeInsets.Zero;
        public bool IsInitialPageLoaded { get; set; }
        public bool ShouldExecuteScript { get; set; }

        public void SafeAddScriptMessageHandler(IWKScriptMessageHandler handler, string eventName)
        {
            RemoveScriptMessageHandler(eventName);
            AddScriptMessageHandler(handler, eventName);
        }

        public virtual void AddScriptMessageHandler(IWKScriptMessageHandler handler, string eventName)
        {
            Configuration.UserContentController.AddScriptMessageHandler(handler, eventName);
        }

        public virtual void RemoveScriptMessageHandler(string eventName)
        {
            Configuration.UserContentController.RemoveScriptMessageHandler(eventName);
        }

        public void EnqueueScriptToExecute(string script)
        {
            _scriptsToExecute.Enqueue(script);
            RunEvaluation()
                .FireAndForget();
        }

        public virtual async Task RunEvaluation()
        {
            if (!ShouldExecuteScript)
                return;

            try
            {
                await _evaluationSemaphore.WaitAsync();
                ShouldExecuteScript = false;
                while (_scriptsToExecute.TryDequeue(out string script))
                {
                    if (!await ExecuteEvaluation(script))
                        return;
                }

                ShouldExecuteScript = true;
            }
            finally
            {
                _evaluationSemaphore.Release();
            }
        }

        protected virtual async Task<bool> ExecuteEvaluation(string script)
            => await PassScriptForBrowserEvaluation(script);

        protected async Task<bool> PassScriptForBrowserEvaluation(string script)
        {
            var taskSource = new TaskCompletionSource<(bool Succeded, NSObject Result)>();
            BeginInvokeOnMainThread(
                async () =>
                {
                    NSObject taskResult = null;
                    bool successful = false;
                    try
                    {
                        taskResult = await EvaluateJavaScriptAsync(script);
                        successful = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message, e);
                    }
                    finally
                    {
                        taskSource.SetResult((successful, taskResult));
                    }
                });
            (bool succeeded, _) = await taskSource.Task;

            if (!succeeded)
            {
                ReloadPageAfterJsException();
            }

            return succeeded;
        }

        protected virtual void ReloadPageAfterJsException()
        {
            IsInitialPageLoaded = false;
            ShouldExecuteScript = false;
            InvokeOnMainThread(() => Reload());
        }
    }
}