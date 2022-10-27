using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace entrance_test.Model
{
    public class UserRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string passwrord { get; set; }
    }
}
