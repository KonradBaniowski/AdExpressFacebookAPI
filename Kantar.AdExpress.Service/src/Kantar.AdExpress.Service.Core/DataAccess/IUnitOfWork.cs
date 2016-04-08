using Kantar.AdExpress.Service.Core.DataAccess.Repository;
using Kantar.AdExpress.Service.Core.DataAccess.Repository.Mau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.DataAccess
{
    public interface IUnitOfWork
    {
        void Save();
        IAddressRepository AddressRepository { get; set; }
        ILoginRepository LoginRepository { get; set; }
        IMyResultsRepository MyResultRepository { get; set; }
    }
}
