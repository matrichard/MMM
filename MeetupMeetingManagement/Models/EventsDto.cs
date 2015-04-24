using Newtonsoft.Json;

namespace MeetupMeetingManagement.Models
{
    public class EventsDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("event_url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("time")]
        public string Epochtime { get; set; }
    }
}