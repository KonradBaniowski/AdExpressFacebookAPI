using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.Identity
{
    public class ApplicationUserClaim
    {
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
    }
}
