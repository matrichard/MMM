using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupMeetingManagement.Models
{
    public class GroupDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("urlname")]
        public string UrlName { get; set; }
        [JsonProperty("members")]
        public int Members { get; set; }
        [JsonProperty("group_photo")]
        public IDictionary<string, string> Photo { get; set; }
    }
}