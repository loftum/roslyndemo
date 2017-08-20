using System;
using Studio.Extensions;

namespace Studio.ViewModels
{
    public class VariableModel
    {
        public Type Type { get; }
        public string Name { get; }
        public object Value { get; }

        public VariableModel(Type type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type.GetFriendlyName()} {Name} {Value ?? "null"}";
        }
    }
}