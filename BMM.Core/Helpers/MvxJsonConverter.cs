using Newtonsoft.Json;
using System;
using System.IO;
using MvvmCross.Base;

namespace BMM.Core.Helpers
{
    public class MvxJsonConverter : IMvxJsonConverter
    {
        public T DeserializeObject<T>(string inputText)
        {
            return JsonConvert.DeserializeObject<T>(inputText);
        }

        public T DeserializeObject<T>(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public string SerializeObject(object toSerialise)
        {
            return JsonConvert.SerializeObject(toSerialise);
        }

        public object DeserializeObject(Type type, string inputText)
        {
            throw new NotImplementedException();
            //return JsonConvert.DeserializeObject (type, inputText);
        }
    }
}