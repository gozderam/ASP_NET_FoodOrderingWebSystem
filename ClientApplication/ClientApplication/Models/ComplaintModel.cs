using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models
{
    public class ComplaintModel
    {
        public string Content { get; set; }

        public string Response { get; set; }

        public bool Open { get; set; }

        public int OrderId { get; set; }
    }
}
