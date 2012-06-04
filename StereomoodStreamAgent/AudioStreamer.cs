using Microsoft.Phone.BackgroundAudio;

namespace StereomoodStreamAgent
{
    /// <summary>
    /// Фоновый агент, выполняющий потоковую передачу каждой дорожки для воспроизведения
    /// </summary>
    public class AudioTrackStreamer : AudioStreamingAgent
    {
        /// <summary>
        /// Вызывается, когда для новой дорожки требуется декодирование звука
        /// (обычно перед началом воспроизведения)
        /// </summary>
        /// <param name="track">
        /// Дорожка, для которой требуется потоковая передача звука
        /// </param>
        /// <param name="streamer">
        /// Объект AudioStreamer, к которому должен быть прикреплен MediaStreamSource
        /// для начала воспроизведения
        /// </param>
        /// <remarks>
        /// Для вызова этого метода для дорожки установите для параметра Source элемента AudioTrack значение null,
        /// перед тем как установить для свойства Track экземпляра BackgroundAudioPlayer
        /// значение true;
        /// в противном случае предполагается, что потоковая передача
        /// и декодирование будут выполняться системой
        /// </remarks>
        protected override void OnBeginStreaming(AudioTrack track, AudioStreamer streamer)
        {
            //TODO: установите для свойства SetSource средства потоковой передачи источник MSS

            NotifyComplete();
        }

        /// <summary>
        /// Вызывается при отмене запроса агента
        /// Вызов base.OnCancel() необходим для освобождения ресурсов потоковой передачи
        /// </summary>
        protected override void OnCancel()
        {
            base.OnCancel();
        }
    }
}
