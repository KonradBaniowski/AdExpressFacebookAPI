using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Service.Core.DomainModels.BusinessModel;
using ExtensionMethods;
using LinqKit;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Facebook.DataAccess.Repository
{
    public class DataSearchRepository : GenericRepository<DataSearch>, IDataSearchRepository
    {
        public DataSearchRepository(FacebookContext context) : base(context)
        {

        }

        public List<DataSearch> GetDataSearchWithCriteria(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var query = (from d in context.DataSearch
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End
                         select d);

            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            Expression<Func<DataSearch, bool>> predicate = null;

            if (includedProduct != null && includedProduct.Any())
            {
                var include2 = query.Predicate(includedProduct);
                predicate = arg => (include2.Invoke(arg));
            }
            if (excludedProduct != null && excludedProduct.Any())
            {
                var exclude2 = query.Predicate(excludedProduct);
                predicate = arg => !(exclude2.Invoke(arg));
            }

            if (predicate != null)
                query = query.AsExpandable().Where(predicate);

            if (Brand != null && Brand.Count > 0)
            {
                query = query.Include(a => a.Brand)
                   .Where(e => Advertiser.Contains(e.IdBrand));
            }
            else
            if (Advertiser != null && Advertiser.Count > 0)
            {
                query = query.Include(a => a.Advertiser)
                    .Where(e => Advertiser.Contains(e.IdAdvertiser));
            }
            var res = (from g in query
                       where g.IdLanguageData == idLanguage
                       select g);
            return res.ToList();
        }
    }
}
