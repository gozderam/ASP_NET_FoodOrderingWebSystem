using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Models.ViewModels
{
    public class MenuSectionViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public MenuPositionViewModel[] positions { get; set; }
    }
}
