
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StereomoodPlaybackAgent;


namespace TuneYourMood.Json
{
    [KnownType(typeof(JsonObject))]
    public class SearchResult : JsonObject
    {
        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "songs")]
        public List<Song> songs { get; set; }
    }
}
