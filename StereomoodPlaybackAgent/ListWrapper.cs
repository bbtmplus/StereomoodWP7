using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StereomoodPlaybackAgent
{
    [DataContract]
    public class ListWrapper<T>
    {
        [DataMember]
        public List<T> Elements { get; set; }
    }
}
