using System;
using System.IO;
using System.Net;
using System.Windows;
using Codeplex.OAuth;
using Microsoft.Phone.Reactive;


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
                }, ex => MessageBox.Show(ReadWebException(ex)));
        }
    }
}
