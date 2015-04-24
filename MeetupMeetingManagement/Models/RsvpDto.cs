using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupMeetingManagement.Models
{
    public class RsvpDto
    {
        [JsonProperty("response")]
        public string Status { get; set; }
        [JsonProperty("member")]
        public IDictionary<string, string> Member { get; set; } 
    }
}