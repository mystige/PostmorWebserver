using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PostmorWebServer.Extensions
{
    //Extensions for extraction the Id of a user from a claim
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}
