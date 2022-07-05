using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class CreateMenuPositoinViewModel
    {
        public NewMenuPositionModel MenuPosition {get;set;}
        public MenuSectionModel[] Sections { get; set; }
    }
}
