
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stereomood.Json
{
    public class SelectedTagsResult : JsonObject
    {
        [DataMember(Name = "tags")]
        public List<Tag> tags { get; set; }
    }
}
