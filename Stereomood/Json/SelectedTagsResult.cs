
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TuneYourMood.Json
{
    [KnownType(typeof(JsonObject))]
    public class SelectedTagsResult : JsonObject
    {
        [DataMember(Name = "tags")]
        public List<Tag> tags { get; set; }
    }
}
