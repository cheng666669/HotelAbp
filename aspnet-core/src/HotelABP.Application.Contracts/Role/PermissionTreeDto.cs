﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Role
{
    public class PermissionTreeDto:EntityDto<Guid>
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否选中（用于角色权限分配时标记）
        /// </summary>
        public bool IsSelected { get; set; } = false;
        
        /// <summary>
        /// 子级权限
        /// </summary>
        public List<PermissionTreeDto> Children { get; set; } = new List<PermissionTreeDto>();
    }
}
