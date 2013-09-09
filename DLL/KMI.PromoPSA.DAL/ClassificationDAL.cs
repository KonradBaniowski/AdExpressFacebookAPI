using System.Collections.Generic;
using System.Linq;
using BLToolkit.Data;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.DAL.DbType;

namespace KMI.PromoPSA.DAL
{
    public class ClassificationDAL
    {


        public List<Segment> GetSegments(DbManager db, long idLanguage)
        {
            var query = from p in db.GetTable<DataSegment>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        select new Segment
                            {
                                Id = p.IdSegment,
                                Label = p.Segment,
                                IdLanguage = p.IdLanguage
                            };
            return query.ToList();
        }

        public Segment GetSegment(DbManager db, long idLanguage, long idSegment)
        {
            var query = from p in db.GetTable<DataSegment>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        && p.IdSegment == idSegment
                        select new Segment
                        {
                            Id = p.IdSegment,
                            Label = p.Segment,
                            IdLanguage = p.IdLanguage
                        };
            return query.FirstOrDefault();
        }

        public List<Product> GetProducts(DbManager db, long idLanguage)
        {
            var query = from p in db.GetTable<DataProduct>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        select new Product
                        {
                            Id = p.IdProduct,
                            Label = p.Product,
                            IdLanguage = p.IdLanguage,
                            IdCategory = p.IdCategory
                        };
            return query.ToList();
        }

        public Product GetProduct(DbManager db, long idLanguage, long idProduct)
        {
            var query = from p in db.GetTable<DataProduct>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        && p.IdProduct == idProduct
                        select new Product
                        {
                            Id = p.IdProduct,
                            Label = p.Product,
                            IdLanguage = p.IdLanguage,
                            IdCategory = p.IdCategory
                        };
            return query.FirstOrDefault();
        }

        public List<Brand> GetBrands(DbManager db, long idLanguage)
        {
            var query = from p in db.GetTable<DataBrand>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        select new Brand
                        {
                            Id = p.IdBrand,
                            Label = p.Brand,
                            IdLanguage = p.IdLanguage,
                            IdCircuit = p.IdCircuit
                        };
            return query.ToList();
        }

        public Brand GetBrand(DbManager db, long idLanguage, long idBrand)
        {
            var query = from p in db.GetTable<DataBrand>()
                        where p.Activation < 50
                        && p.IdLanguage == idLanguage
                        && p.IdBrand == idBrand
                        select new Brand
                        {
                            Id = p.IdBrand,
                            Label = p.Brand,
                            IdLanguage = p.IdLanguage,
                            IdCircuit = p.IdCircuit
                        };
            return query.FirstOrDefault();
        }


    }
}
