using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.Identity
{
    public class ApplicationRole
    {
        public ApplicationRole()
        {
            Users = new List<ApplicationUserRole>();
        }

        public int Id
        {
            get; set;
        }

        public virtual ICollection<ApplicationUserRole> Users { get; private set; }

        public string Name { get; set; }
    }
}
