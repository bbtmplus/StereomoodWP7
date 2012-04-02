using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows.Input;
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

        private CurrentItemCollections itemCollections;

        private List<Song> songs;

        public SongDetailsPage()
        {
            InitializeComponent();

            itemCollections = CurrentItemCollections.Instance();


            pauseImage = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));
            playImage = new BitmapImage(new Uri("Images/play.png", UriKind.Relative));
            BackgroundAudioPlayer.Instance.Track = null;

            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;

        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    playButton.Source = pauseImage;
                    break;

                case PlayState.Paused:
                    playButton.Source = playImage;
                    break;
                case PlayState.Stopped:
                    playButton.Source = playImage;
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
                var value = CurrentItemCollections.Instance().currentMood.value;
                if (value != null)
                    songs = CurrentItemCollections.Instance().getSongsForTagDictionary()[value];
                StorageUtility.writeListToFile(IsolatedStorageFile.GetUserStoreForApplication(), "SongList.txt", songs);
                int ctn = CurrentItemCollections.Instance().currentTrackNumber;
                BackgroundAudioPlayer.Instance.Track =
                    CurrentItemCollections.Instance().convertSongToAudioTrack(songs[ctn]);

                BackgroundAudioPlayer.Instance.Play();
                if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
                {
                    playButton.Source = pauseImage;
                    songTextBox.Text = BackgroundAudioPlayer.Instance.Track.Title;
                    authorTextBox.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                    artImage.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                }
                else
                {
                    playButton.Source = playImage;
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

        private void PreviousButton_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!e.IsInertial)
            {
                BackgroundAudioPlayer.Instance.SkipPrevious();
            }

        }

        private void PlayButton_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!e.IsInertial)
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
        }

        private void NextButton_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!e.IsInertial)
            {
                BackgroundAudioPlayer.Instance.SkipNext();
            }
        }

        #endregion


    }
}