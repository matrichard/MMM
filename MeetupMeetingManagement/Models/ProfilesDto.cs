using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupMeetingManagement.Models
{
    public class ProfilesDto
    {
        public ProfilesDto()
        {
            Picture = new Dictionary<string, string>();
            MembershipDues = new Dictionary<string, string>();
        }

        [JsonProperty("member_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("photo")]
        public IDictionary<string,string> Picture { get; set; }

        [JsonProperty("profile_url")]
        public string Url { get; set; }

        [JsonProperty("membership_dues")]
        public IDictionary<string,string> MembershipDues { get; set; }
    }
}