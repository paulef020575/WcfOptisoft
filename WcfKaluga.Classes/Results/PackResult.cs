using System.Runtime.Serialization;

namespace WcfKaluga.Classes.Results
{
    [DataContract]
    public class PackResult 
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public RollPack Package { get; set; }
    }
}
