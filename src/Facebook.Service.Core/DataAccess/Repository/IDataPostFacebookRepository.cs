using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataPostFacebookRepository : IGenericRepository<DataPostFacebook>
    {
        List<PostFacebook> GetDataPostFacebook(List<CriteriaData> criteria, long begin, long end, List<long> advertiser, List<long> brand, List<long> post);
    }
}
