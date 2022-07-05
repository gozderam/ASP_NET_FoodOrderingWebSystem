using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public double Rate { get; set; }
        public Client Client { get; set; }
        public Restaurant Restaurant { get; set; }

    }
}
