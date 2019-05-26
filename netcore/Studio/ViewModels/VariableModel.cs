using System;
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo.Core.Studio;

namespace Studio.ViewModels
{
    public class VariableModel
    {
        public Type Type { get; }
        public string Name { get; }
        public object Value { get; }
        public bool IsReadonly { get; }

        public VariableModel(ScriptVariable variable)
        {
            Type = variable.Type;
            Name = variable.Name;
            Value = variable.Value;
            IsReadonly = variable.IsReadOnly;
        }

        public override string ToString()
        {
            var ro = IsReadonly ? "readonly " : "";
            return $"{ro}{Type.GetFriendlyName()} {Name} {Value ?? "null"}";
        }
    }
}