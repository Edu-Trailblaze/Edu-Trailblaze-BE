using Contracts.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Common
{
    public class SerializeService : ISerializeService
    {
        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize<T>(T obj)
        {
          return  JsonConvert.SerializeObject(obj, new JsonSerializerSettings 
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> 
                { new Newtonsoft.Json.Converters.StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    } 
                }
            });
        }

        public string Serialize<T>(T obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, new JsonSerializerSettings());
        }
    }
}
