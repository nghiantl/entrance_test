using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace entrance_test.Model
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string displayName
        {
            get
            {
                return firstName + lastName;
            }
            set
            {
                displayName = firstName + lastName;
            }
        }
    }
}
