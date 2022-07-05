using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class RespondToComplaintViewModel
    {
        public int complaintId { get; set; }
        public string text { get; set; }
    }
}
