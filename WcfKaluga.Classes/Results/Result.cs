using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfKaluga.Classes.Results
{
    [DataContract]
    public class Result<TResultItem>
    {
        [DataMember]
        public string Message { get; set; }
        
        [DataMember]
        public TResultItem ResultItem { get; set; }
    }
}
