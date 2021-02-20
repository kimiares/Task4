using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersTask4.ViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
