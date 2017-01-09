using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace HubApp.Utilities
{
    class HtmlConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(string))
            {
                return WebUtility.HtmlDecode(reader.Value as string);
            }
            else if (objectType == typeof(List<string>))
            {
                return (reader.Value as List<string>).Select(v => WebUtility.HtmlDecode(v)).ToList();
            }
            else
            {
                return reader.Value;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(List<string>);
        }
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
