using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Editor.Serializers
{
    public static class Json
    {
        public static string ToJson(this object item, bool indented = false, bool suppressErrors = false)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            var settings = new JsonSerializerSettings();
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
    }
}