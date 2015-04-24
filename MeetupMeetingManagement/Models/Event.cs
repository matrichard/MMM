using System.Collections.Generic;

namespace MeetupMeetingManagement.Models
{
    public class Event
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public IEnumerable<dynamic> Rsvps { get; set; }
        public string Id { get; set; }

        public string EpochTime { get; set; }
    }
}