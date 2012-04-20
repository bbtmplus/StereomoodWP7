
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StereomoodPlaybackAgent;


namespace TuneYourMood.Json
{
    [KnownType(typeof(JsonObject))]
    public class SearchResult : JsonObject
    {
        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "tracksTotal")]
        public int tracksTotal { get; set; }

        [DataMember(Name = "creator")]
        public string creator { get; set; }

        [DataMember(Name = "trackList")]
        public Song[] trackList { get; set; }
    }
}
