using System;
using System.Collections.Generic;
using System.Linq;
using Convenient.Stuff.Serializers;

namespace Studio.Extensions
{
    public static class ResultExtensions
    {
        public static string ToResultString(this object o)
        {
            if (o == null)
            {
                return "null";
            }
            var s = o as string;
            if (s != null)
            {
                return s;
            }
            if (o.GetType().IsValueType || o.GetType().IsEnum)
            {
                return o.ToString();
            }

            var ex = o as Exception;
            if (ex != null)
            {
                var dictionary = new Dictionary<string, object>();
                var type = ex.GetType();
                foreach (var property in type.GetProperties().Where(p => p.DeclaringType == type))
                {
                    dictionary[property.Name] = property.GetValue(ex);
                }
                return $"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{Environment.NewLine}{dictionary.ToJson(true, true)}";
            }
            return o.ToJson(true, true);
        }
    }
}