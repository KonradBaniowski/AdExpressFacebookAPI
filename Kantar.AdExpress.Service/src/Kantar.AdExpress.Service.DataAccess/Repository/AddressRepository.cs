using Kantar.AdExpress.Service.Core.DataAccess.Repository;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.Repository
{
    public class AddressRepository : GenericRepository<AdExpressContext, Address>, IAddressRepository
    {
        public AddressRepository(AdExpressContext context) : base(context)
        {

        }
    }
}
