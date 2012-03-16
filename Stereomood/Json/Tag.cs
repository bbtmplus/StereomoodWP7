using System.Runtime.Serialization;

namespace Stereomood.Json
{
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
