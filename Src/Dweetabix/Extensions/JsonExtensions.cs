using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dweetabix.Extensions
{
    public static class JsonExtensions
    {
        public static TEntity As<TEntity> (this object obj)
        {
            if(obj is string || obj is JToken)
                return JsonConvert.DeserializeObject<TEntity>(obj.ToString());

            var str = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<TEntity>(str);
        }
    }
}
