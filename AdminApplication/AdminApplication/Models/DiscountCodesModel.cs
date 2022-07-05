using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.Models
{
    public class DiscountCodesViewModel
    {
        public List<DiscountCodeModel> globalCodes;
        public List<DiscountCodeModel> restaurantsCodes;

        public DiscountCodesViewModel(DiscountCodeModel[] discountCodes)
        {
            this.globalCodes = new List<DiscountCodeModel>();
            this.restaurantsCodes = new List<DiscountCodeModel>();

            if (discountCodes == null)
                return;

            foreach (var d in discountCodes)
            {
                if (d.RestaurantId != null)
                {
                    this.restaurantsCodes.Add(d);
                }
                else
                {
                    this.globalCodes.Add(d);
                }
            }
        }
    }

    public class DiscountCodeModel
    {
        public int Id { get; set; }

        public double Percent { get; set; }

        public string Code { get; set; }

        public int? RestaurantId { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }
    }

    public class NewDiscountCodeModel
    {
        [Required]
        [Range(1.0, 99.0)]
        public double Percent { get; set; }

        [Required]
        public string Code { get; set; }

        public int? RestaurantId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        public bool AreDatesValid()
        {
            //var from = DateTime.Parse(this.DateFrom);
            //var to = DateTime.Parse(this.DateTo);
            var today = DateTime.Today;
            if (DateTime.Compare(today, this.DateFrom) >= 0 || DateTime.Compare(this.DateFrom, this.DateTo) >= 0)
            {
                return false;
            }
            return true;
        }
    }
}
