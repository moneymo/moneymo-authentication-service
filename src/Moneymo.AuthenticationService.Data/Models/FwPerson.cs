using System;
using System.Collections.Generic;

namespace Moneymo.AuthenticationService.Data.Models
{
    public partial class FwPerson
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool? Status { get; set; }
        public int RoleId { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastUpd { get; set; }
        public int? LastUpdBy { get; set; }
        public int UpdServiceId { get; set; }
    }
}
