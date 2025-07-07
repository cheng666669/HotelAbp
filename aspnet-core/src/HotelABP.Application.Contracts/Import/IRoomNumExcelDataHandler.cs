using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Import
{
    public interface IRoomNumExcelDataHandler<TDto>
    {
        Task<int> HandleAsync(Stream stream);
    }
}
