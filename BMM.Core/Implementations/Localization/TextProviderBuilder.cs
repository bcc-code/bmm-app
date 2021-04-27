using BMM.Core.Helpers;
using System.Collections.Generic;
using MvvmCross.Plugin.JsonLocalization;

namespace BMM.Core.Implementations.Localization
{
    public class TextProviderBuilder : MvxTextProviderBuilder
    {
        private static string DefaultLanguage;

        public TextProviderBuilder(BmmJsonDictionaryTextProvider provider)
            : base(GlobalConstants.GeneralNamespace, GlobalConstants.RootFolderForResources, provider, provider)
        {
        }

        public static void SetDefaultLanguage(string language)
        {
            DefaultLanguage = language;
        }

        public static string GetDefaultLanguage()
        {
            return DefaultLanguage;
        }

        /// <summary>
        /// Load a dictionary, containing the ViewModel and the file, that should be loaded for that ViewModel.
        /// </summary>
        protected override IDictionary<string, string> ResourceFiles => new Dictionary<string, string>() { { "main", "main" } };

        protected override string GetResourceFilePath(string whichLocalisationFolder, string whichFile)
        {
            if (string.IsNullOrEmpty(whichLocalisationFolder))
            {
                whichLocalisationFolder = DefaultLanguage;
            }

            return base.GetResourceFilePath(whichLocalisationFolder, whichFile);
        }
    }
}