using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;

namespace MeetupMeetingManagement.Auth
{
    public class CustomOAuthOptions : OAuthAuthorizationServerOptions
    {
        public CustomOAuthOptions()
        {
            TokenEndpointPath = new PathString("/oauth/token");
            AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60);
            AccessTokenFormat = new CustomJwtFormat();
            Provider = new CustomOAuthProvider();
            AllowInsecureHttp = true;
        }
    }
}