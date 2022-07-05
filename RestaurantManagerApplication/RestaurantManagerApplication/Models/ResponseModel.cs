using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public string ActionToGoBackTo { get; set; }
        public string ControllerToGoBackTo { get; set; }
    }
}
