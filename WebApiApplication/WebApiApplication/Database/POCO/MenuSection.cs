using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Database;

namespace WebApiApplication.Database.POCO
{
    public class MenuSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<MenuPosition> MenuPositions { get; set; }
    }
}
