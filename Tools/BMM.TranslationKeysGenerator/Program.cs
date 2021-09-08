using BMM.TranslationKeysGenerator.Generators;
using CommandLine;

namespace BMM.TranslationKeysGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var generator = new KeysGenerator();
            generator.GenerateTranslationKeysFile(options.SolutionPath, options.CurrentFilePath);
        }
    }
}