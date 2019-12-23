using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WcfKaluga.Classes.Items;
using WcfKaluga.Classes.Results;

namespace WcfKaluga.Classes
{
    [DataContract]
    public class RollPack : IResultItem
    {
        [DataMember]
        public string RollPackNum { get; set; }

        [DataMember]
        public QualityStatus QualityStatus { get; set; }

        [DataMember]
        public List<Roll> Rolls { get; set; }
        
        [DataMember]
        public int SapStatus { get; set; } 
    }
}
