using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersTask4.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool Flag { get; set; }


    }
}
