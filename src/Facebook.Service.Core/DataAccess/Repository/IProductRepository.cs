using Facebook.Service.Core.DomainModels.AdExprSchema;
using System.Collections.Generic;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IProductRepository : IGenericRepository<LevelItem>
    {
        List<LevelItem> Search(string keyword);
    }
}
