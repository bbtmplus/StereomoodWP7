using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using DeepForest.Phone.Assets.Tools;
using RestSharp;
using StereomoodPlaybackAgent;
using TuneYourMood.Json;

namespace TuneYourMood
{
    public class RestApiCommunication
    {
        private static RestApiCommunication instance;

        public static Dictionary<string, string> KEYS_DICT;

        public static RestApiCommunication getInstance()
        {
            return instance = instance ?? new RestApiCommunication();
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

        #region Delegates & Events

        public class LoadEvent<T>
        {

            private static LoadEvent<T> arrayEventInstance;

            public static LoadEvent<T> getArrayEventInstance()
            {
                return arrayEventInstance = arrayEventInstance ?? new LoadEvent<T>();
            }

            public delegate void loadFinishedEventHandler(int METHOD, T[] items, Dictionary<string, string> parameters);

            public event loadFinishedEventHandler loadFinished;

            public void onLoadFinished(int METHOD, T[] items, Dictionary<string, string> parameters)
            {
                if (loadFinished != null)
                {
                    loadFinished(METHOD, items, parameters);
                }
            }

            public void loadEvent(int METHOD, T[] items, Dictionary<string, string> parameters)
            {
                loadFinished(METHOD, items, parameters);

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
                if (parameters != null)
                {
                    loadFinishedWithArray(METHOD, parameters);
                }
            }
        }

        #endregion

        public void searchSongs(Tag tag)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var client = new RestClient(Constants.SEARCH);
                var request = new RestRequest(tag.type + "/" + tag.value + "/playlist.json?shuffle&index=0", Method.GET);

                client.ExecuteAsync(request, response =>
                {
                    var s = new DataContractJsonSerializer(typeof(SearchResult));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    SearchResult objectResponse = s.ReadObject(ms) as SearchResult;
                    ms.Close();
                    if (objectResponse != null && objectResponse.tracksTotal > 0)
                    {
                        LoadEvent<Song> loadEvent = LoadEvent<Song>.getArrayEventInstance();
                        loadEvent.loadEvent(Constants.METHOD_SEARCH,
                            objectResponse.trackList,
                            new Dictionary<string, string> { { "TYPE", tag.type }, { "VALUE", tag.value } });
                    }
                    else
                    {
                        NotificationTool.Show("Error",
                                      "Sorry, couldn't find anything like that. Could you be more precise?",
                                      new NotificationAction("Okay", () =>
                                      {
                                          LoadEvent<Song> loadEvent = LoadEvent<Song>.getArrayEventInstance();
                                          loadEvent.loadEvent(Constants.METHOD_RESET, null, null);
                                      }));
                    }
                });
            }
        }

        public void searchSongs(string TYPE, string searchQuery)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var client = new RestClient(Constants.SEARCH);
                var request = new RestRequest(TYPE + "/" + searchQuery + "/playlist.json?shuffle&index=0", Method.GET);

                client.ExecuteAsync(request, response =>
                                                 {
                                                     var s = new DataContractJsonSerializer(typeof(SearchResult));
                                                     var ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                                                     SearchResult objectResponse = s.ReadObject(ms) as SearchResult;
                                                     ms.Close();
                                                     if (objectResponse != null && objectResponse.tracksTotal > 0)
                                                     {
                                                         LoadEvent<Song> loadEvent = LoadEvent<Song>.getArrayEventInstance();
                                                         loadEvent.loadEvent(Constants.METHOD_SEARCH,
                                                             objectResponse.trackList,
                                                             new Dictionary<string, string> { { "TYPE", TYPE }, { "VALUE", searchQuery } });

                                                     }
                                                     else
                                                     {
                                                         NotificationTool.Show("Error", "Sorry, couldn't find anything like that. Could you be more precise?",
                                                             new NotificationAction("Okay", () =>
                                                             {
                                                                 LoadEvent<Song> loadEvent = LoadEvent<Song>.getArrayEventInstance();
                                                                 loadEvent.loadEvent(Constants.METHOD_RESET, null, null);
                                                             }));
                                                     }
                                                 });
            }
        }

        public void getSelectedTags()
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) return;

            var client = new RestClient(Constants.SELECTED_TAGS);
            var request = new RestRequest("", Method.POST);

            client.ExecuteAsync(request, response =>
                                             {
                                                 MemoryStream ms = null;
                                                 try
                                                 {
                                                     var serializer = new DataContractJsonSerializer(typeof(Tag[]));
                                                     ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                                                     Tag[] objectResponse = serializer.ReadObject(ms) as Tag[];
                                                     ms.Close();
                                                     if (objectResponse == null || objectResponse.Length <= 0) return;
                                                     ArrayEvent<Tag> arrayEvent = ArrayEvent<Tag>.getArrayEventInstance();
                                                     arrayEvent.loadEvent(Constants.METHOD_SELECTED_TAGS, objectResponse);
                                                 }
                                                 finally
                                                 {
                                                     if (ms != null)
                                                     {
                                                         ms.Close();
                                                     }

                                                 }
                                             });
        }

        public void getTopTags()
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) return;


            var client = new RestClient(Constants.TOP_TAGS);
            var request = new RestRequest("", Method.POST);

            client.ExecuteAsync(request, response =>
            {
                MemoryStream ms = null;
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(Tag[]));
                    ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    Tag[] objectResponse = serializer.ReadObject(ms) as Tag[];

                    if (objectResponse == null || objectResponse.Length <= 0) return;
                    ArrayEvent<Tag> arrayEvent = ArrayEvent<Tag>.getArrayEventInstance();
                    arrayEvent.loadEvent(Constants.METHOD_TOP_TAGS, objectResponse);
                }
                finally
                {
                    if (ms != null)
                    {
                        ms.Close();
                    }

                }
            });
        }
    }
}
