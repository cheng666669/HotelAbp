using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Store
{
    [ApiExplorerSettings(GroupName = "store")]
    public class StoreService : ApplicationService, IStoreService
    {
        private readonly IRepository<StoreInfo, Guid> storeRep;

        public StoreService(IRepository<StoreInfo,Guid> storeRep)
        {
            this.storeRep = storeRep;
        }
        /// <summary>
        /// 创建门店
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult> CreateStore(CreateUpdateStoreDto dto)
        {
            try
            {
                var res = await storeRep.FindAsync(x => x.StoreName == dto.StoreName);
                if (res != null)
                {
                    return ApiResult.Fail("门店已存在", ResultCode.ValidationError);
                }
                var stores = ObjectMapper.Map<CreateUpdateStoreDto, StoreInfo>(dto);
                stores.CreatorId = dto.UserId;
                await storeRep.InsertAsync(stores);
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取门店列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="storeDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<PageResult<StoreResultDto>>> GetStorelist(Seach seach, SearchStoreDto storeDto)
        {
            try
            {
                var query = await storeRep.GetQueryableAsync();
                query = query.WhereIf(!string.IsNullOrEmpty(storeDto.StoreName), x => x.StoreName.Contains(storeDto.StoreName)).WhereIf(!string.IsNullOrEmpty(storeDto.Address), x => x.Address.Contains(storeDto.Address)).WhereIf(storeDto.Status != null, x => x.Status == storeDto.Status);
                var dto = ObjectMapper.Map<List<StoreInfo>, List<StoreResultDto>>(query.ToList());
                var res = dto.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
                return ApiResult<PageResult<StoreResultDto>>.Success(
                new PageResult<StoreResultDto>
                {
                    Data = res.Queryable.ToList(),
                    TotleCount = query.Count(),
                    TotlePage = (int)Math.Ceiling(query.Count() / (double)seach.PageSize)
                },
                ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 修改门店状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut]
        public async Task<ApiResult> UpdateStatus(Guid id)
        {
            try
            {
                var res = await storeRep.GetAsync(id);
                if(res.Status==true)
                {
                    res.Status = false;
                }
                else if(res.Status==false)
                {
                    res.Status = true;
                }
                await storeRep.UpdateAsync(res);
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 修改门店信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut]
        public async Task<ApiResult> UpdateStoreinfo(Guid id, CreateUpdateStoreDto dto)
        {
            try
            {
                var res = await storeRep.GetAsync(id);
                if (res == null)
                {
                    return ApiResult.Fail("门店不存在", ResultCode.NotFound);
                }
                var stores = ObjectMapper.Map<CreateUpdateStoreDto, StoreInfo>(dto);
                stores.LastModifierId = dto.UserId;
                await storeRep.UpdateAsync(stores);
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
