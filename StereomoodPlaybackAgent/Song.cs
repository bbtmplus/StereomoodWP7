
using System;
using System.Runtime.Serialization;

namespace StereomoodPlaybackAgent
{
    [DataContract]
    public class Song
    {
        [DataMember(Name = "identifier")]
        public string id { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "creator")]
        public string artist { get; set; }

        [DataMember(Name = "album")]
        public string album { get; set; }

        [DataMember(Name = "image")]
        public Uri _image_url;

        public Uri image_url
        {
            get
            {
                return _image_url.ToString().Equals("http://www.stereomood.com/gui/img/default_album_player.gif")
                        ? new Uri("http://dl.dropbox.com/u/7947878/album_placeholder.png")
                        : _image_url;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.image_url = value;
            }
        }

        [DataMember(Name = "location")]
        public Uri audio_url { get; set; }

        [DataMember(Name = "trackNum")]
        public int trackNumber { get; set; }

        [DataMember(Name = "code")]
        public string code { get; set; }

        [DataMember(Name = "owner")]
        public string owner { get; set; }
    }
}
