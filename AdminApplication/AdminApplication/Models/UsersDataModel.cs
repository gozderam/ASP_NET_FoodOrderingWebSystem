using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.Models
{
    public class UsersDataViewModel
    {
        public UserDataModel[] users;
    }

    public class UserDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public AddressModel Address { get; set; }
    }

    public class AddressModel
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Postcode { get; set; }
    }
}
