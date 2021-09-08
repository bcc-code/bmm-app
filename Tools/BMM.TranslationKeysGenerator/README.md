This tool is design to automatically create translation keys for BMM project.

If you're using Jetbrains Rider, the Translations.designer.cs file should be regenerated on every saved change in main.json for English file without any additional settings. 

If the tool does not work as expected, please follow these steps:

 - Rider:
     - Preferences -> Tools -> Custom Tools
     - Find BMMTranslationKeysGenerator in list or create new
     - Make sure that in Custom Tools field name is 'BMMTranslationKeysGenerator'
     - Path should point to 'Tools\\BMM.TranslationKeysGenerator\\run_program.sh'
     - Arguments to script need to be '\"$SOLUTION_FOLDER$\" \"$FILE$\"'
     - In some cases it is needed to make run_program.sh executable by using chmod  


 - Visual Studio for Mac: 
     - Auto-generation won't work in VS as it doesn't support such custom tools, but it can still be run manually
     - Tools -> Edit Custom Tools
     - Add new tool with name 'BMMTranslationKeysGenerator'
     - Command should point to 'Tools\\BMM.TranslationKeysGenerator\\run_program.sh' (absolute path)"
     - Arguments to script need to be '\"${SolutionDir}\" \"${FileDir}/${FileName}\"
     - You can bind a key to execute it faster"
     - Click OK. Script can be executed from Tools -> Custom -> BMMTranslationKeysGenerator (main.json needs to be opened and focused)
