using HotelABP.RoomNummbers;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.RoomNumms
{
    /// <summary>
    /// 房号管理
    /// </summary>
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "roomnum")]
    public class RoomNummServices:ApplicationService,IRoomNummberService
    {
        IRepository<RoomNummber, Guid> _roomNummberRepository;
        IRepository<RoomType, Guid> _roomTypeRepository;
        IDistributedCache<RoomNummber> _cache;
        public RoomNummServices(IRepository<RoomNummber, Guid> roomNummberRepository, IDistributedCache<RoomNummber> cache, IRepository<RoomType, Guid> roomTypeRepository)
        {
            _roomNummberRepository = roomNummberRepository;
            _cache = cache;
            _roomTypeRepository = roomTypeRepository;
        }
        /// <summary>
        /// 新增房号（支持DTO映射）
        /// </summary>
        /// <param name="input">房号新增DTO</param>
        /// <returns>新增后的房号DTO</returns>
        /// <remarks>
        /// 1. 使用ObjectMapper将输入DTO映射为实体。
        /// 2. 插入数据库。
        /// 3. 映射为返回DTO。
        /// </remarks>
        public async Task<ApiResult<RoomNummDto>> CreateRoomNumAdd(CreateUpdataRoomNummDto input)
        {
            var entity=ObjectMapper.Map<CreateUpdataRoomNummDto, RoomNummber>(input);
            var entityDto=await _roomNummberRepository.InsertAsync(entity);
            var dto=ObjectMapper.Map<RoomNummber, RoomNummDto>(entityDto);
            return ApiResult<RoomNummDto>.Success(dto, ResultCode.Success);
        }

        /// <summary>
        /// 根据房型Id查询房号列表（支持分页和条件筛选）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="input">房型ID请求DTO</param>
        /// <returns>分页后的房号DTO列表</returns>
        /// <remarks>
        /// 1. 查询所有房号。
        /// 2. 按房型ID筛选。
        /// 3. DTO映射。
        /// 4. 分页返回。
        /// </remarks>
        public async Task<ApiResult<PageResult<RoomNummDto>>> GetListToRoomTypeId(Seach seach, RoomNummRoomTypeRequestDto input)
        {
            var queryable = await _roomNummberRepository.GetQueryableAsync();

            queryable = queryable
                .WhereIf(!string.IsNullOrWhiteSpace(input.RoomTypeId), x => x.RoomTypeId == input.RoomTypeId);

            var dto=ObjectMapper.Map<List<RoomNummber>, List<RoomNummDto>>(queryable.ToList());
            var res = dto.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
            return ApiResult<PageResult<RoomNummDto>>.Success(
               new PageResult<RoomNummDto>
               {
                   Data = res.Queryable.OrderByDescending(x => x.Order).ToList(),
                   TotleCount = queryable.Count(),
                    TotlePage = (int)Math.Ceiling(queryable.Count() / (double)seach.PageSize)
               },
               ResultCode.Success);
        }
        /// <summary>
        /// 批量删除房号（支持事务）
        /// </summary>
        /// <param name="ids">房号ID集合</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务，保证批量删除的原子性。
        /// 2. 遍历ID集合，依次删除房号。
        /// 3. 提交事务。
        /// </remarks>
        public async Task<ApiResult<bool>> DeleteRoomNumBatch(List<Guid> ids)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var id in ids)
                {
                    await _roomNummberRepository.DeleteAsync(id);
                }
                tran.Complete();
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
        }
        /// <summary>
        /// 删除房号（单个）
        /// </summary>
        /// <param name="Id">房号ID</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据ID删除房号。
        /// </remarks>
        public async Task<ApiResult<bool>> DeleteRoomNum(Guid Id)
        {
            await _roomNummberRepository.DeleteAsync(Id);
            return ApiResult<bool>.Success(true, ResultCode.Success);
        }
        /// <summary>
        /// 设置房号状态(上架/下架)
        /// </summary>
        /// <param name="id">房号ID</param>
        /// <param name="state">状态（true上架，false下架）</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据ID查找房号。
        /// 2. 修改状态字段。
        /// 3. 更新数据库。
        /// 4. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> UpdateStateToRoomNum(Guid id,bool state)
        {
            try
            {
                var entity = await _roomNummberRepository.GetAsync(id);
                entity.State = state;
                await _roomNummberRepository.UpdateAsync(entity);
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 批量设置房号状态（上架下架，支持事务）
        /// </summary>
        /// <param name="ids">房号ID集合</param>
        /// <param name="state">状态（true上架，false下架）</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务，保证批量操作的原子性。
        /// 2. 遍历ID集合，依次修改状态。
        /// 3. 提交事务。
        /// 4. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> UpdateStateToRoomNumBatch(List<Guid> ids,bool state)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var id in ids)
                    {
                        var entity = await _roomNummberRepository.GetAsync(id);
                        entity.State = state;
                        await _roomNummberRepository.UpdateAsync(entity);
                    }
                    tran.Complete();
                    return ApiResult<bool>.Success(true, ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 条件查询显示页面（支持多条件筛选和分页，房型联表）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="input">筛选条件DTO</param>
        /// <returns>分页后的房号DTO列表</returns>
        /// <remarks>
        /// 1. 查询所有房号。
        /// 2. 查询所有房型。
        /// 3. 联表查询，组装DTO。
        /// 4. 支持房型ID、状态、房号多条件筛选。
        /// 5. 分页返回。
        /// </remarks>
        public async Task<ApiResult<PageResult<RoomNummDto>>> GetRoomNumList(Seach seach, GetRoomNummberQuery input)
        {
            // 查询数据库
            //预订信息
            var list = await _roomNummberRepository.GetQueryableAsync();
            //房间类型
            var roomTypes = await _roomTypeRepository.GetQueryableAsync();

            var listdto = from roomnum in list
                          join roomType in roomTypes on roomnum.RoomTypeId equals roomType.Id.ToString()
                          select new RoomNummDto
                          {
                              Id = roomnum.Id,
                               RoomTypeId = roomType.Id.ToString(),
                               TypeName = roomType.Name,
                               RoomNum = roomnum.RoomNum,
                               State = roomnum.State,
                               Order = roomnum.Order,
                               Description = roomnum.Description
                          };

            listdto = listdto
               .WhereIf(!string.IsNullOrEmpty(input.RoomTypeId), x => x.RoomTypeId == input.RoomTypeId)
               .WhereIf(input.State != null, x => x.State == input.State)
                .WhereIf(!string.IsNullOrEmpty(input.RoomNum), x => x.RoomNum == input.RoomNum);
            var res = listdto.PageResult(seach.PageIndex, seach.PageSize);
            return ApiResult<PageResult<RoomNummDto>>.Success(
                new PageResult<RoomNummDto>
                {
                    Data = res.Queryable.ToList(),
                    TotleCount = res.RowCount,
                    TotlePage = (int)Math.Ceiling(res.RowCount / (double)seach.PageSize)
                },
                ResultCode.Success);
        }

        /// <summary>
        /// 修改房号（支持DTO映射）
        /// </summary>
        /// <param name="Id">房号ID</param>
        /// <param name="input">房号修改DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据ID查找房号。
        /// 2. 用ObjectMapper将DTO属性映射到实体。
        /// 3. 更新数据库。
        /// 4. 捕获异常并抛出。
        /// </remarks>
        public async Task<ApiResult<int>> UpdateRoomNumm(Guid Id, CreateUpdataRoomNummDto input) 
        {
            try
            {
                var queryable = await _roomNummberRepository.GetAsync(Id);
                // 映射为 DTO 返回
                var dto = ObjectMapper.Map< CreateUpdataRoomNummDto, RoomNummber>(input, queryable);
                await _roomNummberRepository.UpdateAsync(dto);
                return ApiResult<int>.Success(1, ResultCode.Success);
            }
            catch (Exception)
            {
                throw;
            }
        }


        
            
    }
}
