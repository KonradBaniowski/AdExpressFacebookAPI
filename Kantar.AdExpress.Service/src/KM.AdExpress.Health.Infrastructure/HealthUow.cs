using KM.AdExpress.Health.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KM.AdExpress.Health.Core.Interfaces.Repository;
using KM.AdExpress.Health.Infrastructure.Repository;

namespace KM.AdExpress.Health.Infrastructure
{
    public class HealthUow : IHealthUow, IDisposable
    {

        private readonly HealthContext _context;

        public HealthUow(HealthContext context)
        {
            _context = context;
            InstanciateRepository();
        }

        private void InstanciateRepository()
        {
            this.DataCostRepository = new DataCostRepository(_context);
            
        }

        #region Implement IDisposable

        private bool disposed = false;

        public IDataCostRepository DataCostRepository { get; private set; }
        

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        #endregion

    }
}
