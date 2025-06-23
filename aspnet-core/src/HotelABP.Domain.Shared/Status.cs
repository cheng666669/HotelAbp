using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP
{
    public enum Status
    {
        [Description("禁用")]
        Disable = 0,
        [Description("正常")]
        Enable
    }
}
