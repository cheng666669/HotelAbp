using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP
{
    public class PageResult<T>
    {
        public int TotleCount { get; set; }
        public int TotlePage { get; set; }
        public IList<T> Data { get; set; }
    }
    public class Seach
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
