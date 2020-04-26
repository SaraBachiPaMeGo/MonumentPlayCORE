using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteMonument.Models
{
    public class MonumentosList
    {
        [JsonProperty("monuments")]
        public List<Monument> Monumentos { get; set; }
    }
}
