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
    public class MembersController : ApiController
    {
        private static string ApiKey = System.Configuration.ConfigurationManager.AppSettings["apiKey"];
        private static List<dynamic> Members = new List<dynamic>();

        private HttpClient client = new HttpClient() { BaseAddress = new Uri(@"https://api.meetup.com/", UriKind.Absolute) };

        public async Task<IHttpActionResult> Get()
        {
            if (Members.Any())
            {
                return Ok(Members);
            }

            var group = await LoadGroup();
            var groups = @group.Members / 200;
            for (var i = 0; i <= groups; i++)
            {
                await LoadMembers(i).ContinueWith(coll => Members.AddRange(coll.Result));
            }

            return Ok(Members);
        }

        [HttpGet]
        [Route("api/members/refresh")]
        public async Task<IHttpActionResult> Refresh()
        {
            Members.Clear();

            var group = await LoadGroup();
            var groups = @group.Members / 200;
            for (var i = 0; i <= groups; i++)
            {
                await LoadMembers(i).ContinueWith(coll => Members.AddRange(coll.Result));
            }

            return Ok(Members);
        }


        protected override void Dispose(bool disposing)
        {
            client.Dispose();
            base.Dispose(disposing);
        }
        private async Task<GroupDto> LoadGroup()
        {
            var response = await client.GetAsync("2/groups?&sign=true&photo-host=public&group_urlname=msdevmtl&key=" + ApiKey);
            GroupDto groupDto = null;
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await GetResponseContent(response);
                groupDto = JsonConvert.DeserializeObject<MeetupResponse<GroupDto>>(jsonContent).Results.FirstOrDefault();
            }

            return groupDto;
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

        private async Task<IEnumerable<dynamic>> LoadMembers(int offset)
        {
            IEnumerable<dynamic> members = null;
            var response = await client.GetAsync(string.Format("2/profiles?sign=true&photo-host=public&group_urlname=msdevmtl&fields=membership_dues&order=name&offset={0}&key={1}", offset, ApiKey));
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await GetResponseContent(response);
                members = JsonConvert.DeserializeObject<MeetupResponse<ProfilesDto>>(jsonContent).Results.Select(Member.Create);
            }

            return members;
        }
    }
}