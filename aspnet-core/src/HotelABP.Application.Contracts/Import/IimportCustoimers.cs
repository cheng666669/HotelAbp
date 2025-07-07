using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Import
{
    public interface IimportCustoimers
    {
        Task<int> ImportCustoimers(Stream stream);
    }
}
