using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IOrderClientProductRepository
    {
        List<OrderClientProduct> GetProductRights(int idLogin);
    }
}
