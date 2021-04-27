# BMM Mobile Apps

This repository contains an app to consume the content from BMM on your phone. It uses Xamarin to deliver apps for Android and iOS.

If you have a feature request please submit it at [User Voice](https://uservoice.bcc.no). If you find a bug feel free to create an issue or submit a pull request.

## Getting started

> Warning: if you want to run your locally built version of BMM on your iPhone, you need an Developer license from Apple (~100$/year).
> Even then, the process to get it to work is quite complicated and you should not rely on our help.

todo: add steps on how to get started. Something like:
1. Download and install Visual Studio
2. ....

## Branching model

We use [git flow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) as our branching model. We use the following naming conventions:

* For release branches: `release/*`
  * Where '*' is the version number
* For feature branches: `feature/*`
  * Where '*' is the name of the feature in PascalCase
* For hotfix branches: `hotfix/*`
  * Where '*' is the name of the critical bug in PascalCase

## Creating a pull request

We work with pull requests so other project members can review your code before it appears in the develop branch.

1. Create a feature branch for your feature according to the naming convention above.
2. Commit your changes and push your changes to the repository in the features branch.
3. Create a pull request to the `develop` branch.
4. Link the issue and the pull request

Your pull requests should be small, ideally below 400 lines of code but never more than one feature.

After you submitted a pull request the code will be reviewed by some selected people. After your pull request is submitted, make sure you check back regularly and
have notifications enabled so you get notified whenever someone adds a comment to your pull request. When the reviewers are satisfied and there are no more comments or tasks,
the pull request will be merged. You are not done with implementing the feature before the pull request is merged.

## DOs and DON'Ts

* **DO** include tests when adding new features, either Unit tests where possible
  and UITests to test the platform specific UI code. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up it's
  often better to create new issue than to side track the discussion.
* **DON'T** create big pull request, but always keep them as small and focused
  as possible. Generally you should split up a pull request whenever you can and
  it doesn't break anything. Only one feature per pull request.
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion, so we can agree on a direction before you invest a large amount
  of time.


## App Center

We use [App Center](https://appcenter.ms/apps) for:
1. Gathering crashes: Only by install the packages App Center will automatically save the crashes with the respective report, nevertheless if there is some specific crash that is common, there should be an specific log for it
3. Logging:
    * Errors: It gives more specific information about common errors that may occur
    * Debug: It gives specific information about data that may be recolected while debugging
    * Information: It gives extra information about the different functions and sections that are being use
    * Warnings It gives extra information about the warning that are boing use and how can they affect the application
4. UI Tests


# How to release a version of the BMM App

We have 3 different types of release **environments** for the app these are:
1. *Release*
2. *Release Candidate*
3. *Beta*

We follow [semantic versioning](http://semver.org/) for the versioning of the app.

## The release process

1. Create release branch
    1. Create a branch with the following naming: `release/{version number}`.
    2. Push it to the remote.

2. Add release notes in the release branch
  * Add a file in the **release-notes** folder with the name `release-notes/{version name}.md`
    * Examples:
      * release-notes/1.0.0-BETA1.md (release notes for the *Beta*)
      * release-notes/1.0.0-RC1.md (release notes for the *Release Candidate*)
      * release-notes/1.0.0.md (release notes for the *Release*)

3. Run the build in [VSTS](https://bcc-its.visualstudio.com/BMM/_build?path=%5C&_a=allDefinitions)
    1. Trigger a release for the respective platform and **environment**
    2. Set the `VersionNumber` field to your **version number**.
    3. (Only RC and BETA) Set the `RevisionNumber` field to the REVISION of the Release Candidate or Beta release.

4. Merge release branch
    If you are creating a *Release* to the App Store and Google Play, you can merge the `master` branch and also back into `develop` when the builds have finished.

If you are creating a public release to App Store you also have to go to iTunes connect, copy and paste the release notes into the new release and submit the app for review. For Google Play all of this is done automatically.

## How can I see the traffic from my devices?

**Windows**

Some video tutorials are available here: https://www.telerik.com/videos/fiddler
Docs for iOS: https://docs.telerik.com/fiddler/Configure-Fiddler/Tasks/ConfigureForiOS
Docs for Android: https://docs.telerik.com/fiddler/Configure-Fiddler/Tasks/ConfigureForAndroid

**MacOS**

https://proxyman.io - it's awesome

# Firebase
## How can I test Firebase Analytics (Debug View), Firebase Remote Config and Notifications on DEBUG mode and simulator for iOS?

1. Go to BMM.UI.iOS -> Properties.
2. Go to iOS Bundle Signing and make sure you have set Signing Identity and Provisional Profile correctly. 
3. Set up Custom Entitlements in iOS Bundle Signing. There is Entitlements.plist in iOS project which should be set there.

**To use Notifications, you must do this additional step too:**
1. Make sure you use 'RegisterForNotification(app)' method in AppDelegate.

**To use Firebase Analytics Debug View - follow instruction below:**
[How to set DebugView for Analytics](https://firebase.google.com/docs/analytics/debugview)

## Updating Firebase packages
When the app doesn't start after upgrading a firebase package or throws some weird message clean this folder: `~/Library/Caches/XamarinBuildDownload` 

# Running UITests

> For best results you should run UITests on a physical device

## Preparation

### iOS
For running **iOS** you have to build and deploy the app to the device with the **DEBUG** configuration.

### Android
For running **Android** you have to build and deploy the app with **RELEASE** configuration.
Furthermore, you have to set a valid version number like `android:versionName="1.22.0-alpha"` into `AndroidManifest.xml`
as well in `GlobalConstants.cs`:
```
public const string AppVersion = "1.22.0-alpha";
```

## Common pitfalls
While doing UITests you should only have one device (physical or virtual) connected else you have to define which device you want to use in the AppConfiguration
