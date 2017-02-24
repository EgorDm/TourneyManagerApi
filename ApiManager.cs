using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TournamentManagerApi.Models;

namespace TournamentManagerApi
{
    public class ApiManager
    {
        public const string BASE = "http://tournament-manager.ml";
        public const string REDIRECT_URI = BASE + "/api/authorized_callback";
        public const string AUTHORIZE_URI = BASE + "/oauth/authorize";
        public const string TOKEN_URI = BASE + "/oauth/token";

        public static string ClientId { get; private set; }
        public static string ClientSecret { get; private set; }
        public static string Scope { get; private set; }

        private static AuthorizationToken _token;

        /// <summary>
        /// API token
        /// </summary>
        public static AuthorizationToken Token {
            get {
                if (_token == null && !string.IsNullOrEmpty(Properties.Settings.Default.AuthToken)) {
                    _token = AuthorizationToken.Deserialize(Properties.Settings.Default.AuthToken);
                }
                return _token;
            }
            set {
                _token = value;
                if (value == null) {
                    Properties.Settings.Default.AuthToken = "";
                    Properties.Settings.Default.Save();
                } else {
                    Properties.Settings.Default.AuthToken = _token.Serialize();
                    Properties.Settings.Default.Save();
                }
               
            }
        }

        /// <summary>
        /// Init api manager with your app credentials
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="scope"></param>
        public static void Init(string clientId, string clientSecret, string scope) {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
        }

        /// <summary>
        /// Designed to run in the ui thread because of the AuthorizeWindow. 
        /// Make your own implementation if you want to run everything(except authorization) in background. 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Task PrepareToken(Action<bool> callback) {
            return new Task(() => {
                if (Token == null) {
                    var window = new AuthorizeWindow(ClientId, Scope);
                    if (window.ShowDialog() != true) {
                        callback.Invoke(false);
                        return;
                    }
                    callback.Invoke(CodeToToken(window.Code));
                    return;
                }
                if (Token.IsExpired()) {
                    callback.Invoke(RefreshToken(Token));
                    return;
                }
                callback.Invoke(true);
            });
        }

        /// <summary>
        /// Send a http GET api request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T ApiCallGetAbstract<T>(string url) where T : class {
            if (Token.IsExpired() && !RefreshToken(Token)) return null;
            //Build Request
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["Authorization"] = $"Bearer {Token.AccessToken}";
            //Request
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var respString = WebUtils.ResponseToString(webResponse);
            var json = JsonConvert.DeserializeObject<T>(respString);
            return json;
        }

        /// <summary>
        /// Send a http GET api request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ApiResponse<T> ApiCallGet<T>(string url) {
            return ApiCallGetAbstract<ApiResponse<T>>(url);
        }

        /// <summary>
        /// Send a http POST api request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public static ApiResponse<T> ApiCallPost<T>(string url, IEnumerable<KeyValuePair<string, string>> post) {
            if (Token.IsExpired() && !RefreshToken(Token)) return null;
            //Build post params
            var postdata = Encoding.UTF8.GetBytes(WebUtils.BuildParmeters(post));
            //Build Request
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["Authorization"] = $"Bearer {Token.AccessToken}";
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;
            using (var writer = webRequest.GetRequestStream()) {
                writer.Write(postdata, 0, postdata.Length);
            }
            //Request
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var json = JsonConvert.DeserializeObject<ApiResponse<T>>(WebUtils.ResponseToString(webResponse));
            return json;
        }

        /// <summary>
        /// Exchange code to token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected static bool CodeToToken(string code) {
            //Build post params
            var postdata = Encoding.UTF8.GetBytes(WebUtils.BuildParmeters(new Dictionary<string, string>() {
                {"grant_type", "authorization_code" },
                 {"client_id", ClientId },
                 {"client_secret", ClientSecret },
                 {"code", code },
                 {"redirect_uri", REDIRECT_URI }
            }));
            //Build Request
            var webRequest = (HttpWebRequest) WebRequest.Create(TOKEN_URI);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;
            using (var writer = webRequest.GetRequestStream()) {
                writer.Write(postdata, 0, postdata.Length);
            }
            //Request
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            Token = new AuthorizationToken(WebUtils.ResponseToString(webResponse));
            return true;
        }

        /// <summary>
        /// Refresh existing token
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        protected static bool RefreshToken(AuthorizationToken current) {
            if (current == null) return false;
            //Build post params
            var postdata = Encoding.UTF8.GetBytes(WebUtils.BuildParmeters(new Dictionary<string, string>() {
                {"grant_type", "refresh_token" },
                 {"refresh_token", current.RefreshToken },
                 {"client_id", ClientId },
                 {"client_secret", ClientSecret },
                 {"scope", Scope }
            }));
            //Build Request
            var webRequest = (HttpWebRequest) WebRequest.Create(TOKEN_URI);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;
            using (var writer = webRequest.GetRequestStream()) {
                writer.Write(postdata, 0, postdata.Length);
            }
            //Request
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            Token = new AuthorizationToken(WebUtils.ResponseToString(webResponse));
            return true;
        }
    }
}