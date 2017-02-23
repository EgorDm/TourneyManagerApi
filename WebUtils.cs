using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TournamentManagerApi
{
    public class WebUtils
    {
        public static string ResponseToString(WebResponse response) {
            using (var stream = response.GetResponseStream()) {
                using (var responseStream = new StreamReader(stream)) {
                    var responseText = responseStream.ReadToEnd();
                    return responseText;
                }
            }
        }

        public static string BuildParmeters(IEnumerable<KeyValuePair<string, string>> parameters) {
            var ret = new StringBuilder();
            foreach (var keyValuePair in parameters) {
                if (ret.Length > 0) ret.Append("&");
                ret.AppendFormat("{0}={1}", keyValuePair.Key, HttpUtility.UrlEncode(keyValuePair.Value));
            }
            return ret.ToString();
        }
    }
}
