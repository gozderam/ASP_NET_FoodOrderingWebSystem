using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class AllReviewsViewModel
    {
        public ReviewViewModel[] Reviews { get; set; }
        public RestaurantViewModel Restaurant { get; set; }
    }
}
