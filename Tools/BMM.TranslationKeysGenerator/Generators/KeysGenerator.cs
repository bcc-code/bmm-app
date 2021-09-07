using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BMM.TranslationKeysGenerator.Helpers;
using Newtonsoft.Json.Linq;

namespace BMM.TranslationKeysGenerator.Generators
{
    public class KeysGenerator
    {
        private const string TranslationKeysClassName = "Translations";

        public void GenerateThemeColorsFile(string solutionPath, string inputFile)
        {
            try
            {
                var translationKeys = GetTranslationKeys(inputFile);
                string fileNamespace = GetNamespace(solutionPath, inputFile);
                string fileContentString = BuildFile(@fileNamespace, translationKeys);
                var fileInfo = new FileInfo(inputFile);
                string outputFile = $"{TranslationKeysClassName}.designer.cs";
                string outputPath = Path.Combine(fileInfo.DirectoryName!, outputFile);
                File.WriteAllText(outputPath, fileContentString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KeysGenerator] {ex}");
            }
        }

        private static IList<string> GetTranslationKeys(string inputFile)
        {
            var translationsObject = FileHelper.GetTranslations(inputFile);

            var listOfKeys = new List<string>();
            foreach (var translationsNode in translationsObject.Properties())
            {
                foreach (var tra in translationsNode.Values().OfType<JProperty>())
                {
                    var category = translationsNode.Name;
                    var name = tra.Name.Replace(".", "_");

                    listOfKeys.Add($"{category}_{name}");
                }
            }

            return listOfKeys;
        }

        private string GetNamespace(string solutionPath, string inputFile)
        {
            string directory = Path.GetDirectoryName(inputFile);
            string path = directory.Replace(solutionPath, "");
            string @namespace = path.Replace(Path.DirectorySeparatorChar.ToString(), ".").Trim('.');
            return @namespace;
        }

        private static string BuildFile(string @namespace, IEnumerable<string> keys)
        {
            var fileContent = new StringBuilder();
            fileContent.AppendLine("");
            fileContent.AppendLine($"namespace {@namespace}");
            fileContent.AppendLine("{");
            fileContent.AppendLine($"\tpublic static class {TranslationKeysClassName}");
            fileContent.AppendLine("\t{");

            foreach (var key in keys!)
            {
                string field = $"\t\tpublic const string {key} = nameof({key});";
                fileContent.AppendLine(field);
            }

            fileContent.AppendLine("\t}");
            fileContent.AppendLine("}");

            return fileContent.ToString();
        }
    }
}