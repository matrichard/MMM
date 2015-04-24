using MeetupMeetingManagement.Auth;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MeetupMeetingManagement.Startup))]

namespace MeetupMeetingManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(new CustomOAuthOptions());
            app.UseJwtBearerAuthentication(new CustomJwtOptions());
        }
    }
}
