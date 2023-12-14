using System.Collections.Concurrent;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using BMM.Core.Extensions;
using Microsoft.Maui.ApplicationModel;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.CustomViews
{
    public class BmmWebView : WebView, IValueCallback
    {
        private const string Error = "error";
        private readonly ConcurrentQueue<string> _scriptsToExecute = new ConcurrentQueue<string>();
        private readonly SemaphoreSlim _evaluationSemaphore = new SemaphoreSlim(1);
        private WebViewClient _client;
        private TaskCompletionSource<string> _taskSource;

        public bool ShouldInterceptTouch { get; set; }
        public bool IsClientSet { get; set; }
        public bool IsInitialPageLoaded { get; set; }
        public bool ShouldExecuteScript { get; set; }

        protected BmmWebView(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference,
            transfer)
        {
        }

        public BmmWebView(Context context) : base(context)
        {
        }

        public BmmWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public BmmWebView(Context context, IAttributeSet attrs, int defStyleAttr) : base(
            context,
            attrs,
            defStyleAttr)
        {
        }

        public BmmWebView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(
            context,
            attrs,
            defStyleAttr,
            defStyleRes)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            RequestDisallowInterceptTouchEvent(ShouldInterceptTouch);
            base.OnTouchEvent(e);
            return true;
        }

        public override void SetWebViewClient(WebViewClient client)
        {
            base.SetWebViewClient(client);
            _client = client;
            IsClientSet = true;
        }

        /// <summary>
        /// GetWebViewClient does not exists prior to API 26.
        /// </summary>
        public override WebViewClient WebViewClient => _client;

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
            _taskSource = new TaskCompletionSource<string>();

            Platform.CurrentActivity?.RunOnUiThread(
                () =>
                {
                    EvaluateJavascript(script, this);
                });

            string result = await _taskSource.Task;

            if (!result.ToLower().Contains(Error))
                return true;
            
            ReloadPageAfterJsException();
            return false;
        }

        protected virtual void ReloadPageAfterJsException()
        {
            IsInitialPageLoaded = false;
            ShouldExecuteScript = false;
            Reload();
        }

        public void OnReceiveValue(Object value)
            => _taskSource?.TrySetResult(value?.ToString());
    }
}