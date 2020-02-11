using System;
using System.Collections.Generic;

namespace Moneymo.AuthenticationService.Data.Models
{
    public partial class FwSession
    {
        public int Id { get; set; }
        public string SessionKey { get; set; }
        public DateTime Created { get; set; }
        public DateTime Valid { get; set; }
        public int PersonId { get; set; }
        public int UpdServiceId { get; set; }
        public int ChannelId { get; set; }
    }
}
