using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Service.Core.DomainModels.BusinessModel;
using ExtensionMethods;
using System.Linq.Expressions;
using LinqKit;
using System.Data.Entity;

namespace Facebook.DataAccess.Repository
{
    public class DataPostFacebookRepository : GenericRepository<DataPostFacebook>, IDataPostFacebookRepository
    {
        public DataPostFacebookRepository(FacebookContext context) : base(context)
        {
           
        }

        public PostFacebook GetDataPostFacebook(long idPostFacebook,int idLanguage)
        {
            var query = (from dp in context.DataPostFacebook
                         join ap in context.Products on dp.IdBrand equals ap.BrandId
                         where dp.IdPostFacebook == idPostFacebook
                         //&& ap.LanguageId == idLanguage
                         select new PostFacebook
                         {
                             IdPostFacebook = dp.IdPostFacebook,
                             IdPost = dp.IdPost,
                             Advertiser = ap.Advertiser,
                             Brand = ap.Brand,
                             DateCreationPost = dp.DateCreationPost,
                             Commitments = dp.Commitment,
                             NumberLikes = dp.NumberLike,
                             NumberShares = dp.NumberShare,
                             NumberComments = dp.NumberComment,
                             // PageName = df.PageName, TODO : voir Eric pour creeer la page name dans datasot facebook
                         });
            return query.FirstOrDefault();
        }

        public List<PostFacebook> GetDataPostFacebook(List<CriteriaData> criteria, long begin, long end, List<long> advertisers, List<long> brands, List<long> pages, int idLanguage)
        {
            var beginDate = new DateTime(Convert.ToInt32(begin.ToString().Substring(0, 4)), Convert.ToInt32(begin.ToString().Substring(4, 2))
                , Convert.ToInt32(begin.ToString().Substring(6, 2)));
            var endDate = new DateTime(Convert.ToInt32(end.ToString().Substring(0, 4)), Convert.ToInt32(end.ToString().Substring(4, 2)),
                Convert.ToInt32(end.ToString().Substring(6, 2)));

            //Dernière minutes de la journée
            endDate = endDate.AddDays(1).AddTicks(-1);

            var includedProduct = criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            var query = (from df in context.DataFacebook
                         select df);

            Expression<Func<DataFacebook, bool>> predicate = null;


            if (pages == null || !pages.Any()) //Do not apply right on facebook pages
            {
                if (includedProduct != null && includedProduct.Any())
                {
                    var include2 = query.Predicate(includedProduct);
                    predicate = arg => include2.Invoke(arg);
                }

                if (excludedProduct != null && excludedProduct.Any())
                {
                    var exclude2 = query.Predicate(excludedProduct);
                    predicate = arg => !(exclude2.Invoke(arg));
                }
                if (predicate != null)
                    query = query.AsExpandable().Where(predicate);
            }


           

            if (brands != null && brands.Count > 0)
            {
                query = query.Include(a => a.Brand)
                    .Where(e => brands.Contains(e.IdBrand));

            }
            else if (advertisers != null && advertisers.Count > 0)
            {
                query = query.Include(a => a.Advertiser)
                    .Where(e => advertisers.Contains(e.IdAdvertiser));
            }


            var query1 = (from dp in context.DataPostFacebook
                          select dp);
            if (pages != null && pages.Any())
            {
                query1 = query1.Where(p => pages.Contains(p.IdPageFacebook));
            }


            var query2 = (from df in query
                          join dp in query1 on df.IdPageFacebook equals dp.IdPageFacebook
                          join ap in context.Products on new { pd = df.IdProduct } equals new { pd = ap.ProductId }
                          orderby ap.Advertiser
                          where df.DateMediaNum >= begin && df.DateMediaNum <= end
                          && dp.DateCreationPost >= beginDate.Date && dp.DateCreationPost <= endDate
                          && ap.LanguageId == idLanguage
                          select new PostFacebook
                          {
                              IdPostFacebook = dp.IdPostFacebook,
                              IdPost = dp.IdPost,
                              Advertiser = ap.Advertiser,
                              Brand = ap.Brand,
                              DateCreationPost = dp.DateCreationPost,
                              Commitments = dp.Commitment,
                              NumberLikes = dp.NumberLike,
                              NumberShares = dp.NumberShare,
                              NumberComments = dp.NumberComment,
                              PageName = df.PageName,
                          });

            return query2.Distinct().ToList();

        }
    }
}
