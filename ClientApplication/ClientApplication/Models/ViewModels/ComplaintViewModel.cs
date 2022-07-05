using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class ComplaintViewModel
    {
        public List<ComplaintModel> Complaints { get; set; }

        public ComplaintViewModel(List<ComplaintModel> complaints)
        {
            Complaints = complaints ?? new();
        }
    }
}
