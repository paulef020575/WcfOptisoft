using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WcfKaluga.Classes.Results;

namespace WcfKaluga.Classes.Items
{
    [DataContract]
    public class RollQuality : Roll
    {
        [DataMember]
        public string RollNum { get; set; }
        
        [DataMember]
        public List<QualityParam> QualityParams { get; set; }
    }
}
