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

        public List<DateFacebookContract> GetDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var query = (from d in context.DataFacebook
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End

                         select d);

            //var includedMedia = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Media));
            //var excludedMedia = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Media));

            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            //var include1 = query.Predicate(includedMedia);
            //var exclude1 = query.Predicate(excludedMedia);
            Expression<Func<DataFacebook, bool>> predicate = null;

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
            //Expression<Func<DataFacebook, bool>> predicate = arg => ((include1.Invoke(arg) || include2.Invoke(arg)) && !(exclude1.Invoke(arg) && exclude2.Invoke(arg)));
            //predicate = arg => (include2.Invoke(arg)) && !(exclude2.Invoke(arg));
            //var exp = Expression.Equal(predicate, Expression.Constant(null, predicate.Type));

            if (predicate != null)
                query = query.AsExpandable().Where(predicate);

            if (Brand != null && Brand.Count > 0)
            {
                query = query.Include(a => a.Brand)
                    .Where(e => Brand.Contains(e.IdBrand));
                var brandQuery = (from g in query.GroupBy(p => new { p.IdBrand, p.IdPageFacebook, p.IdLanguageData })
                                  join c in context.Brand on new { id = g.Key.IdBrand, IdLanguage = g.Key.IdLanguageData } equals new { id = c.Id, IdLanguage = c.IdLanguage }
                                  where c.IdLanguage == idLanguage
                                  select new DateFacebookContract
                                  {
                                      IdAdvertiser = g.Key.IdBrand,
                                      NumberPost = g.Sum(a => a.NumberPost),
                                      NumberLike = g.Sum(a => a.NumberLike),
                                      NumberComment = g.Sum(a => a.NumberComment),
                                      NumberShare = g.Sum(a => a.NumberShare),
                                      Expenditure = g.Sum(a => a.Expenditure),
                                      NumberFan = g.Max(a => a.NumberFan),
                                      PageName = g.FirstOrDefault().PageName,
                                      IdPage = g.FirstOrDefault().IdPage,
                                      IdPageFacebook = g.FirstOrDefault().IdPageFacebook,
                                      Url = g.FirstOrDefault().Url,
                                      Label = c.BrandLabel
                                  }).ToList();
                return brandQuery;
            }
            else if (Advertiser != null && Advertiser.Count > 0)
            {
                query = query.Include(a => a.Advertiser)
                    .Where(e => Advertiser.Contains(e.IdAdvertiser));

                var tata = (from g in query.GroupBy(p => new { p.IdAdvertiser, p.IdPageFacebook, p.IdLanguageData })
                            join c in context.Advertiser on new { g.Key.IdAdvertiser, IdLanguage = g.Key.IdLanguageData } equals new { c.IdAdvertiser, IdLanguage = c.IdLanguage }
                            where c.IdLanguage == idLanguage
                            select new DateFacebookContract
                            {
                                IdAdvertiser = g.Key.IdAdvertiser,
                                NumberPost = g.Sum(a => a.NumberPost),
                                NumberLike = g.Sum(a => a.NumberLike),
                                NumberComment = g.Sum(a => a.NumberComment),
                                NumberShare = g.Sum(a => a.NumberShare),
                                Expenditure = g.Sum(a => a.Expenditure),
                                NumberFan = g.Max(a => a.NumberFan),
                                PageName = g.FirstOrDefault().PageName,
                                IdPage = g.FirstOrDefault().IdPage,
                                IdPageFacebook = g.FirstOrDefault().IdPageFacebook,
                                Url = g.FirstOrDefault().Url,
                                Label = c.AdvertiserLabel

                            });
                return tata.ToList();
            }
            else
                return new List<DateFacebookContract>();
        }

        public List<DataFacebook> GetKPIDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var query = (from d in context.DataFacebook
                         where d.DateMediaNum >= Begin && d.DateMediaNum <= End
                         select d);

            var includedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Include) && e.TypeNomenclature == (TypeNomenclature.Product));
            var excludedProduct = Criteria.Where(e => e.TypeCriteria == (TypeCriteria.Exclude) && e.TypeNomenclature == (TypeNomenclature.Product));

            Expression<Func<DataFacebook, bool>> predicate = null;

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
