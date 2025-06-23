using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP
{
    public enum Gender
    {
        [Description("男")]
        Male,
        [Description("女")]
        Female,
        [Description("保密")]
        Other
    }
}
