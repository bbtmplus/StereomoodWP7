using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
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

        #region Delegates & Events

        public delegate void loadFinishedEventHandler(int METHOD, Dictionary<string, string> parameters, JsonObject jsonObject);

        public event loadFinishedEventHandler loadFinished;

        public void onLoadFinished(int METHOD, Dictionary<string, string> parameters, JsonObject jsonObject)
        {
            if (loadFinished != null)
            {
                loadFinished(METHOD, parameters, jsonObject);
            }
        }

        public class ArrayEvent<T>
        {
            private static ArrayEvent<T> arrayEventInstance;

            public static ArrayEvent<T> getArrayEventInstance()
            {
                return arrayEventInstance = arrayEventInstance ?? new ArrayEvent<T>();
            }

            public delegate void loadFinishedWithArrayEventHandler(int METHOD, T[] parameters);

            public event loadFinishedWithArrayEventHandler loadFinishedWithArray;

            public void onLoadFinishedWithArray(int METHOD, T[] parameters)
            {
                if (loadFinishedWithArray != null)
                {
                    loadFinishedWithArray(METHOD, parameters);
                }
            }

            public void loadEvent(int METHOD, T[] parameters)
            {
                loadFinishedWithArray(METHOD, parameters);
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
            return e.ToString();
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
                                                new Dictionary<string, string> { { "URL", url } }, null);
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
                                   loadFinished(Constants.METHOD_ACCESS_TOKEN, null, null);
                               }, ex => MessageBox.Show(ReadWebException(ex)));
        }

        public void searchSong(string TYPE, string searchQuery)
        {
            var serializer = new DataContractJsonSerializer(typeof(SearchResult));

            var client = new OAuthClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET, accessToken)
                             {
                                 Url = Constants.SEARCH,
                                 Parameters = { { "type", TYPE }, { "q", searchQuery } }
                             };
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
                    t => loadFinished(Constants.METHOD_SEARCH, new Dictionary<string, string> { { "TYPE", TYPE } }, t),
                    ex => MessageBox.Show(ReadWebException(ex)));
        }


        public void getSelectedTags()
        {
            var serializer = new DataContractJsonSerializer(typeof(Tag[]));

            var client = new OAuthClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET, accessToken)
                             {
                                 Url = Constants.SELECTED_TAGS,
                                 Parameters = { { "type", "mood" } }
                             };
            client.GetResponseLines()
                .Select(s =>
                            {
                                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                                {
                                    Tag[] selectedTags;
                                    if (stream == null)
                                    {
                                        selectedTags = (Tag[])serializer.ReadObject(stream);
                                    }
                                    else
                                    {
                                        selectedTags = (Tag[])serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(s)));
                                    }
                                    return selectedTags;
                                }
                            })
                .ObserveOnDispatcher()
                .Subscribe(
                    t =>
                    {
                        ArrayEvent<Tag> arrayEvent = ArrayEvent<Tag>.getArrayEventInstance();
                        arrayEvent.loadEvent(Constants.METHOD_SELECTED_TAGS, t);
                    },
                    ex => MessageBox.Show(ReadWebException(ex)));
        }


        public void getTopTags()
        {
            var serializer = new DataContractJsonSerializer(typeof(Tag[]));

            var client = new OAuthClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET, accessToken)
            {
                Url = Constants.TOP_TAGS,
                // Parameters = { { "type", "mood" } }
            };
            client.GetResponseLines()
                .Select(s =>
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                    {
                        Tag[] selectedTags = (Tag[])serializer.ReadObject(stream);
                        return selectedTags;
                    }
                })
                .ObserveOnDispatcher()
                .Subscribe(
                    t =>
                    {
                        ArrayEvent<Tag> arrayEvent = ArrayEvent<Tag>.getArrayEventInstance();
                        arrayEvent.loadEvent(Constants.METHOD_TOP_TAGS, t);
                    },
                    ex => MessageBox.Show(ReadWebException(ex)));
        }

    }
}