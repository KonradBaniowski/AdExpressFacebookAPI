using Kantar.AdExpress.Service.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.DataAccess.Repository;
using Kantar.AdExpress.Service.Core.DataAccess.Repository.Mau;
using Kantar.AdExpress.Service.DataAccess.Repository.Mau;
using Kantar.AdExpress.Service.DataAccess.Repository;

namespace Kantar.AdExpress.Service.DataAccess
{
    public class AdExpressUnitOfWork : IUnitOfWork
    {
    

        private AdExpressContext _context;


        public AdExpressUnitOfWork(AdExpressContext context)
        {
            _context = context;
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            LoginRepository = new LoginRepository(_context);
            //AddressRepository = new AddressRepository(_context);
        }

        //public IAddressRepository AddressRepository { get; set; }
        public ILoginRepository LoginRepository { get; set; }

        public IMyResultsRepository MyResultRepository { get; set; }


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
