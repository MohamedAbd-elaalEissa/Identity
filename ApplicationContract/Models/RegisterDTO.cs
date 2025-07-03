using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContract.Models
{
    public class RegisterDTO
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six charaters")]
        public string Password { get; set; }
        public int? academicLevelId { get; set; }

        public bool IsTeacher { get; set; }
    }
}
