using System;
using MvvmCross;
using System.Collections.Generic;
using System.IO;
using MvvmCross.Base;
using MvvmCross.Plugin.JsonLocalization;

namespace BMM.Core.Implementations.Localization
{
    public class BmmJsonDictionaryTextProvider : MvxDictionaryTextProvider, IMvxJsonDictionaryTextLoader
    {
        protected IMvxResourceLoader ResourceLoader
        {
            get
            {
                return Mvx.IoCProvider.Resolve<IMvxResourceLoader>();
            }
        }

        public BmmJsonDictionaryTextProvider(bool maskErrors = true)
            : base(maskErrors)
        {
        }

        public void LoadJsonFromResource(string namespaceKey, string typeKey, string resourcePath)
        {
            IMvxResourceLoader resourceLoader = this.ResourceLoader;
            string textResource = resourceLoader.GetTextResource(resourcePath);
            if (string.IsNullOrEmpty(textResource))
            {
                throw new FileNotFoundException("Unable to find resource file " + resourcePath);
            }
            LoadJsonFromText(namespaceKey, typeKey, textResource);
        }

        public void LoadJsonFromText(string namespaceKey, string typeKey, string rawJson)
        {
            var jsonConverter = Mvx.IoCProvider.Resolve<IMvxJsonConverter>();

            var dictionaries = jsonConverter.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(rawJson);

            foreach (var currentDictionary in dictionaries)
            {
                foreach (var current in currentDictionary.Value)
                {
                    string translationKey = $"{currentDictionary.Key}_{current.Key.Replace(".", "_")}";
                    AddOrReplace(string.Empty, string.Empty, translationKey, current.Value);
                }
            }
        }
    }
}