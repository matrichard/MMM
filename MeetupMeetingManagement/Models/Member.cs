using System.Globalization;
using System.Linq;
using System.Text;

namespace MeetupMeetingManagement.Models
{
    public class Member
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public string Picture { get; private set; }

        public string Url { get; private set; }

        public Membership Membership { get; internal set; }

        public static Member Create(ProfilesDto dto)
        {
            var member = new Member
            {
                Id = dto.Id,
                Name = dto.Name,
                Url = dto.Url,
                Searchable = RemoveDiacritics(dto.Name)
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

        public string Searchable { get; private set; }

        private static string RemoveDiacritics(string text)
        {
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                    .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                 UnicodeCategory.NonSpacingMark)
                ).Normalize(NormalizationForm.FormC);
        }
    }
}