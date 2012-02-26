
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stereomood.Json
{
    public class SearchResult : JsonObject
    {
        [DataMember(Name = "total")]
        public string total            { get; set; }

        [DataMember(Name = "songs")]
        public List<Song> songs { get; set; }
    }
}
