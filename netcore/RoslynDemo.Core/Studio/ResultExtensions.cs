using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoslynDemo.Core.Serializers;

namespace RoslynDemo.Core.Studio
{
    public static class ResultExtensions
    {
        public static string ToResultString(this object o)
        {
            switch (o)
            {
                case null:
                    return "null";
                case string s:
                    return s;
                case ICollection c:
                    return c.ToPrettyJson();
            }

            var type = o.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return o.ToString();
            }

            if (o is Exception ex)
            {
                var dictionary = new Dictionary<string, object>();
                var exceptionType = ex.GetType();
                foreach (var property in exceptionType.GetProperties().Where(p => p.DeclaringType == exceptionType))
                {
                    dictionary[property.Name] = property.GetValue(ex);
                }
                return $"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{Environment.NewLine}{dictionary.ToPrettyJson()}";
            }

            return o.ToPrettyJson();
        }
    }
}