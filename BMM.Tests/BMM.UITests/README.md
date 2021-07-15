# UI tests

## Running locally
- Run the iOS and Android app which are pointing against the production API
- Just have one Android device/simulator started/connected
- For the iOS tests the connected device or the default simulator will get used
  - You can change it with the `.DeviceIdentifier("")` setting inside of `AppInitializer.cs` in the `StartApp` method. 
  - To get a list of all simulators run `xcrun simctl list` in your terminal
- If you use a iOS simulator disable the "I/O" -> "Keyboard" -> "Connect Hardware Keyboard" setting
- Replace the credentials inside of the TestSecrets.cs file to match our test user
- Run the tests inside of your IDE

## Problems running the tests
- Try to update the "Xamarin.UITest" and the "Xamarin.TestCloud.Agent" packages
- Remove and clean the app and app-data