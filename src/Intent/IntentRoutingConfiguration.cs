using System;

namespace Cheevly.Intent
{
    public class IntentRoutingConfiguration {

        public List<IntentMapping> Mappings = new();

        public IntentMapping<object> Use() {
            var mapping = new IntentMapping<object>(new object());
            Mappings.Add(mapping);
            return mapping;
        }

        public IntentMapping<T> Use<T>(T host) {
            var mapping = new IntentMapping<T>(host);
            Mappings.Add(mapping);
            return mapping;
        }
    }
}