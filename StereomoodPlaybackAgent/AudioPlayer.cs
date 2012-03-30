using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;


namespace StereomoodPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {

        private static AudioPlayer instance;

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

            PlayTrack(player);
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

            PlayTrack(player);
        }



        private void PlayTrack(BackgroundAudioPlayer player)
        {

            if (PlayState.Paused == player.PlayerState)
            {
                player.Play();
            }
            else if (_playList.Count > 0)
            {
                player.Track = _playList[currentTrackNumber];
                player.Play();
            }
            else
            {
                Song[] tracklist = StorageUtility.PickValueOrDefault<Song[]>("tracklist");
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
                PlayTrack(player);
            }

        }

        private void loadPlaylist()
        {
            //_playList.Clear();
            Song[] tracklist = StorageUtility.PickValueOrDefault<Song[]>("tracklist");
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

        /// <summary>
        /// Вызывается при изменении состояния воспроизведения, за исключением состояния ошибки (см. OnError)
        /// </summary>
        /// <param name="player">BackgroundAudioPlayer</param>
        /// <param name="track">Дорожка, воспроизводимая во время изменения состояния воспроизведения</param>
        /// <param name="playState">Новое состояние воспроизведения проигрывателя</param>
        /// <remarks>
        /// Изменения состояния воспроизведения невозможно отменить. Они вызываются, даже если изменение состояния
        /// было вызвано самим приложением при условии, что в приложении используется обратный вызов.
        ///
        /// Важные события playstate: 
        /// (а) TrackEnded: вызывается, когда в проигрывателе нет текущей дорожки. Агент может задать следующую дорожку.
        /// (б) TrackReady: звуковая дорожка задана и готова для воспроизведения.
        ///
        /// Вызовите NotifyComplete() только один раз после завершения запроса агента, включая асинхронные обратные вызовы.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackReady:
                    // The track to play is set in the PlayTrack method.
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


        /// <summary>
        /// Вызывается при запросе пользователем действия с помощью пользовательского интерфейса приложения или системы
        /// </summary>
        /// <param name="player">BackgroundAudioPlayer</param>
        /// <param name="track">Дорожка, воспроизводимая во время действия пользователя</param>
        /// <param name="action">Действие, запрошенное пользователем</param>
        /// <param name="param">Данные, связанные с запрошенным действием.
        /// В текущей версии этот параметр используется только с действием поиска
        /// для обозначения запрошенного положения в звуковой дорожке</param>
        /// <remarks>
        /// Действия пользователя не изменяют автоматически состояние системы; за выполнение действий
        /// пользователя, если они поддерживаются, отвечает агент.
        ///
        /// Вызовите NotifyComplete() только один раз после завершения запроса агента, включая асинхронные обратные вызовы.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    PlayTrack(player);
                    break;

                case UserAction.Pause:
                    player.Pause();
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

        /// <summary>
        /// Вызывается в случае ошибки воспроизведения, например, если звуковая дорожка не загружается правильно
        /// </summary>
        /// <param name="player">BackgroundAudioPlayer</param>
        /// <param name="track">Дорожка, в которой произошла ошибка</param>
        /// <param name="error">Произошедшая ошибка</param>
        /// <param name="isFatal">При значении true воспроизведение дорожки невозможно и будет остановлено</param>
        /// <remarks>
        /// Вызов этого метода во всех случаях не гарантируется. Например, если в фоновом агенте 
        /// произошло необработанное исключение, он не будет вызываться для обработки своих ошибок.
        /// </remarks>
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

        /// <summary>
        /// Вызывается при отмене запроса агента
        /// </summary>
        /// <remarks>
        /// После отмены запроса агент завершает работу в течение 5 секунд
        /// путем вызова NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }
    }
}
