using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Service.Core.DomainModels.BusinessModel;
using ExtensionMethods;
namespace Facebook.DataAccess.Repository
{
    public class DataPostFacebookRepository : GenericRepository<DataPostFacebook>, IDataPostFacebookRepository
    {
        public DataPostFacebookRepository(FacebookContext context) : base(context)
        {
        }

        public List<PostFacebook> GetDataPostFacebook(List<CriteriaData> criteria, long begin, long end, List<long> advertisers, List<long> brands, List<long> posts)
        {
            var beginDate = new DateTime(Convert.ToInt32(begin.ToString().Substring(0, 4)), Convert.ToInt32(begin.ToString().Substring(4, 2))
                , Convert.ToInt32(begin.ToString().Substring(6, 2)));
            var endDate = new DateTime(Convert.ToInt32(end.ToString().Substring(0, 4)), Convert.ToInt32(end.ToString().Substring(4, 2)),
                Convert.ToInt32(end.ToString().Substring(6, 2)));

            var includedProduct = criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            var query = (from df in context.DataFacebook
                         select df);
            var include2 = query.Predicate(includedProduct);
            var exclude2 = query.Predicate(excludedProduct);

            var query2 = (from df in query
                         join dp in context.DataPostFacebook on df.IdPageFacebook equals dp.IdPageFacebook
                         join ap in context.Products on new { pd = df.IdProduct } equals new { pd = ap.ProductId }
                         orderby ap.Advertiser
                         where df.DateMediaNum >= begin && df.DateMediaNum <= end
                         && dp.DateCreationPost >= beginDate.Date && dp.DateCreationPost <= endDate.Date
                         select new PostFacebook
                         {
                             IdPostFacebook = dp.IdPostFacebook,
                             IdPost = dp.IdPost,
                             Advertiser = ap.Advertiser,
                             Brand = ap.Brand,
                             DateCreationPost = dp.DateCreationPost,
                             Commitment = dp.Commitment,
                             NumberLike = dp.NumberLike,
                             NumberShare = dp.NumberShare,
                             NumberComment = dp.NumberComment
                         });



            return query2.ToList();
        }
    }
}
