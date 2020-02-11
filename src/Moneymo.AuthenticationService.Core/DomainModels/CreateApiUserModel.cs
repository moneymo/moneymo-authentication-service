using System;
using System.Collections.Generic;
using System.Text;

namespace Moneymo.AuthenticationService.Core.DomainModels
{
    public class CreateApiUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
    }
}
