using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        //public static Expression<Func<T, bool>> Contains<T>(this IQueryable<T> data, IEnumerable<long> filter) where T : PostFacebook
        //{
        //    if (!filter.Any())
        //        return x => true;
        //    else
        //    {
        //        var predicateBuilder = PredicateBuilder.False<T>();

        //        predicateBuilder = predicateBuilder.And(p => filter.Contains(p.IdPageFacebook));
        //        return predicateBuilder;
        //    }
            
        //}
        public static Expression<Func<T, bool>> Predicate<T>(this IQueryable<T> data, IEnumerable<CriteriaData> filter) where T : Data
        {
            if (!filter.Any())
                return x => true;
            else
            {
                var predicateBuilder = PredicateBuilder.False<T>();

                var typeCriteria = filter.First().TypeCriteria == (TypeCriteria.Include) ? TypeCriteria.Include : TypeCriteria.Exclude;
                var lvlType = filter.First().TypeNomenclature == (TypeNomenclature.Media) ? TypeNomenclature.Media : TypeNomenclature.Product;
                if (typeCriteria == (TypeCriteria.Include) && lvlType == (TypeNomenclature.Media))
                {
                    foreach (var i in filter)
                    {
                        if (i.LevelType == (LevelType.Media))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdVehicle));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdVehicle));
                        }
                        else if (i.LevelType == (LevelType.Categorie))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdCategory));
                        }
                        else if (i.LevelType == (LevelType.Support))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdMedia));
                            // predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdMedia));
                        }
                    }
                }
                else if (typeCriteria == (TypeCriteria.Include) && lvlType == (TypeNomenclature.Product))
                {
                    foreach (var i in filter)
                    {
                        if (i.LevelType == (LevelType.Famille))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdSector));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdSector));
                        }
                        else if (i.LevelType == (LevelType.Classe))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdSubSector));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdSubSector));
                        }
                        else if (i.LevelType == (LevelType.Groupe))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdGroup));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdGroup));
                        }
                        else if (i.LevelType == (LevelType.Variete))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdSegment));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdSegment));
                        }
                        else if (i.LevelType == (LevelType.Annonceur))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdAdvertiser));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdAdvertiser));
                        }
                        else if (i.LevelType == (LevelType.Marque))
                        {
                            predicateBuilder = predicateBuilder.Or(t => i.Filter.Contains(t.IdBrand));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdBrand));
                        }
                    }
                }
                else if (typeCriteria == (TypeCriteria.Exclude) && lvlType == (TypeNomenclature.Media))
                {
                    foreach (var i in filter)
                    {
                        if (i.LevelType == (LevelType.Media))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdVehicle));
                            //predicate = predicate.AndAlso(t => !i.Filter.Cast<long>().Contains(t.IdVehicle));
                        }
                        else if (i.LevelType == (LevelType.Categorie))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdCategory));
                            //predicate = predicate.AndAlso(t => !i.Filter.Cast<long>().Contains(t.IdCategory));
                        }
                        else if (i.LevelType == (LevelType.Support))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdMedia));
                            //predicate = predicate.AndAlso(t => !i.Filter.Cast<long>().Contains(t.IdMedia));
                        }
                    }
                }
                else if (typeCriteria == (TypeCriteria.Exclude) && lvlType==(TypeNomenclature.Product))
                {
                    foreach (var i in filter)
                    {
                        if (i.LevelType == (LevelType.Famille))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdSector));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdSector));
                        }
                        else if (i.LevelType == (LevelType.Classe))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdSubSector));
                            //predicate = predicate.OrElse(t => i.Filter.Cast<long>().Contains(t.IdSubSector));
                        }
                        else if (i.LevelType == (LevelType.Groupe))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdGroup));
                        }
                        else if (i.LevelType == (LevelType.Variete))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdSegment));
                        }
                        else if (i.LevelType == (LevelType.Annonceur))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdAdvertiser));
                        }
                        else if (i.LevelType == (LevelType.Marque))
                        {
                            predicateBuilder = predicateBuilder.And(t => i.Filter.Contains(t.IdBrand));
                        }
                    }
                }
                return predicateBuilder;
            }
            
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> predicate1, Expression<Func<T, bool>> predicate2)
        {
            return arg => predicate1.Invoke(arg) && predicate2.Invoke(arg);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> predicate1, Expression<Func<T, bool>> predicate2)
        {
            return arg => predicate1.Invoke(arg) || predicate2.Invoke(arg);
        }
    }
}
