#!/bin/sh
echo "Build BMM.TranslationKeysGenerator script start"
echo "Arguments: $1"

relative_project_path="$1"

dotnet restore $relative_project_path
dotnet build $relative_project_path

echo "Build BMM.TranslationKeysGenerator finished"