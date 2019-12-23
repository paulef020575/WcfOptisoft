using System.Runtime.Serialization;

namespace WcfKaluga.Classes.Items
{
    [DataContract]
    public class Roll 
    {
        [DataMember]
        public string RollNumber { get; set; }

        [DataMember]
        public QualityStatus Quality { get; set; }
    }
}
