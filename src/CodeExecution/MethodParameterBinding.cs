using System;

namespace Cheevly.CodeExecution {
    public struct MethodParameterBinding {
        public string Name { get; set; }
        public object Value { get; set; }

        public MethodParameterBinding(string name, object value) {
            Name = name;
            Value = value;
        }
    }
}