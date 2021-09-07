#!/bin/sh
echo "TranslationKeysGenerator script start"
echo "Arguments: $@"

solution_path="$2"
dll_path="$solution_path/Tools/BMM.TranslationKeysGenerator/Output/bin/BMM.TranslationKeysGenerator.dll"
echo "Program path: $dll_path"

dotnet "$dll_path" "$@"

echo "TranslationKeysGenerator finished"