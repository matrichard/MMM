using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MeetupMeetingManagement.Models;
using Newtonsoft.Json;

namespace MeetupMeetingManagement.Controllers
{
    [Authorize]
    public class EventsController : ApiController
    {
        private static string ApiKey = System.Configuration.ConfigurationManager.AppSettings["apiKey"];
        private static List<Event> Events = new List<Event>();
        private HttpClient client = new HttpClient() { BaseAddress = new Uri(@"https://api.meetup.com/", UriKind.Absolute) };

        public async Task<IHttpActionResult> Get()
        {
            if (Events.Any())
            {
                return Ok(Events);
            }

            var response = await client.GetAsync(string.Format("2/events?&sign=true&photo-host=public&group_urlname=msdevmtl&page=5&key={0}", ApiKey));
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await GetResponseContent(response);
                Events.AddRange(
                    JsonConvert.DeserializeObject<MeetupResponse<EventsDto>>(jsonContent)
                        .Results.Select(x => new Event {Name = x.Name, Url = x.Url, Id = x.Id, EpochTime = x.Epochtime}));
                foreach (var e in Events)
                {
                    var res = await client.GetAsync(string.Format("2/rsvps?&sign=true&event_id={0}&key={1}", e.Id, ApiKey));
                    if (res.IsSuccessStatusCode)
                    {
                        var jContent = await GetResponseContent(res);
                        e.Rsvps = JsonConvert.DeserializeObject<MeetupResponse<RsvpDto>>(jContent).Results.Select(x => new {x.Status, MemberId = x.Member["member_id"]});
                    }
                }
            }

            return Ok(Events);
        }

        [HttpGet]
        [Route("api/events/refresh")]
        public async Task<IHttpActionResult> Refresh()
        {
            Events.Clear();

            var response = await client.GetAsync(string.Format("2/events?&sign=true&photo-host=public&group_urlname=msdevmtl&page=5&key={0}", ApiKey));
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await GetResponseContent(response);
                Events.AddRange(
                    JsonConvert.DeserializeObject<MeetupResponse<EventsDto>>(jsonContent)
                        .Results.Select(x => new Event { Name = x.Name, Url = x.Url, Id = x.Id, EpochTime = x.Epochtime }));
                foreach (var e in Events)
                {
                    var res = await client.GetAsync(string.Format("2/rsvps?&sign=true&event_id={0}&key={1}", e.Id, ApiKey));
                    if (res.IsSuccessStatusCode)
                    {
                        var jContent = await GetResponseContent(res);
                        e.Rsvps = JsonConvert.DeserializeObject<MeetupResponse<RsvpDto>>(jContent).Results.Select(x => new { x.Status, MemberId = x.Member["member_id"] });
                    }
                }
            }

            return Ok(Events);
        } 

        protected override void Dispose(bool disposing)
        {
            client.Dispose();
            base.Dispose(disposing);
        }

        private async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            string jsonContent;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                jsonContent = new StreamReader(responseStream).ReadToEnd();
            }

            return jsonContent;
        }
    }
}