using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using TuneYourMood.Json;
using StereomoodPlaybackAgent;
using Song = StereomoodPlaybackAgent.Song;


namespace TuneYourMood
{
    public partial class SongListPage : PhoneApplicationPage
    {
        public List<Song> songs;
        public Tag currentTag { get; set; }
        public SearchResult searchResult { get; set; }
        private CurrentItemCollections itemCollections;

        public SongListPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            itemCollections = CurrentItemCollections.Instance();
            currentTag = itemCollections.currentMood;
            if (currentTag != null)
            {
                if (currentTag.type.ToLower().Equals(Constants.TYPE_MOOD))
                {
                    tagTitle.Text = "I'm feeling " + currentTag.value;
                }
                else if (currentTag.type.ToLower().Equals(Constants.TYPE_ACTIVITY))
                {
                    tagTitle.Text = "hey! go on, " + currentTag.value;
                }
            }
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (searchResult != null && searchResult.total > 0)
                {
                    songs = searchResult.songs;
                    songList.ItemsSource = songs;
                    if (currentTag != null && !itemCollections.getSongsForTagDictionary().ContainsKey(currentTag.value))
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
            else
            {
                NotificationTool.Show("Offline",
                                     "Sorry, the network is not available at the moment",
                                     new NotificationAction("Okay :(", () => { throw new Exception("ExitApp"); }));
            }
        }

        private void songSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Song song = ((Song)listBox.SelectedItem);
                if (song != null)
                {
                    int ctn = itemCollections.currentTrackNumber = songs.IndexOf(song);
                    StorageUtility.writeStringToFile(IsolatedStorageFile.GetUserStoreForApplication(),
                        "CurrentTrackNumber.txt",
                        ctn.ToString(CultureInfo.InvariantCulture));

                    itemCollections.currentSong = song;
                    Uri songDetailsUri = new Uri("/SongDetailsPage.xaml", UriKind.Relative);
                    NavigationService.Navigate(songDetailsUri);
                }
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            songList.SelectedIndex = -1;
        }
    }
}