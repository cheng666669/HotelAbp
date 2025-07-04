using HotelABP.Customer;
using HotelABP.Customers;
using HotelABP.Labels;
using HotelABP.RoomNummbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using static Volo.Abp.Http.MimeTypes;

namespace HotelABP.Label
{
    /// <summary>
    /// 标签管理
    /// </summary>
    public class LabelService : ApplicationService, ILabelService
    {
        private readonly IRepository<HotelABPLabelss, Guid> _labelRepository;

        public LabelService(IRepository<HotelABPLabelss, Guid> labelRepository)
        {
            _labelRepository = labelRepository;
        }
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="ldto"></param>
        /// <returns></returns>
        public async Task<ApiResult<LabelDto>> AddLabelAsync(LabelDto ldto)
        {
            try
            {
                // 将 LabelDto 映射为 HotelABPLabels 实体对象
                var entity = ObjectMapper.Map<LabelDto, HotelABPLabelss>(ldto);
                // 插入实体对象到数据库，并返回插入后的实体
                var entitydto = await _labelRepository.InsertAsync(entity);
                // 将插入后的实体对象再次映射为 LabelDto
                var s = ObjectMapper.Map<HotelABPLabelss, LabelDto>(entitydto);
                // 返回带有插入结果的 ApiResult
                return ApiResult<LabelDto>.Success(s, ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败的 ApiResult
                return ApiResult<LabelDto>.Fail(ex.Message, ResultCode.Error);
            }
        }

        public async Task<ApiResult<PageResult<GetLabelDto>>> GetCustomerListAsync(Seach seach, GetLabeDtoList dtoList)
        {
            try
            {
                var list = await _labelRepository.GetQueryableAsync();
                list = list.WhereIf(!string.IsNullOrEmpty(dtoList.LabelName), x => x.LabelName.Contains(dtoList.LabelName));

                var res = list.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
                var dto = ObjectMapper.Map<List<HotelABPLabelss>, List<GetLabelDto>>(list.ToList());
                return ApiResult<PageResult<GetLabelDto>>.Success(
             new PageResult<GetLabelDto>
             {
                 Data = dto,
                 TotleCount = list.Count(),
                 TotlePage = (int)Math.Ceiling(list.Count() / (double)seach.PageSize)

             },
                         ResultCode.Success
          );
            }
            catch (Exception ex)
            { 
             
                // 捕获异常并返回失败的 ApiResult
                return ApiResult<PageResult<GetLabelDto>>.Fail(ex.Message, ResultCode.Error);
            }
        }
    }
}
