using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TournamentManagerApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiResponse<T>
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "data")]
        public virtual T Data { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public JArray Errors { get; set; }

        public ApiResponse() {}
    }
}