using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IOrderTemplateProductRepository : IGenericRepository<OrderTemplateProduct>
    {
        List<OrderTemplateProduct> GetTemplateProductRight(int idLogin);
    }
}
