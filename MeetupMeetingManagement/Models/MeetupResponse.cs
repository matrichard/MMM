using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupMeetingManagement.Models
{
    public class MeetupResponse<T>
    {
        [JsonProperty("results")]
        public IList<T> Results { get; set; }

        [JsonProperty("meta")]
        public IDictionary<string, string> Meta { get; set; } 
    }
}