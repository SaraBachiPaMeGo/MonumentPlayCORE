using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteMonument.Models
{
    public class Country
    {
        [JsonProperty("alpha2Code")]
        public String Siglas { get; set; }

        [JsonProperty("name")]
        public String NombrePais { get; set; }
    }
}
