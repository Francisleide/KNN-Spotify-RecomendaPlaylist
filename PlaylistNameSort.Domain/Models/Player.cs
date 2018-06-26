using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistNameSort.Domain.Models
{
    public class Player
    {
        [JsonProperty("played_at")]
        public string Played_at { get; set; }
        [JsonProperty("items")]
        public List<Track> Items { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }

    }
}
