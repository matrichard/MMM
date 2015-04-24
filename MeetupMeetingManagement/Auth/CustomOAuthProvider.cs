using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace MeetupMeetingManagement.Auth
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
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
}