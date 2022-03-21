#!/bin/bash

versionInfo=$VERSIONINFO
branch="$(Build.SourceBranch)"

## These two lines can be used to run the script locally
#versionInfo="\"1.12.4.3.0.0\""
#branch="refs/heads/main"

# major.minor.patch.beta.alpha.hotfix-beta
# develop -> 1.1.0-alpha1      1.0.0.0.1.0
# develop -> 1.1.0-alpha2      1.0.0.0.2.0
# release -> 1.1.0-beta1       1.0.0.1.2.0
# develop -> 1.1.0-alpha3      1.0.0.1.3.0
# release -> 1.1.0-beta2       1.0.0.2.3.0  (minor++, increment beta)
# main    -> 1.1.0             1.1.0.0.0.0  (increment minor, reset patch, beta, alpha)    VV1
# develop -> 1.2.0-alpha1      1.1.0.0.1.0  (minor++, increment alpha)
# hotfix  -> 1.1.1-beta1       1.1.0.0.1.1  (patch++, increment hotfix-beta)
# main    -> 1.1.1             1.1.1.0.1.0 (increment patch, reset hotfix-beta)      VV2
# develop -> 1.2.0-alpha2      1.1.1.0.2.0 (minor++, increment alpha)
# hotfix  -> 1.1.2-beta1       1.1.1.0.2.1 (patch++, increment hotfix-beta)
# main    -> 1.1.2             1.1.2.0.2.0  (increment patch, reset hotfix-beta)      VV2

echo "Parse version number for branch $branch: $versionInfo"

# We have to do it twice to remove all quotation marks
versionInfo=${versionInfo/\"}
versionInfo=${versionInfo/\"}

major=$(echo $versionInfo | cut -d'.' -f1)
minor=$(echo $versionInfo | cut -d'.' -f2)
patch=$(echo $versionInfo | cut -d'.' -f3)
beta=$(echo $versionInfo | cut -d'.' -f4)
alpha=$(echo $versionInfo | cut -d'.' -f5)
hotfixbeta=$(echo $versionInfo | cut -d'.' -f6)

printMajor=$major
printMinor=$minor
printPatch=$patch

printRevision=""

if [[ "$branch" == "refs/heads/release/"* ]]
  then
    printMinor=$(($printMinor + 1))
    printPatch=0
    beta=$(($beta + 1))
    printRevision="-beta$beta"
    hotfixbeta=0

elif [[ "$branch" == "refs/heads/hotfix/"* ]]
  then
    printPatch=$(($printPatch + 1))
    hotfixbeta=$(($hotfixbeta + 1))
    printRevision="-beta$hotfixbeta"

elif [ "$branch" == "refs/heads/main" ]
  then
    if [ "$hotfixbeta" -eq 0 ]
      then
        minor=$(($minor + 1))
        printMinor=$minor
        patch=0
        printPatch=0
        beta=0
        alpha=0
      else
        patch=$(($patch + 1))
        printPatch=$patch
        hotfixbeta=0
    fi
else
    printMinor=$(($printMinor + 1))
    printPatch=0
    alpha=$(($alpha + 1))
    printRevision="-alpha$alpha"

fi

newVersionInfo=$major.$minor.$patch.$beta.$alpha.$hotfixbeta
versionNumber=$printMajor.$printMinor.$printPatch$printRevision
majorMinorPatch=$printMajor.$printMinor.$printPatch
# since a new patch only contains hotfixes we don't need to use different release notes
releaseNotesName="$printMajor.$printMinor.0.md"

echo "new version number is $versionNumber             $newVersionInfo"
versionInfoString="\"$newVersionInfo\""
versionInfoLength=${#versionInfoString}

echo "##vso[task.setvariable variable=VersionInfo]$newVersionInfo"
echo "##vso[task.setvariable variable=VersionInfo_String]$versionInfoString"
echo "##vso[task.setvariable variable=VersionInfo_Length]$versionInfoLength"
echo "##vso[task.setvariable variable=VersionNumber]$versionNumber"
echo "##vso[task.setvariable variable=VersionMajorMinorPatch]$majorMinorPatch"
echo "##vso[task.setvariable variable=ReleaseNotesName]$releaseNotesName"
echo "##vso[build.updatebuildnumber]$BUILDNUMBERPREFIX$versionNumber-$VERSIONCODE"