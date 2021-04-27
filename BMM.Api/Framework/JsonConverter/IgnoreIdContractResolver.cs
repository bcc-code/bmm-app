using System.Reflection;
using BMM.Api.Implementation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BMM.Api.Framework.JsonConverter
{
    /// <summary>
    /// When sending objects to the Api the Id property is not supposed to be sent along since it's already included in the Url.
    /// Since Json.Net does not support multiple IContractResolver it has to inherit from <see cref="UnderscoreMappingResolver"/>.
    /// </summary>
    public class IgnoreIdContractResolver : UnderscoreMappingResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "Id" || property.PropertyName == "id")
            {
                property.ShouldSerialize = instance => !(instance is Document);
            }

            return property;
        }
    }
}