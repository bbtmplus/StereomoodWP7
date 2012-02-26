using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Codeplex.OAuth;
using Microsoft.Phone.Reactive;
using Stereomood.Json;


namespace Stereomood
{
    public class OauthCommunication
    {
        private static OauthCommunication instance;

        private RequestToken requestToken;
        private AccessToken accessToken;

        public static OauthCommunication getInstance()
        {
            return instance = instance ?? new OauthCommunication();
        }

        #region Delegates

        public delegate void loadFinishedEventHandler(int METHOD, Dictionary<string, string> parameters);

        #endregion

        #region Events

        public event loadFinishedEventHandler loadFinished;

        private void onLoadFinished(int METHOD, Dictionary<string, string> parameters)
        {
            if (loadFinished != null)
            {
                loadFinished(METHOD, parameters);
            }
        }

        #endregion


        private string ReadWebException(Exception e)
        {
            var ex = e as WebException;
            if (ex != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
            else return e.ToString();
        }

        public void getRequestToken()
        {
            var authorizer = new OAuthAuthorizer(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET);
            IObservable<TokenResponse<RequestToken>> collection = authorizer.GetRequestToken(Constants.REQUEST_TOKEN);
            collection.Select(res => res.Token)
                .ObserveOnDispatcher()
                .Subscribe(token =>
                               {
                                   requestToken = token;
                                   var url = authorizer.BuildAuthorizeUrl(Constants.AUTHORIZE, token);
                                   loadFinished(Constants.METHOD_AUTHORIZATION,
                                                new Dictionary<string, string> { { "URL", url } });
                               }, ex => MessageBox.Show(ReadWebException(ex)));
        }


        public void getAccessToken(string pinCode)
        {

            var authorizer = new OAuthAuthorizer(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET);
            authorizer.GetAccessToken(Constants.ACCESS_TOKEN, requestToken, pinCode)
                .ObserveOnDispatcher()
                .Subscribe(res =>
                {
                    accessToken = res.Token;
                    loadFinished(Constants.METHOD_ACCESS_TOKEN, null);
                }, ex => MessageBox.Show(ReadWebException(ex)));
        }

        public void searchSong(string searchQuery)
        {
            var serializer = new DataContractJsonSerializer(typeof(SearchResult));

            var client = new OAuthClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET, accessToken)
            {
                Url = Constants.SEARCH,
                Parameters = { { "type", "site" }, { "q", searchQuery } }
            };
            //    client.GetResponseText()
            //     .Select(s => XElement.Parse(s))
            //     .ObserveOnDispatcher()
            //      .Subscribe(x => MessageBox.Show("Post Success:" + x.ToString()),
            //         ex => MessageBox.Show(ReadWebException(ex)));

            client.GetResponseLines()
                .Select(s =>
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                    {
                        return (SearchResult)serializer.ReadObject(stream);
                    }
                })
                .ObserveOnDispatcher()
                .Subscribe(
                    t => MessageBox.Show("Ololo:" + t.total),
                    ex => MessageBox.Show(ReadWebException(ex)));
        }



        #region OAUTH_METHODS
        public static string nonce()
        {
            return new Random().Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }

        public static string timestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public string ParmsRequestUrl(string url, List<string> userParameters)
        {
            return (url + "?" + string.Join("&", userParameters.ToArray()));
        }

        public string OAuthRequestUrl(string url, List<string> userParameters, string mode = "GET")
        {
            List<string> Parameters = userParameters;
            Parameters.Add("oauth_consumer_key=" + HttpUtility.UrlEncode(Constants.CONSUMER_KEY));
            Parameters.Add("oauth_nonce=" + HttpUtility.UrlEncode(nonce()));
            Parameters.Add("oauth_signature_method=HMAC-SHA1");
            Parameters.Add("oauth_version=1.0");
            Parameters.Add("oauth_callback=oob");
            Parameters.Add("oauth_timestamp=" + HttpUtility.UrlEncode(timestamp()));
            Parameters.Add("oauth_token=" + accessToken.Key);

            Parameters.Sort();

            string parametersStr = string.Join("&", Parameters.ToArray());

            string baseStr = mode + "&" +
                 HttpUtility.UrlEncode(url).Replace("%2f", "%2F").Replace("%3a", "%3A").Replace("%3d", "%3D") + "&" +
                 HttpUtility.UrlEncode(parametersStr).Replace("%2f", "%2F").Replace("%3a", "%3A").Replace("%3d", "%3D");


            /* create the crypto class we use to generate a signature for the request */
            byte[] key = Encoding.UTF8.GetBytes(accessToken.Secret);
            key = Encoding.UTF8.GetBytes(accessToken.Secret + "&" + Constants.CONSUMER_SECRET);


            HMACSHA1 sha1 = new HMACSHA1(key);

            /*
             * byte[] key = Encoding.UTF8.GetBytes(VAL_OAUTHCONSUMERSECRET + "&" +
                                    requestSecret);
             * */
            /* generate the signature and add it to our parameters */
            byte[] baseStringBytes = Encoding.UTF8.GetBytes(baseStr);
            byte[] baseStringHash = sha1.ComputeHash(baseStringBytes);
            String base64StringHash = Convert.ToBase64String(baseStringHash);
            String encBase64StringHash = HttpUtility.UrlEncode(base64StringHash);
            Parameters.Add("oauth_signature=" + encBase64StringHash);
            Parameters.Sort();
            return (url + "?" + string.Join("&", Parameters.ToArray()));
        }
        #endregion
    }

}
