using System.Linq;
using System.Net;
using System.Web;

namespace Geta.EPi.GeolocationTools
{
    /// <summary>
    /// Helper class to retrieve correct IP address from request
    /// </summary>
    internal static class IPAddressHelper
    {
        public static IPAddress GetRequestIpAddress(HttpRequestBase request)
        {
            var test = GetDevIPAddress(request);
            if (!string.IsNullOrEmpty(test))
            {
                return ParseIpAddress(test);
            }

            var address = request.ServerVariables["True-Client-IP"];
            if (string.IsNullOrWhiteSpace(address))
            {
                var forwardedForAddresses = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(forwardedForAddresses))
                {
                    var addresses = forwardedForAddresses.Split(',');
                    if (addresses.Length > 0)
                    {
                        address = addresses.FirstOrDefault();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                address = request.ServerVariables["REMOTE_ADDR"];
            }

            return ParseIpAddress(address);
        }

        private static IPAddress ParseIpAddress(string address)
        {
            if (!IPAddress.TryParse(address, out IPAddress ipAddress))
            {
                return IPAddress.None;
            }

            return ipAddress;
        }

        private static string GetDevIPAddress(HttpRequestBase request)
        {
            return request?.Cookies[Constants.IPAddressOverride]?.Value;
        }
    }
}