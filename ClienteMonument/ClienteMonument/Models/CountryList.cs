using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteMonument.Models
{
    public class CountryList
    {
        [JsonProperty("result")]
        public List<Country> Paises { get; set; }
    }
}
