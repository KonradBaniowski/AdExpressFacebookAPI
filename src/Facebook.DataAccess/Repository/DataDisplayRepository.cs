using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System.Linq.Expressions;
using ExtensionMethods;
using LinqKit;
using System.Data.Entity;

namespace Facebook.DataAccess.Repository
{
    public class DataDisplayRepository : GenericRepository<DataDisplay>, IDataDisplayRepository
    {
        public DataDisplayRepository(FacebookContext context) : base(context)
        {
        }

        public RecapPluriExpenditure GetLastLoadedMonth()
        {
            List<RecapPluriExpenditure> result = null;

            var query = (from p in context.DataRecapDisplaySegment
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
                             Expenditure_Euro_N_12 = g.Sum(a => a.Expenditure_Euro_N_12)
                         });

            result = query.ToList();

            return result.First();
        }


        public List<DataDisplay> GetDataDisplayWithCriteria(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var query = (from d in context.DataDisplay
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End
                         select d);

            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            Expression<Func<DataDisplay, bool>> predicate = null;

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

