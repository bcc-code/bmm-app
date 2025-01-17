using System;
using MvvmCross;
using System.Collections.Generic;
using System.IO;
using MvvmCross.Base;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross.Plugin.ResourceLoader;

namespace BMM.Core.Implementations.Localization
{
    public class BmmJsonDictionaryTextProvider
        : MvxDictionaryTextProvider,
          IMvxJsonDictionaryTextLoader
    {
        private const string DefaultTranslationResourcePath = "Translation/en/main.json";
        private readonly IMvxResourceLoader _mvxResourceLoader;
        private readonly IMvxJsonConverter _mvxJsonConverter;

        public BmmJsonDictionaryTextProvider(
            IMvxResourceLoader mvxResourceLoader,
            IMvxJsonConverter mvxJsonConverter,
            bool maskErrors = true)
            : base(maskErrors)
        {
            _mvxResourceLoader = mvxResourceLoader;
            _mvxJsonConverter = mvxJsonConverter;
        }

        public void LoadJsonFromResource(string namespaceKey, string typeKey, string resourcePath)
        {
            string textResource = _mvxResourceLoader.GetTextResource(resourcePath);
            ThrowExceptionWhenResourceNotFound(resourcePath, textResource);
            LoadJsonFromText(namespaceKey, typeKey, textResource);
        }

        public void LoadJsonFromText(
            string namespaceKey,
            string typeKey,
            string textResourceJson)
        {
            string defaultTextResourceJson = _mvxResourceLoader.GetTextResource(DefaultTranslationResourcePath);
            ThrowExceptionWhenResourceNotFound(DefaultTranslationResourcePath, defaultTextResourceJson);
            
            var textResourceDictionary = DeserializeTextResource(textResourceJson);
            var defaultResourceDictionary = DeserializeTextResource(defaultTextResourceJson);

            foreach (var currentDictionary in textResourceDictionary)
            {
                foreach (var current in currentDictionary.Value)
                {
                    string valueToUse = string.IsNullOrWhiteSpace(current.Value) 
                        ? GetDefaultResourceValue(defaultResourceDictionary, currentDictionary.Key, current.Key)
                        : current.Value;
                    
                    string translationKey = $"{currentDictionary.Key}_{current.Key.Replace(".", "_")}";
                    AddOrReplace(string.Empty, string.Empty, translationKey, valueToUse);
                }
            }
        }
        
        private string GetDefaultResourceValue(
            Dictionary<string, Dictionary<string, string>> defaultResourceDictionary,
            string dictionaryKey,
            string key)
        {
            if (defaultResourceDictionary.TryGetValue(dictionaryKey, out var defaultValues)
                && defaultValues.TryGetValue(key, out string defaultValue))
                return defaultValue;

            return string.Empty;
        }

        private static void ThrowExceptionWhenResourceNotFound(string resourcePath, string textResource)
        {
            if (string.IsNullOrEmpty(textResource))
                throw new FileNotFoundException("Unable to find resource file " + resourcePath);
        }
        
        private Dictionary<string, Dictionary<string, string>> DeserializeTextResource(string textResourceJson)
        {
            return _mvxJsonConverter
                .DeserializeObject<Dictionary<string, Dictionary<string, string>>>(textResourceJson);
        }
    }
}