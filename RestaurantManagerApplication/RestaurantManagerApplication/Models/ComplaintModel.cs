using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagerApplication.Models
{
    public class ComplaintViewModel
    {
        public List<ComplaintModel> Complaints { get; set; }

        public ComplaintViewModel(List<ComplaintModel> complaints)
        {
            Complaints = complaints ?? new();
        }
    }

    public class ComplaintModel
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public string Response { get; set; }

        public bool Open { get; set; }

        public int OrderId { get; set; }

        public EmployeeModel AttendingEmployee { get; set; }
    }
}
