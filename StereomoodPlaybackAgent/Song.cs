
using System;
using System.Runtime.Serialization;

namespace StereomoodPlaybackAgent
{
    [DataContract]
    public class Song
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "artist")]
        public string artist { get; set; }

        [DataMember(Name = "album")]
        public string album { get; set; }

        [DataMember(Name = "url")]
        public Uri url { get; set; }

        [DataMember(Name = "image_url")]
        public Uri image_url { get; set; }

        [DataMember(Name = "audio_url")]
        public Uri audio_url { get; set; }

        [DataMember(Name = "post_url")]
        public Uri post_url { get; set; }
    }
}
