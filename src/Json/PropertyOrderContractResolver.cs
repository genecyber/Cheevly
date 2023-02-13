using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cheevly.Json {
    public class PropertyOrderContractResolver : CamelCasePropertyNamesContractResolver {

        public List<string> OrderedProperties { get; set; }

        public PropertyOrderContractResolver(List<PropertyInfo> orderedProperties) {
            OrderedProperties = orderedProperties.Select(each => each.Name).ToList();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var ordered = base.CreateProperties(type, memberSerialization)
                .OrderByDescending(each => OrderedProperties.Contains(each.UnderlyingName))
                .ThenBy(each => OrderedProperties.IndexOf(each.UnderlyingName))
                .ToList();

            return ordered;
        }
    }
}