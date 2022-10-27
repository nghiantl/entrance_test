using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace entrance_test.Model
{
    public class SignInResponse
    {
        public UserResponse user { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
