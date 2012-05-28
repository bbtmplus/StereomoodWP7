using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Devices;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Tasks;
using StereomoodPlaybackAgent;
using TuneYourMood.Json;
using Song = StereomoodPlaybackAgent.Song;

namespace TuneYourMood
{
    public partial class SongDetailsPage
    {
        private BitmapImage playImage;
        private BitmapImage pauseImage;

        private readonly IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

        private CurrentItemCollections itemCollections;

        private Song[] songs;

        public SongDetailsPage()
        {
            itemCollections = CurrentItemCollections.Instance();

            InitializeComponent();

            InitializeElements();

            InitializePlayer();
        }

        private void UpdateIcons()
        {
            if (itemCollections.favoritesDict != null && itemCollections.currentSong != null)
            {
                favoritesButton.IconUri = itemCollections.favoritesDict.ContainsKey(itemCollections.currentSong.audio_url.ToString()) ? new Uri("/Images/appbar.favs.remove.rest.png", UriKind.Relative) : new Uri("/Images/appbar.favs.addto.rest.png", UriKind.Relative);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            UpdateIcons();
            base.OnNavigatedTo(e);
        }

        private void InitializeElements()
        {
            Tag currentTag = itemCollections.currentMood;

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

            pauseImage = new BitmapImage(new Uri("/Images/appbar.transport.pause.rest.png", UriKind.Relative));
            playImage = new BitmapImage(new Uri("/Images/appbar.transport.play.rest.png", UriKind.Relative));
            BackgroundAudioPlayer.Instance.Track = null;

            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
            loadAppBackground();
            UpdateIcons();
        }

        private void InitializePlayer()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              if (NetworkInterface.GetIsNetworkAvailable())
                                                              {
                                                                  var value = itemCollections.currentMood.value;
                                                                  if (value != null)
                                                                      itemCollections.getSongsForTagDictionary().
                                                                          TryGetValue(value, out songs);

                                                                  StorageUtility.writeSongArrayToFile(storage, songs);


                                                                  if (PlayState.Playing ==
                                                                      BackgroundAudioPlayer.Instance.PlayerState)
                                                                  {
                                                                      playButton.IconUri = pauseImage.UriSource;
                                                                      songTextBox.Text =
                                                                          BackgroundAudioPlayer.Instance.Track.Title;
                                                                      authorTextBox.Text =
                                                                          BackgroundAudioPlayer.Instance.Track.Artist;
                                                                      artImageBrush.ImageSource =
                                                                          new BitmapImage(
                                                                              BackgroundAudioPlayer.Instance.Track.
                                                                                  AlbumArt);
                                                                  }
                                                                  else
                                                                  {
                                                                      BackgroundAudioPlayer.Instance.Play();
                                                                      playButton.IconUri = playImage.UriSource;
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  NotificationTool.Show("Offline",
                                                                                        "Sorry, the network is not available at the moment",
                                                                                        new NotificationAction(
                                                                                            "Okay :(",
                                                                                            () =>
                                                                                            {
                                                                                                throw new Exception(
                                                                                                    "ExitApp");
                                                                                            }));
                                                              }
                                                          });
        }

        private void loadAppBackground()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              Dictionary<string, BitmapImage> backgroundBrushes =
                                                                  itemCollections.backgroundBrushes;
                                                              var backgroundBrush = new ImageBrush
                                                                                        {
                                                                                            Opacity = 0.8d,
                                                                                            ImageSource =
                                                                                                backgroundBrushes[
                                                                                                    itemCollections.
                                                                                                        currentBackgroundKey
                                                                                                ]
                                                                                        };
                                                              LayoutRoot.Background = backgroundBrush;
                                                          });
        }

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    MediaHistoryItem item = MediaHistory.Instance.NowPlaying;
                    playButton.IconUri = pauseImage.UriSource;
                    break;

                case PlayState.Paused:
                    playButton.IconUri = playImage.UriSource;
                    break;
                case PlayState.Stopped:
                    playButton.IconUri = playImage.UriSource;
                    break;
            }

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                songTextBox.Text = BackgroundAudioPlayer.Instance.Track.Title;
                authorTextBox.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                artImageBrush.ImageSource = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
            }
        }

        #region Button Click Event Handlers

        private void previousClicked(object sender, EventArgs e)
        {
            UpdateIcons();
            BackgroundAudioPlayer.Instance.SkipPrevious();
            itemCollections.currentTrackNumber =
                Int16.Parse(StorageUtility.readObjectFromFile<string>(IsolatedStorageFile.GetUserStoreForApplication(),
                                                          "CurrentTrackNumber.txt"));
            itemCollections.currentSong = itemCollections.audioTracks[itemCollections.currentTrackNumber];
            UpdateIcons();
        }

        private void playClicked(object sender, EventArgs e)
        {
            if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
            {
                BackgroundAudioPlayer.Instance.Pause();
            }
            else
            {
                BackgroundAudioPlayer.Instance.Play();
            }
        }

        private void nextClicked(object sender, EventArgs e)
        {
            UpdateIcons();
            BackgroundAudioPlayer.Instance.SkipNext();
            itemCollections.currentTrackNumber =
                Int16.Parse(StorageUtility.readObjectFromFile<string>(IsolatedStorageFile.GetUserStoreForApplication(),
                                                          "CurrentTrackNumber.txt"));
            itemCollections.currentSong = itemCollections.audioTracks[itemCollections.currentTrackNumber];
            UpdateIcons();
        }

        private void shareClicked(object sender, EventArgs e)
        {
            string shareString = "";
            Tag currentTag = itemCollections.currentMood;

            if (currentTag.type.ToLower().Equals(Constants.TYPE_MOOD))
            {
                shareString = "I am listening to " + BackgroundAudioPlayer.Instance.Track.Title + " and feeling " + currentTag.value;
            }
            else if (currentTag.type.ToLower().Equals(Constants.TYPE_ACTIVITY))
            {
                shareString = "Hey, go on " + currentTag.value + "! I'm listening to " + BackgroundAudioPlayer.Instance.Track.Title + "";
            }
            ShareLinkTask shareLinkTask = new ShareLinkTask
                                              {
                                                  Title = "Enjoying Tune Your Mood for WP7",
                                                  LinkUri = BackgroundAudioPlayer.Instance.Track.Source,
                                                  Message = shareString
                                              };
            shareLinkTask.Show();
        }

        private void addToFavorites(object sender, EventArgs e)
        {
            if (itemCollections.favoritesDict != null && itemCollections.currentSong != null)
            {
                if (itemCollections.favoritesDict.ContainsKey(itemCollections.currentSong.audio_url.ToString()))
                {
                    itemCollections.favoritesDict.Remove(itemCollections.currentSong.audio_url.ToString());
                    itemCollections.SaveApplicationState();
                    favoritesButton.IconUri = new Uri("/Images/appbar.favs.addto.rest.png", UriKind.Relative);
                }
                else
                {
                    itemCollections.favoritesDict.Add(itemCollections.currentSong.audio_url.ToString(), itemCollections.currentSong);
                    StorageUtility.writeObjectToFile(IsolatedStorageFile.GetUserStoreForApplication(), "favoritesDict.txt", itemCollections.favoritesDict);
                    favoritesButton.IconUri = new Uri("/Images/appbar.favs.remove.rest.png", UriKind.Relative);
                    itemCollections.SaveApplicationState();

                }
            }
        }

        #endregion
    }
}