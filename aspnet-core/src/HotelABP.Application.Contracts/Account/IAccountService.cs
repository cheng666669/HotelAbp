﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Account
{
    public interface IAccountService:IApplicationService
    {
        Task<ApiResult> AddAccount();
    }
}
