using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotocorn.Common.Attributes
{
    public class KotocornOptions : Attribute
    {
        public Type OptionType { get; set; }

        public KotocornOptions(Type t)
        {
            this.OptionType = t;
        }
    }
}
