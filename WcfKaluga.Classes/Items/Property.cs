using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfKaluga.Classes.Items
{
    /// <summary>
    ///     Класс "Свойство объекта"
    /// </summary>
    [DataContract]
    public class Property 
    {
        /// <summary>
        ///     Код свойство
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        ///     Значение свойства
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
