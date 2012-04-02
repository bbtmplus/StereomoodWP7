using System.Runtime.Serialization;

namespace TuneYourMood.Json
{
    [KnownType(typeof(JsonObject))]
    public class Tag : JsonObject
    {
        [DataMember(Name = "value")]
        public string value { get; set; }

        [DataMember(Name = "type")]
        public string type { get; set; }

        [DataMember(Name = "weight")]
        public string weight { get; set; }
    }
}
