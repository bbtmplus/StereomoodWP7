using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using Stereomood.Json;
using Song = StereomoodPlaybackAgent.Song;


namespace Stereomood
{
    public partial class SongListPage : PhoneApplicationPage
    {
        public List<Song> songs;
        public Tag currentTag { get; set; }
        public SearchResult searchResult { get; set; }
        private OauthCommunication oauthCommunication;
        private CurrentItemCollections itemCollections;

        public SongListPage()
        {
            InitializeComponent();

            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();
            itemCollections = CurrentItemCollections.Instance();

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (searchResult != null && searchResult.total > 0)
                {
                    songs = searchResult.songs;
                    songList.ItemsSource = songs;
                    if (!itemCollections.getSongsForTagDictionary().ContainsKey(currentTag.value))
                    {
                        itemCollections.getSongsForTagDictionary().Add(currentTag.value, songs);
                    }
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
            var itemContainerGenerator = this.songList.ItemContainerGenerator;
            if (itemContainerGenerator != null)
            {
                ListBoxItem selectedItem = itemContainerGenerator.ContainerFromIndex(2) as ListBoxItem;
                if (selectedItem != null)
                {
                    Song song = selectedItem.DataContext as Song;
                    if (song != null)
                    {
                        CurrentItemCollections.Instance().currentSong = song;
                        Uri songDetailsUri = new Uri("/SongDetailsPage.xaml", UriKind.Relative);
                        NavigationService.Navigate(songDetailsUri);
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SongDetailsPage songDetailsPage = (e.Content as SongDetailsPage);
            if (songDetailsPage != null)
                // songDetailsPage.searchResult = searchResult;
                base.OnNavigatedFrom(e);
        }
    }
}