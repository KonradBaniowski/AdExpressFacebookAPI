
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System.Collections.Generic;

namespace Facebook.DataAccess.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(FacebookContext context) : base(context)
        {

        }
        public List<Product> Search(string keyword)
        {
            return new List<Product>();
        }
    }
}
