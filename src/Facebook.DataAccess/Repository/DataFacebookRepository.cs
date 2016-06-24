using ExtensionMethods;
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using Facebook.Service.Core.DomainModels.MauSchema;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class DataFacebookRepository : GenericRepository<DataFacebook>, IDataFacebookRepository
    {
        public DataFacebookRepository(FacebookContext context) : base(context)
        {
        }

        public List<DataFacebook> GetDataFacebook()
        {

            var query = (from d in context.DataFacebook
                         select d);

            return new List<DataFacebook>();
        }

        public List<DataFacebook> GetDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand)
        {
            var query = (from d in context.DataFacebook
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End
                         select d);

            var includedMedia = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Media));
            var excludedMedia = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Media));
            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            var include1 = query.Predicate(includedMedia);
            var include2 = query.Predicate(includedProduct);
            var exclude1 = query.Predicate(excludedMedia);
            var exclude2 = query.Predicate(excludedProduct);
            Expression<Func<DataFacebook, bool>> predicate = arg => ((include1.Invoke(arg) || include2.Invoke(arg)) && !(exclude1.Invoke(arg) && exclude2.Invoke(arg)));
            query = query.AsExpandable().Where(predicate);
            if (Brand != null && Brand.Count > 0)
            {
               
                query = query.Where(e => Brand.Contains(e.IdBrand));
                //query.GroupBy(e => new { e.IdPageFacebook, e.IdAdvertiser}).Select(c => new DataFacebookKPI
                //{
                //    AdvertiserLabel = c.First().Advertiser.AdvertiserLabel,
                //    IdPageFacebook = c.First().IdPageFacebook,
                //    IdAdvertiser = c.First().IdAdvertiser,
                //    NumberPost = c.Sum(e => e.NumberPost),
                //    NumberLike = c.Sum(e => e.NumberLike),
                //    NumberShare = c.Sum(e => e.NumberShare),
                //    Expenditure = c.Sum(e => e.Expenditure),
                //    NumberFan = c.Max(e => e.NumberFan),

                //});
            }
            else if (Advertiser != null && Advertiser.Count > 0)
            {
                //query = query.Join(context.Advertiser,
                //   e => e.IdAdvertiser,
                //   a => a.IdAdvertiser,
                //   (e, a) => new DataFacebook
                //   {
                //       Advertiser = a
                //   });
                query = query.Include(a => a.Advertiser).Where(e => Advertiser.Contains(e.IdAdvertiser));
            }
            else
                return new List<DataFacebook>();
            return query.ToList();
        }
    }
}
