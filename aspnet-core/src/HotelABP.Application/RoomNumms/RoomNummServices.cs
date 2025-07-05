using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomNummbers;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.ObjectMapping;

namespace HotelABP.RoomNumms
{
    [IgnoreAntiforgeryToken]
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
        /// 新增房号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ApiResult<RoomNummDto>> CreateRoomNumAdd(CreateUpdataRoomNummDto input)
        {
            var roomnum = await _roomNummberRepository.FindAsync(x => x.RoomNum == input.RoomNum);
            if (roomnum != null)
            {
                return ApiResult<RoomNummDto>.Fail("此房号已存在", ResultCode.Error);
            }
            var entity = ObjectMapper.Map<CreateUpdataRoomNummDto, RoomNummber>(input);
            var entityDto = await _roomNummberRepository.InsertAsync(entity);
            var dto = ObjectMapper.Map<RoomNummber, RoomNummDto>(entityDto);
            return ApiResult<RoomNummDto>.Success(dto, ResultCode.Success);
        }


        /// <summary>
        /// 根据房型Id查询房号列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// 批量删除房号
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// 删除房号
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> DeleteRoomNum(Guid Id)
        {
            await _roomNummberRepository.DeleteAsync(Id);
            return ApiResult<bool>.Success(true, ResultCode.Success);
        }
        /// <summary>
        /// 设置房号状态(上架/下架)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// 批量设置房号状态（上架下架）
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// 条件查询显示页面
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// 修改房号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
