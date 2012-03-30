using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Stereomood.Json;
using StereomoodPlaybackAgent;
using Song = StereomoodPlaybackAgent.Song;

namespace Stereomood
{
    public partial class SongDetailsPage : PhoneApplicationPage
    {
        private readonly BitmapImage playImage;
        private readonly BitmapImage pauseImage;

        private CurrentItemCollections itemCollections;
        private Song currentSong;
        private readonly Tag currentTag;
        public double Duration { get; set; }
        private readonly Dictionary<string, List<Song>> playList;
        readonly DependencyProperty property = DependencyProperty.Register("Progress", typeof(double), typeof(PhoneApplicationPage), new PropertyMetadata(0.0));
        private Song[] songs;

        public double Progress
        {
            get
            {
                return (double)GetValue(property);
            }
            set
            {
                SetValue(property, value);
            }
        }


        public SongDetailsPage()
        {
            InitializeComponent();

            itemCollections = CurrentItemCollections.Instance();

            playList = CurrentItemCollections.Instance().getSongsForTagDictionary();
            currentSong = CurrentItemCollections.Instance().currentSong;
            currentTag = CurrentItemCollections.Instance().currentMood;

            pauseImage = new BitmapImage(new Uri("Images/appbar.transport.pause.rest.png", UriKind.Relative));
            playImage = new BitmapImage(new Uri("Images/appbar.transport.play.rest.png", UriKind.Relative));
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
                case PlayState.Stopped:
                    playButton.Source = playImage;
                    break;
            }

            if (null != BackgroundAudioPlayer.Instance.Track)
            {
                songTextBox.Text = BackgroundAudioPlayer.Instance.Track.Title;
                authorTextBox.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                artImage.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            songs = playList[CurrentItemCollections.Instance().currentMood.value].ToArray();
            StorageUtility.AddOrUpdateValue("tracklist", songs);
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