using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.BackgroundAudio;
using StereomoodPlaybackAgent;
using Song = StereomoodPlaybackAgent.Song;

namespace TuneYourMood
{
    public partial class SongDetailsPage
    {
        private readonly BitmapImage playImage;
        private readonly BitmapImage pauseImage;

        private readonly IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

        private CurrentItemCollections itemCollections;

        private Song[] songs;

        public SongDetailsPage()
        {
            InitializeComponent();

            itemCollections = CurrentItemCollections.Instance();


            pauseImage = new BitmapImage(new Uri("/Images/appbar.transport.pause.rest.png", UriKind.Relative));
            playImage = new BitmapImage(new Uri("/Images/appbar.transport.play.rest.png", UriKind.Relative));
            BackgroundAudioPlayer.Instance.Track = null;

            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
        }

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
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
                artImage.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          

            if (NetworkInterface.GetIsNetworkAvailable())
            {


                var value = itemCollections.currentMood.value;
                if (value != null)
                    itemCollections.getSongsForTagDictionary().TryGetValue(value, out songs);
                //StorageUtility.writeListToFile(storage, "SongList.txt", songs);
                StorageUtility.writeSongArrayToFile(storage, songs);
                //int ctn = CurrentItemCollections.Instance().currentTrackNumber;
                //  BackgroundAudioPlayer.Instance.Track =
                //      CurrentItemCollections.Instance().convertSongToAudioTrack(songs[ctn]);

                BackgroundAudioPlayer.Instance.Play();
                if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
                {
                    playButton.IconUri = pauseImage.UriSource;
                    songTextBox.Text = BackgroundAudioPlayer.Instance.Track.Title;
                    authorTextBox.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                    artImage.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                }
                else
                {
                    playButton.IconUri = playImage.UriSource;
                }
            }
            else
            {
                NotificationTool.Show("Offline",
                                      "Sorry, the network is not available at the moment",
                                      new NotificationAction("Okay :(", () => { throw new Exception("ExitApp"); }));
            }
        }


        #region Button Click Event Handlers

        private void previousClicked(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipPrevious();
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
            BackgroundAudioPlayer.Instance.SkipNext();
        }


        #endregion

    }
}