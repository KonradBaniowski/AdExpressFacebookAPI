using ExtensionMethods;
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using Facebook.Service.Core.DomainModels.MauSchema;
using LinqKit;
using System;
using System.Collections.Generic;
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

        public List<DataFacebook> GetDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<int> Advertiser, List<int> Brand)
        {
            var query = (from d in context.DataFacebook
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End
                         select d);

            var includedMedia = Criteria.Where(e => e.TypeCriteria.HasFlag(TypeCriteria.Include) && e.TypeNomenclature.HasFlag(TypeNomenclature.Media));
            var excludedMedia = Criteria.Where(e => e.TypeCriteria.HasFlag(TypeCriteria.Exclude) && e.TypeNomenclature.HasFlag(TypeNomenclature.Media));
            var includedProduct = Criteria.Where(e => e.TypeCriteria.HasFlag(TypeCriteria.Include) && e.TypeNomenclature.HasFlag(TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria.HasFlag(TypeCriteria.Exclude) && e.TypeNomenclature.HasFlag(TypeNomenclature.Product));
            
            var include1 = query.Predicate(includedMedia);
            var include2 = query.Predicate(includedProduct);
            var exclude1 = query.Predicate(excludedMedia);
            var exclude2 = query.Predicate(excludedProduct);
            Expression<Func<DataFacebook, bool>> predicate = arg => ((include1.Invoke(arg) || include2.Invoke(arg)) && (exclude1.Invoke(arg) && exclude2.Invoke(arg)));
            query = query.AsExpandable().Where(predicate);
            //query = query.ApplyRight(includedMedia);
            //query = query.ApplyRight(includedProduct);
            //query = query.ApplyRight(excludedMedia);
            //query = query.ApplyRight(excludedProduct);

            if (Brand != null && Brand.Count > 0)
            {
                query = query.Where(e => Brand.Cast<long>().Contains(e.IdBrand));
            }
            else if (Advertiser != null && Advertiser.Count > 0)
            {
                query = query.Where(e => Advertiser.Cast<long>().Contains(e.IdAdvertiser));
            }
            else
                return new List<DataFacebook>();
            return query.ToList();
            //        //query = query.Where(e => string.Join(",", ProductRights[61].Where(_ => _.Exception == 0).Select(_ => _.Rights)).Split(',').AsEnumerable<string>().Cast<long>().ToList().Contains(e.IdSubSector));
        }
    }
}
