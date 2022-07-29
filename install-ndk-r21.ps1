$sdkRoot = "C:\Android\android-sdk"
$sdkManager = "$sdkRoot\cmdline-tools\latest\bin\sdkmanager.bat"

Install-AndroidSDKPackages -AndroidSDKManagerPath $sdkManager `
                           -AndroidSDKRootPath $sdkRoot `
                           -AndroidPackages "ndk;21.4.7075529"