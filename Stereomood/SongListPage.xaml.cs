using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using Stereomood.Json;

namespace Stereomood
{
    public partial class SongListPage : PhoneApplicationPage
    {
        private List<Song> songs;
        public Tag selectedTag { get; set; }
        public SearchResult searchResult { get; set; }
        private OauthCommunication oauthCommunication;

        public SongListPage()
        {
            InitializeComponent();

            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (searchResult != null && searchResult.total > 0)
                {
                    songs = searchResult.songs;
                    songList.ItemsSource = songs;
                }
                else
                {
                    NotificationTool.Show("Sorry",
                                          "Couldn't load songs. Please try once again.",
                                          new NotificationAction("Okay", () => { }));
                    NavigationService.GoBack();
                }
            }
        }

        private void songSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}