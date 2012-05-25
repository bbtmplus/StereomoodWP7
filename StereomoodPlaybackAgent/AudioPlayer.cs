using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;


namespace StereomoodPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private readonly IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

        private static AudioPlayer instance;

        private List<Song> songList;

        private static int currentTrackNumber = 0;

        public static AudioPlayer Instance()
        {
            return instance = instance ?? new AudioPlayer();
        }

        public List<AudioTrack> _playList;

        private void PlayNextTrack(BackgroundAudioPlayer player)
        {
            if (_playList.Count == 0)
            {
                loadPlaylist();
            }
            else
                if (++currentTrackNumber >= _playList.Count)
                {
                    currentTrackNumber = 0;
                }
            StorageUtility.writeStringToFile(IsolatedStorageFile.GetUserStoreForApplication(),
                "CurrentTrackNumber.txt",
                currentTrackNumber.ToString(CultureInfo.InvariantCulture));
            player.Track = _playList[currentTrackNumber];
            player.Play();
        }


        private void PlayPreviousTrack(BackgroundAudioPlayer player)
        {
            if (_playList.Count == 0)
            {
                loadPlaylist();
            }
            else if (--currentTrackNumber < 0)
            {
                currentTrackNumber = _playList.Count - 1;

            }
            StorageUtility.writeStringToFile(IsolatedStorageFile.GetUserStoreForApplication(),
                "CurrentTrackNumber.txt",
                currentTrackNumber.ToString(CultureInfo.InvariantCulture));
            player.Track = _playList[currentTrackNumber];
            player.Play();
        }



        private void PlayTrack(BackgroundAudioPlayer player)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                currentTrackNumber = _playList.IndexOf(player.Track) == -1
                                         ? Convert.ToInt16(StorageUtility.readStringFromFile(isoStore,
                                                                                             "CurrentTrackNumber.txt"))
                                         : _playList.IndexOf(player.Track);

                if (_playList.Count > 0)
                {
                    if (!(player.Track != null && player.Track.Title == _playList[currentTrackNumber].Title) && player.PlayerState == PlayState.Playing)
                    {
                        player.Track = _playList[currentTrackNumber];
                    }
                }
                else
                {
                    loadPlaylist();
                }
                player.Play();
            }
        }

        private void loadPlaylist()
        {
            _playList.Clear();
            //  Song[] tracklist = StorageUtility.readListFromFile<Song>(isoStore, "SongList.txt").ToArray();
            Song[] tracklist = StorageUtility.readSongArrayFromFile(isoStore);
            if (tracklist.Length > 0)
            {
                foreach (var song in tracklist)
                {
                    _playList.Add(new AudioTrack(
                                      song.audio_url,
                                      song.title,
                                      song.artist,
                                      song.album,
                                      song.image_url));
                }
            }
        }

        public AudioPlayer()
        {
            _playList = new List<AudioTrack>();
            loadPlaylist();
        }

        /// Код для выполнения на необработанных исключениях
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackReady:
                    player.Play();
                    break;

                case PlayState.TrackEnded:
                    PlayNextTrack(player);
                    break;
                case PlayState.Shutdown:
                    // TODO: обработайте здесь состояние отключения (например, сохраните состояние)
                    break;
                case PlayState.Unknown:
                    break;
                case PlayState.Stopped:
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
            }

            NotifyComplete();
        }

        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    PlayTrack(player);
                    break;

                case UserAction.Pause:
                    try
                    {
                        if (player.CanPause)
                        {
                            player.Pause();
                        }
                    }catch(UnauthorizedAccessException ex)
                    {
                        // what the fuck??
                    }
                    break;

                case UserAction.SkipPrevious:
                    PlayPreviousTrack(player);
                    break;

                case UserAction.SkipNext:
                    PlayNextTrack(player);
                    break;

                case UserAction.Stop:
                    player.Stop();
                    break;

                case UserAction.FastForward:
                    player.FastForward();
                    break;

                case UserAction.Rewind:
                    player.Rewind();
                    break;

                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
            }

            NotifyComplete();
        }

        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                NotifyComplete();
            }

        }

        protected override void OnCancel()
        {

        }
    }
}
