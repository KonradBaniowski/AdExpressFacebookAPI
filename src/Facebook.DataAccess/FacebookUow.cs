using Facebook.DataAccess.Repository;
using Facebook.Service.Core.DataAccess;
using Facebook.Service.Core.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facebook.DataAccess
{
    public class FacebookUow : IFacebookUow, IDisposable
    {
        private readonly FacebookContext _context;

        public FacebookUow(FacebookContext context)
        {
            _context = context;
            InstanciateRepository();
        }

        private void InstanciateRepository()
        {
            this.OrderClientMediaRepository = new OrderClientMediaRepository(_context);
            this.OrderTemplateMediaRepository = new OrderTemplateMediaRepository(_context);
            this.TemplateAssignmentRepository = new TemplateAssignmnetRepository(_context);
            this.ProductRepository = new ProductRepository(_context);
            this.DataFacebookRepository = new DataFacebookRepository(_context);
            this.OrderClientProductRepository = new OrderClientProductRepository(_context);
            this.OrderTemplateProductRepository = new OrderTemplateProductRepository(_context);
            this.DataPostFacebookRepository = new DataPostFacebookRepository(_context);
            this.WebPlanMediaMonthRepository = new WebPlanMediaMonthRepository(_context);

        }

        public IOrderClientMediaRepository OrderClientMediaRepository { get; private set; }
        public IOrderTemplateMediaRepository OrderTemplateMediaRepository { get; private set; }
        public ITemplateAssignmentRepository TemplateAssignmentRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IDataFacebookRepository DataFacebookRepository { get; private set; }
        public IDataPostFacebookRepository DataPostFacebookRepository { get; private set; }
        public IOrderClientProductRepository OrderClientProductRepository { get; private set; }
        public IOrderTemplateProductRepository OrderTemplateProductRepository { get; private set; }
        public IWebPlanMediaMonthRepository WebPlanMediaMonthRepository { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }

        #region Implement IDisposable

        private bool disposed = false;

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

        #endregion
    }
}