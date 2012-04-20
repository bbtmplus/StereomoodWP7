using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Codeplex.OAuth;
using Microsoft.Phone.Reactive;
using TuneYourMood.Json;


namespace TuneYourMood
{
    public class OauthCommunication
    {
        private static OauthCommunication instance;

        public static Dictionary<string, string> KEYS_DICT;

        private RequestToken requestToken;
        private AccessToken accessToken;

        public static OauthCommunication getInstance()
        {
            return instance = instance ?? new OauthCommunication();
        }

        public static Dictionary<string, string> getKeysDict()
        {
            KEYS_DICT = new Dictionary<string, string>
                            {
                                {"5e1051156f4e8f3b8a0f6438cfbc0dc904f46cbb5", "e78745fd6af765b320b21e351982b5d5"},

                            };
            return KEYS_DICT;
        }

        public string getConsumerKey()
        {
            return "5e1051156f4e8f3b8a0f6438cfbc0dc904f46cbb5"; //TODO: make it actually random
        }

        public string getConsumerSecret()
        {
            return "e78745fd6af765b320b21e351982b5d5"; //TODO: make it actually random
        }
        public bool isConnected()
        {
            return (requestToken != null && accessToken != null);
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

            return ex != null && (ex.Message.ToString(CultureInfo.InvariantCulture).ToLower().Contains("notfound."))
                       ? Constants.ERROR_REBOOT_MESSAGE
                       : Constants.ERROR_MESSAGE;
        }


        public void getRequestToken()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                string key = getConsumerKey();
                string secret = getConsumerSecret();
                var authorizer = new OAuthAuthorizer(getConsumerKey(), getConsumerSecret());
                IObservable<TokenResponse<RequestToken>> collection = authorizer.GetRequestToken(Constants.REQUEST_TOKEN);
                collection.Select(res => res.Token)
                    .ObserveOnDispatcher()
                    .Subscribe(token =>
                                   {
                                       requestToken = token;
                                       var url = authorizer.BuildAuthorizeUrl(Constants.AUTHORIZE, token);
                                       loadFinished(Constants.METHOD_AUTHORIZATION,
                                                    new Dictionary<string, string> { { "URL", url } }, null);
                                   }, ex =>
                                          {
                                              MessageBox.Show(ReadWebException(ex));
                                          });
            }
        }


        public void getAccessToken(string pinCode)
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) return;
            var authorizer = new OAuthAuthorizer(getConsumerKey(), getConsumerSecret());
            authorizer.GetAccessToken(Constants.ACCESS_TOKEN, requestToken, pinCode)
                .ObserveOnDispatcher()
                .Subscribe(res =>
                               {
                                   accessToken = res.Token;
                                   loadFinished(Constants.METHOD_ACCESS_TOKEN, null, null);
                               }, ex => MessageBox.Show(ReadWebException(ex)));
        }



        public void searchSongs(string TYPE, string searchQuery)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var serializer = new DataContractJsonSerializer(typeof(SearchResult));

                var client = new OAuthClient(getConsumerKey(), getConsumerSecret(), accessToken)
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
        }

        public void searchSongs(Tag tag)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var serializer = new DataContractJsonSerializer(typeof(SearchResult));

                var client = new OAuthClient(getConsumerKey(), getConsumerSecret(), accessToken)
                                 {
                                     Url = Constants.SEARCH,
                                     Parameters = { { "type", tag.type }, { "q", tag.value }, { "limit", "20" } }
                                 };
                client.GetResponseLines()
                    .Select(s =>
                                {
                                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                                    return (SearchResult)serializer.ReadObject(stream);

                                })
                    .ObserveOnDispatcher()
                    .Subscribe(
                        t =>
                        loadFinished(Constants.METHOD_SEARCH,
                                     new Dictionary<string, string> { { "TYPE", tag.type }, { "VALUE", tag.value } }, t),
                        ex => MessageBox.Show(ReadWebException(ex)));
            }
        }

        /*
                public void getSelectedTags()
                {
                    if (!NetworkInterface.GetIsNetworkAvailable()) return;
                    var serializer = new DataContractJsonSerializer(typeof(Tag[]));

                    var client = new OAuthClient(getConsumerKey(), getConsumerSecret(), accessToken)
                                     {
                                         Url = Constants.SELECTED_TAGS,
                                         Parameters = { { "type", "mood" } }
                                     };
                    client.GetResponseLines()
                        .Select(s =>
                                    {
                                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));

                                        if (stream.Capacity > 0)
                                        {
                                            Tag[] selectedTags = null;
                                            try
                                            {
                                                selectedTags = (Tag[])serializer.ReadObject(stream);
                                            }
                                            catch (ArgumentNullException ex)
                                            {
                                                getSelectedTags();
                                            }
                                            return selectedTags;
                                        }
                                        else
                                        {
                                            getSelectedTags();
                                        }
                                        return null;

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
                    if (!NetworkInterface.GetIsNetworkAvailable()) return;
                    var serializer = new DataContractJsonSerializer(typeof(Tag[]));

                    var client = new OAuthClient(getConsumerKey(), getConsumerSecret(), accessToken)
                                     {
                                         Url = Constants.TOP_TAGS,
                                         Parameters = { { "type", "mood" } }
                                     };
                    client.GetResponseLines()
                        .Select(s =>
                                    {
                                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));

                                        if (stream.Capacity > 0)
                                        {
                                            Tag[] topTags = null;
                                            try
                                            {
                                                topTags = (Tag[])serializer.ReadObject(stream);
                                            }
                                            catch (ArgumentNullException ex)
                                            {
                                                getTopTags();
                                            }
                                            return topTags;
                                        }
                                        else
                                        {
                                            getTopTags();
                                        }
                                        return null;

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
                */
    }
}