using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MeetupMeetingManagement.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public class MembersController : ApiController
    {
        private static string ApiKey = System.Configuration.ConfigurationManager.AppSettings["apiKey"];
        private HttpClient client = new HttpClient() { BaseAddress = new Uri(@"https://api.meetup.com/", UriKind.Absolute) };
        public async Task<IHttpActionResult> Get()
        {
            var group = await LoadGroup();
            var members = new List<dynamic>();
            var groups = group.Members / 200;
            var tasks = new Task<IEnumerable<dynamic>>[groups];
            for (int i = 0; i <= groups; i++)
            {
                await LoadMembers(i).ContinueWith(coll => members.AddRange(coll.Result));
            }

            return Ok(members);
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

    #region Models
    public class Member
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public string Picture { get; private set; }

        public Membership Membership { get; internal set; }

        public static Member Create(ProfilesDto dto)
        {
            var member = new Member
            {
                Id = dto.Id,
                Name = dto.Name
            };

            string thumbnail;
            if(!dto.Picture.TryGetValue("thumb_link", out thumbnail))
            {
                thumbnail =  @"http://img2.meetupstatic.com/img/2982428616572973604/noPhoto_80.gif";
            }
            member.Picture = thumbnail;
            member.Membership = Membership.Create(dto.MembershipDues);
            return member;
        }
    }

    public class Membership
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum State { None, Current, Due, Expired };

        public Membership()
        {
            this.Status = State.None;
        }

        public State Status { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public DateTime TransactionDate { get; private set; }

        internal static Membership Create(IDictionary<string, string> membershipDues)
        {
            var today = DateTime.UtcNow.Date;
            var membership = new Membership();
            var transactionTime = "0";
            if(membershipDues.TryGetValue("transaction_time", out transactionTime))
            {
                double time;
                if(double.TryParse(transactionTime, out time))
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var transaction = epoch.AddMilliseconds(time);

                    membership.TransactionDate = transaction.Date;
                    membership.ExpirationDate = transaction.AddYears(1).Date;
                    if (membership.ExpirationDate.Date < today)
                    {
                        membership.Status = State.Expired;
                    }
                    else if (membership.ExpirationDate.Date.AddDays(-30) <= today)
                    {
                        membership.Status = State.Due;
                    }
                    else
                    {
                        membership.Status = State.Current;
                    }
                }
            }

            return membership;
        }
    }

    #endregion

    #region DTOs
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

        [JsonProperty("membership_dues")]
        public IDictionary<string,string> MembershipDues { get; set; }
    }

    public class MeetupResponse<T>
    {
        [JsonProperty("results")]
        public IList<T> Results { get; set; }
    }
    #endregion
}
