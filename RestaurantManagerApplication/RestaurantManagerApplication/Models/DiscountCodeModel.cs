using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{ 
    public class DiscountCodeModel
    {
        public int Id { get; set; }
        public double Percent { get; set; }
        public string Code { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int RestaurantID { get; set; }
    }
}
