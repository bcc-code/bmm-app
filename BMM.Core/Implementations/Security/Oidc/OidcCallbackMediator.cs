namespace BMM.Core.Implementations.Security.Oidc
{
    public class OidcCallbackMediator
    {
        private static OidcCallbackMediator _instance;

        private OidcCallbackMediator()
        { }

        /// <summary>
        /// Singleton instance of the <see cref="OidcCallbackMediator"/> class.
        /// </summary>
        public static OidcCallbackMediator Instance => _instance ?? (_instance = new OidcCallbackMediator());

        /// <summary>
        /// Method signature required for methods subscribing to the CallbackMessageReceived event.
        /// </summary>
        /// <param name="message">Message that has been received.</param>
        public delegate void MessageReceivedEventHandler(string message);

        /// <summary>
        /// Event listener for subscribing to message received events.
        /// </summary>
        public event MessageReceivedEventHandler CallbackMessageReceived;

        /// <summary>
        /// Send a response message to all listeners.
        /// </summary>
        /// <param name="response">Response message to send to all listeners.</param>
        public void Send(string response)
        {
            CallbackMessageReceived?.Invoke(response);
        }

        /// <summary>
        /// Send a cancellation response message "UserCancel" to all listeners.
        /// </summary>
        public void Cancel()
        {
            Send("UserCancel");
        }
    }
}