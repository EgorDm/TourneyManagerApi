using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TournamentManagerApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AuthorizationToken
    {
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; private set; }

        [JsonProperty(PropertyName = "expires")]
        public DateTime Expires { get; private set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; private set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; private set; }

        public AuthorizationToken() {}

        public AuthorizationToken(string tokenType, DateTime expires, string accessToken, string refreshToken) {
            TokenType = tokenType;
            Expires = expires;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public AuthorizationToken(string jsonString) {
            var json = JObject.Parse(jsonString);
            TokenType = json.GetValue("token_type").Value<string>();
            Expires = DateTime.Now.AddSeconds(json.GetValue("expires_in").Value<int>());
            AccessToken = json.GetValue("access_token").Value<string>();
            RefreshToken = json.GetValue("refresh_token").Value<string>();
        }

        public bool IsExpired() {
            return DateTime.Now >= Expires;
        }

        public string Serialize() {
            return JsonConvert.SerializeObject(this);
        }

        public static AuthorizationToken Deserialize(string json) {
            return JsonConvert.DeserializeObject<AuthorizationToken>(json);
        }
    }
}