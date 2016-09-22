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

        public RecapPluriExpenditure GetLastLoadedMonth()
        {
            List<RecapPluriExpenditure> result = null;

            var query = (from p in context.DataRecapSearchSegment
                         group p by p.IdLanguageData into g
                         select new RecapPluriExpenditure
                         {
                             Expenditure_Euro_N_1 = g.Sum(a => a.Expenditure_Euro_N_1),
                             Expenditure_Euro_N_2 = g.Sum(a => a.Expenditure_Euro_N_2),
                             Expenditure_Euro_N_3 = g.Sum(a => a.Expenditure_Euro_N_3),
                             Expenditure_Euro_N_4 = g.Sum(a => a.Expenditure_Euro_N_4),
                             Expenditure_Euro_N_5 = g.Sum(a => a.Expenditure_Euro_N_5),
                             Expenditure_Euro_N_6 = g.Sum(a => a.Expenditure_Euro_N_6),
                             Expenditure_Euro_N_7 = g.Sum(a => a.Expenditure_Euro_N_7),
                             Expenditure_Euro_N_8 = g.Sum(a => a.Expenditure_Euro_N_8),
                             Expenditure_Euro_N_9 = g.Sum(a => a.Expenditure_Euro_N_9),
                             Expenditure_Euro_N_10 = g.Sum(a => a.Expenditure_Euro_N_10),
                             Expenditure_Euro_N_11 = g.Sum(a => a.Expenditure_Euro_N_11),
                             Expenditure_Euro_N_12 = g.Sum(a => a.Expenditure_Euro_N_12),

                             Expenditure_Euro_N1_1 = g.Sum(a => a.Expenditure_Euro_N1_1),
                             Expenditure_Euro_N1_2 = g.Sum(a => a.Expenditure_Euro_N1_2),
                             Expenditure_Euro_N1_3 = g.Sum(a => a.Expenditure_Euro_N1_3),
                             Expenditure_Euro_N1_4 = g.Sum(a => a.Expenditure_Euro_N1_4),
                             Expenditure_Euro_N1_5 = g.Sum(a => a.Expenditure_Euro_N1_5),
                             Expenditure_Euro_N1_6 = g.Sum(a => a.Expenditure_Euro_N1_6),
                             Expenditure_Euro_N1_7 = g.Sum(a => a.Expenditure_Euro_N1_7),
                             Expenditure_Euro_N1_8 = g.Sum(a => a.Expenditure_Euro_N1_8),
                             Expenditure_Euro_N1_9 = g.Sum(a => a.Expenditure_Euro_N1_9),
                             Expenditure_Euro_N1_10 = g.Sum(a => a.Expenditure_Euro_N1_10),
                             Expenditure_Euro_N1_11 = g.Sum(a => a.Expenditure_Euro_N1_11),
                             Expenditure_Euro_N1_12 = g.Sum(a => a.Expenditure_Euro_N1_12),

                             Expenditure_Euro_N2_1 = g.Sum(a => a.Expenditure_Euro_N2_1),
                             Expenditure_Euro_N2_2 = g.Sum(a => a.Expenditure_Euro_N2_2),
                             Expenditure_Euro_N2_3 = g.Sum(a => a.Expenditure_Euro_N2_3),
                             Expenditure_Euro_N2_4 = g.Sum(a => a.Expenditure_Euro_N2_4),
                             Expenditure_Euro_N2_5 = g.Sum(a => a.Expenditure_Euro_N2_5),
                             Expenditure_Euro_N2_6 = g.Sum(a => a.Expenditure_Euro_N2_6),
                             Expenditure_Euro_N2_7 = g.Sum(a => a.Expenditure_Euro_N2_7),
                             Expenditure_Euro_N2_8 = g.Sum(a => a.Expenditure_Euro_N2_8),
                             Expenditure_Euro_N2_9 = g.Sum(a => a.Expenditure_Euro_N2_9),
                             Expenditure_Euro_N2_10 = g.Sum(a => a.Expenditure_Euro_N2_10),
                             Expenditure_Euro_N2_11 = g.Sum(a => a.Expenditure_Euro_N2_11),
                             Expenditure_Euro_N2_12 = g.Sum(a => a.Expenditure_Euro_N2_12)
                         });

            result = query.ToList();

            return result.First();
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
                   .Where(e => Brand.Contains(e.IdBrand));
            }
            else
            if (Advertiser != null && Advertiser.Count > 0)
            {
                query = query.Include(a => a.Advertiser)
                    .Where(e => Advertiser.Contains(e.IdAdvertiser));
            }
            var res = (from g in query
                       //where g.IdLanguageData == idLanguage
                       select g);
            return res.ToList();
        }
    }
}
