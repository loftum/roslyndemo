using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Convenient.Stuff.Serializers
{
    public static class Json
    {
        public static string ToJson(this object item, bool indented = false, bool suppressErrors = false)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            if (suppressErrors)
            {
                settings.Error = Suppress;
            }
            return JsonConvert.SerializeObject(item, formatting, settings);
        }

        private static void Suppress(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}