using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteMonument.Models
{
    public class Monument
    {
        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("country")]
        public String Siglas{ get; set; }

        [JsonProperty("name")]
        public String NombreMonu { get; set; }

        [JsonProperty("lat")]
        public string Latitud { get; set; }

        [JsonProperty("lon")]
        public string Longitud { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
