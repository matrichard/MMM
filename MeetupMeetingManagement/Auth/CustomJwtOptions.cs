using System;
using Microsoft.Owin.Security.Jwt;

namespace MeetupMeetingManagement.Auth
{
    public class CustomJwtOptions : JwtBearerAuthenticationOptions
    {
        public CustomJwtOptions()
        {
            var issuer = "localhost";
            var audience = "all";
            var key = Convert.FromBase64String("bXlzdXBlcnNlY3JldGtleQ=="); ;

            AllowedAudiences = new[] { audience };
            IssuerSecurityTokenProviders = new[]
            {
                new SymmetricKeyIssuerSecurityTokenProvider(issuer, key)
            };
        }
    }
}