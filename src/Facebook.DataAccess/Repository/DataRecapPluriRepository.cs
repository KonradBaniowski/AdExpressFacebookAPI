using ExtensionMethods;
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.BusinessModel;
using Facebook.Service.Core.DomainModels.RecpaSchema;
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
    public class DataRecapPluriRepository : GenericRepository<RecapPluri>, IDataRecapPluriRepository
    {
        public DataRecapPluriRepository(FacebookContext context) : base(context)
        {
        }


        //public RecapPluriExpenditure GetLastLoadedMonth()
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.Append("SELECT ");

        //    for (int i = 2; i >= 0; i--)
        //    {
        //        string prop = "exp_Euro_N";
        //        if (i != 0)
        //        prop += i.ToString();

        //        for (int j = 1; j < 13; j++)
        //        {
        //            query.AppendFormat("SUM({0}) AS {1},", prop + "_" + j.ToString(), prop.Replace("exp", "Expenditure"));
        //        }
        //    }

        //    query.Length--; //Reomve last char (here th ",")
        //    query.Append(" FROM RECAP01.RECAP_PLURI");

        //    var test = context.Database.SqlQuery<RecapPluriExpenditure>(query.ToString());
        //    var res = test.ToList();

        //    return test;

        //}


        public RecapPluriExpenditure GetLastLoadedMonth()
        {
            List<RecapPluriExpenditure> result = null;

            var query = (from p in context.DataRecapPluriSegment
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


        public List<RecapPluriExpenditure> GetDataRecapPluri(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var query = (from d in context.DataRecapPluri select d);
            List<RecapPluriExpenditure> result = null;
            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            Expression<Func<RecapPluri, bool>> predicate = null;

            if (includedProduct != null && includedProduct.Any())
            {
                var include2 = query.PredicateRecap(includedProduct);
                predicate = arg => (include2.Invoke(arg));
            }
            if (excludedProduct != null && excludedProduct.Any())
            {
                var exclude2 = query.PredicateRecap(excludedProduct);
                predicate = arg => !(exclude2.Invoke(arg));
            }

            if (predicate != null)
                query = query.AsExpandable().Where(predicate);
            
            if (Brand != null && Brand.Count > 0)
            {
                query = query.Include(a => a.Brand)
                   .Where(e => Brand.Contains(e.IdBrand));

               
                var res = (from g in query
                           //where g.IdLanguageData == idLanguage
                           select g);

                var brandQuery = (from g in query.GroupBy(p => new { p.IdBrand })
                                  join c in context.Brand on new { id = g.Key.IdBrand } equals new { id = c.Id }
                                  where c.IdLanguage == idLanguage
                                  select new RecapPluriExpenditure
                                  {
                                      Id = g.Key.IdBrand,
                                      Label = c.BrandLabel,
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

                result = brandQuery.ToList();
            }
            else if (Advertiser != null && Advertiser.Count > 0)
            {
                query = query.Include(a => a.Advertiser)
                    .Where(e => Advertiser.Contains(e.IdAdvertiser));

                var res = (from g in query
                           //where g.IdLanguageData == idLanguage
                           select g);

                var advertiserQuery = (from g in query.GroupBy(p => new { p.IdAdvertiser })
                                  join c in context.Advertiser on new { id = g.Key.IdAdvertiser } equals new { id = c.IdAdvertiser }
                                  where c.IdLanguage == idLanguage
                                  select new RecapPluriExpenditure
                                  {
                                      Id = g.Key.IdAdvertiser,
                                      Label = c.AdvertiserLabel,
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

                result = advertiserQuery.ToList();
            }

            //result = result.GroupBy(e => e.Id).Select(group => group.First()).ToList();

            return result;
        }

    }
}
