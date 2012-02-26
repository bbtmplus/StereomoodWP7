
using System.Runtime.Serialization;

namespace Stereomood.Json
{
    public class Song : JsonObject
    {
        [DataMember(Name = "id")]
        public string id        { get; set; }

        [DataMember(Name = "title")]
        public string title     { get; set; }

        [DataMember(Name = "artist")]
        public string artist    { get; set; }

        [DataMember(Name = "album")]
        public string album     { get; set; }

        [DataMember(Name = "url")]
        public string url       { get; set; }

        [DataMember(Name = "image_url")]
        public string image_url { get; set; }

        [DataMember(Name = "audio_url")]
        public string audio_url { get; set; }

        [DataMember(Name = "post_url")]
        public string post_url  { get; set; }
    }
}
