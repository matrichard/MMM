using System;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;

namespace MeetupMeetingManagement.Auth
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string SignatureAlgorithm => "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";

        public string DigestAlgorithm => "http://www.w3.org/2001/04/xmlenc#sha256";

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var issuer = "localhost";
            var audience = "all";
            var key = Convert.FromBase64String("bXlzdXBlcnNlY3JldGtleQ==");
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(60);
            var signingCredentials = new SigningCredentials(
                new InMemorySymmetricSecurityKey(key),
                SignatureAlgorithm,
                DigestAlgorithm);
            var token = new JwtSecurityToken(issuer, audience, data.Identity.Claims,
                now, expires, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}