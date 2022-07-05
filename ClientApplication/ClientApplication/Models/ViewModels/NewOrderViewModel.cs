using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class NewOrderViewModel
    {
        public PaymentMethodModel paymentMethod { get; set; }
        public string discountcodeId { get; set; }
        public int restaurantId { get; set; }
        public int[] positions { get; set; }
        public int[] quantities { get; set; }
        public AddressViewModel address { get; set; }
    }
}
