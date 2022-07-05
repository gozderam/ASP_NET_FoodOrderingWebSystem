using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class RestaurantMenuViewModel
    {
        public RestaurantViewModel restaurant { get; set; }
        public MenuSectionViewModel[] sections { get; set; }
    }
}
