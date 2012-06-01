using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using TuneYourMood.Json;
using Song = StereomoodPlaybackAgent.Song;


namespace TuneYourMood
{
    public partial class SongListPage : PhoneApplicationPage
    {
        public Song[] songs;
        public Tag currentTag { get; set; }
        public SearchResult searchResult { get; set; }
        private CurrentItemCollections itemCollections;

        public SongListPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Dictionary<string, BitmapImage> backgroundBrushes = CurrentItemCollections.Instance().backgroundBrushes;
            ImageBrush backgroundBrush = new ImageBrush
            {
                Opacity = 0.8d,
                ImageSource = backgroundBrushes[CurrentItemCollections.Instance().currentBackgroundKey]
            };
            LayoutRoot.Background = backgroundBrush;

            base.OnNavigatedTo(e);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            itemCollections = CurrentItemCollections.Instance();
            currentTag = itemCollections.currentMood;
            this.SetValue(RadTileAnimation.ContainerToAnimateProperty, this.songList);
            if (currentTag != null)
            {
                if (currentTag.type.ToLower().Equals(Constants.TYPE_ACTIVITY))
                {
                    tagTitle.Text = "hey! go on, " + currentTag.value;
                }
                else
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
                if (searchResult != null && searchResult.tracksTotal > 0)
                {
                    songs = searchResult.trackList;
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
                                          new NotificationAction("Okay", () => { throw new Exception(); }));
                }
            }
            else
            {
                NotificationTool.Show("Offline",
                                     "Sorry, the network is not available at the moment",
                                     new NotificationAction("Okay :(", () => { }));
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

                }
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            songList.SelectedIndex = -1;
        }
    }
}