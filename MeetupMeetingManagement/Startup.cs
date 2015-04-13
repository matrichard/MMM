using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(MeetupMeetingManagement.Startup))]

namespace MeetupMeetingManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(new MyOAuthOptions());
            app.UseJwtBearerAuthentication(new MyJwtOptions());
        }
    }

    public class MyOAuthOptions : OAuthAuthorizationServerOptions
    {
        public MyOAuthOptions()
        {
            TokenEndpointPath = new PathString("/oauth/token");
            AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60);
            AccessTokenFormat = new MyJwtFormat();
            Provider = new MyOAuthProvider();
            AllowInsecureHttp = true;
        }
    }

    public class MyJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        //private readonly OAuthAuthorizationServerOptions _options;

        public MyJwtFormat()
        {
            //_options = options;
        }

        public string SignatureAlgorithm
        {
            get { return "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"; }
        }

        public string DigestAlgorithm
        {
            get { return "http://www.w3.org/2001/04/xmlenc#sha256"; }
        }

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

    public class MyOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity("otc");
            var username = context.OwinContext.Get<string>("otc:username");
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", username));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "user"));
            context.Validated(identity);
            return Task.FromResult(0);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                var username = SecureValue(context.Parameters["username"]);
                var password = SecureValue(context.Parameters["password"]);

                const string securePw = "BD288A578F50B2F9E881E995BACA7249331CF28F5983E8F25CEE2F1A6807816F";
                const string secureUn = "84A9C90E81FFA433C6C153BD366D815297DFCA024C7FF70CEE0FA1CE64E1F12E";
                
                if (username == secureUn && password == securePw)
                {
                    context.OwinContext.Set("otc:username", username);
                    context.Validated();
                }
                else
                {
                    context.SetError("Invalid credentials");
                    context.Rejected();
                }
            }
            catch
            {
                context.SetError("Server error");
                context.Rejected();
            }
            return Task.FromResult(0);
        }

        private static string SecureValue(string message)
        {
            const string key = "msdevmtl";

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            var keyByte = encoding.GetBytes(key);

            var sha = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = sha.ComputeHash(messageBytes);
            return ByteToString(hashmessage);
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }

    public class MyJwtOptions : JwtBearerAuthenticationOptions
    {
        public MyJwtOptions()
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
