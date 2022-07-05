using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.Models
{
    public class UserComplaintViewModel
    {
        public List<ComplaintModel> Complaints { get; set; }
        public int UserId { get; set; }

        public UserComplaintViewModel(List<ComplaintModel> complaints, int userId)
        {
            Complaints = complaints ?? new();
            UserId = userId;
        }
    }

    public class RestaurantComplaintViewModel
    {
        public List<ComplaintModel> Complaints { get; set; }
        public int RestaurantId { get; set; }

        public RestaurantComplaintViewModel(List<ComplaintModel> complaints, int restuarantId)
        {
            Complaints = complaints ?? new();
            RestaurantId = restuarantId;
        }
    }

    public class ComplaintModel
    {
        public string Content { get; set; }

        public string Response { get; set; }

        public bool Open { get; set; }

        public int OrderId { get; set; }
    }
}
