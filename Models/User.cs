
using System;

namespace registration_simple_webapp.Models
{
    public class User 
    {        
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public User()
        {

        }
        public User(string Name, string Email, string Password)
        {
            this.Password = Password;
            this.Name = Name;
            this.Email = Email;
            Status = "unblocked";
            RegistrationDate = LastLoginDate = DateTime.Now;
        }
    }
}
