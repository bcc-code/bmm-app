using CommandLine;

namespace BMM.TranslationKeysGenerator
{
    public class Options
    {
        [Option('s', "solutionPath", Required = false, HelpText = "Path to solution folder.")]
        public string SolutionPath { get; set; }

        [Option('f', "file", Required = true, HelpText = "Path to current file.")]
        public string CurrentFilePath { get; set; }
    }
}