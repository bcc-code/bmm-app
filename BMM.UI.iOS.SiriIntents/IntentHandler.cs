using System;
using Foundation;
using Intents;

namespace BMM.UI.iOS.SiriIntents
{
	// This class handles Siri and Shortcuts requests.
	// Media playing operation is redirected to the app, which means, that the rest of the logic 
	// is placed in AppDelegate.cs in HandleIntent method.
	
	[Register ("IntentHandler")]
	public class IntentHandler : INExtension, IINPlayMediaIntentHandling
	{
		protected IntentHandler (IntPtr handle) : base (handle)
		{
		}

		public override NSObject GetHandler (INIntent intent) => this;

		public void HandlePlayMedia(INPlayMediaIntent intent, Action<INPlayMediaIntentResponse> completion)
		{
			var response = new INPlayMediaIntentResponse(INPlayMediaIntentResponseCode.HandleInApp, null);
			completion(response);
		}
	}
}

