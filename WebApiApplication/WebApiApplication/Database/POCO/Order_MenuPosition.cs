using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Database.POCO
{
    public class Order_MenuPosition
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public MenuPosition MenuPosition { get; set; }
        public int PositionsInOrder { get; set; }
    }
}
