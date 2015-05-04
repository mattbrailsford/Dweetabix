using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dweetabix.Models
{
    public class Dweet
    {
        [JsonProperty("thing")]
        public string Thing { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("content")]
        public object Content { get; set; }
    }
}
