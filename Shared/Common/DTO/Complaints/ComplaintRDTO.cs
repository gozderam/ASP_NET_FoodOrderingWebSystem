using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class ComplaintRDTO
    {
        [Required]
        public int id { get; set; }

        [Required]
        public string content { get; set; }

        public string response { get; set; }

        [Required]
        public bool open { get; set; }

        [Required]
        public int orderId { get; set; }

        public EmployeeDTO employee { get; set; }
    }
}
