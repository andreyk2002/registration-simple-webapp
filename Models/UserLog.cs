using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace registration_simple_webapp.Models
{
    public class UserLog
    {
        public long Id { get; set; }
        public string Password { get; set; }

        public UserLog(string Password)
        {
            this.Password = Password;
        }
    }
}
