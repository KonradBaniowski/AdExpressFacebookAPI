using Facebook.Service.Core.DomainModels.MauSchema;
using System.Collections.Generic;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IOrderClientMediaRepository : IGenericRepository<OrderClientMedia>
    {
        List<OrderClientMedia> GetMediaRights(int idLogin);
    }
}
