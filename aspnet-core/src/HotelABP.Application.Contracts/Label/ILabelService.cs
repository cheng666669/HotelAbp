using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Label
{
    public interface ILabelService :IApplicationService
    {
        Task<ApiResult<LabelDto>> AddLabelAsync(LabelDto ldto);
    }
}
