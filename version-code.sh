#!/bin/bash

URL="https://bmm-brunstad.firebaseio.com/version_code.json";
VERSIONCODE=`curl -s $URL`

function setVersion {
	find . -name 'AndroidManifest.xml' -type f -exec sed -i.bak -e "s/android:versionCode=\"[0-9]*\"/android:versionCode=\"$VERSIONCODE\"/" {} \;
	echo "Set version code of AndroidManifest.xml to $VERSIONCODE"
}

function incrementVersion {
	((VERSIONCODE++))
	curl -s -X PUT -d "$VERSIONCODE" $URL > /dev/null
	echo "Increased version code from $((VERSIONCODE-1)) to $((VERSIONCODE))"
}

case $1 in
	set)
		setVersion
		;;
	increment)
		incrementVersion
		;;
	*)
		echo "Unknown command, use either get or increment"
		exit;
esac
