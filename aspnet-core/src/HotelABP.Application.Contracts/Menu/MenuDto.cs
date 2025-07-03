using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace HotelABP.Menu
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
        public bool IsMenu { get; set; }
        public bool IsVisible { get; set; }
        public List<MenuDto> Children { get; set; }
    }
}
