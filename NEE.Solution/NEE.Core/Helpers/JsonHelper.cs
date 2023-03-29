using Newtonsoft.Json;
using System.IO;

namespace NEE.Core.Helpers
{
    public static class JsonHelper
    {
        public static string Serialize(object obj, bool indented = true)
        {
            using (var wr = new StringWriter())
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = indented ? Formatting.Indented : Formatting.None,
                };
                JsonSerializer.CreateDefault(settings).Serialize(wr, obj);
                var json = wr.ToString();
                return json;
            }
        }

    }
}