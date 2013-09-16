using System.Collections.Generic;
using System.Linq;
using BLToolkit.Data;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.DAL.DbType;

namespace KMI.PromoPSA.DAL
{
    public class PromoPsaDAL
    {

        public List<Advert> GetAdverts(DbManager db,long loadDate)
        {
            var query = from p in db.GetTable<DataPromotion>()
                        where p.LoadDate == loadDate
                        select new Advert
                            {
                                IdForm = p.IdForm,
                                Activation = p.Activation,
                                LoadDate = p.LoadDate,
                                IdDataPromotion = p.IdDataPromotion,
                                IdVehicle = p.IdVehicle,
                                DateMediaNum = p.DateMediaNum,
                                PromotionVisual = p.PromotionVisual
                            };

            return query.ToList();
        }

        public int GetNbAdverts(DbManager db, long loadDate,long activationCode)
        {
            var query = from p in db.GetTable<DataPromotion>()
                        where p.LoadDate == loadDate && p.Activation==activationCode
                        select p;

            return query.Count();

           
        }

        public List<Advert> GetOneAdvert(DbManager db, long idForm)
        {
            var query = from p in db.GetTable<DataPromotion>()
                        where p.IdForm == idForm
                        select new Advert
                        {
                            IdForm = p.IdForm,
                            IdCategory = p.IdCategory,
                            IdCircuit = p.IdCircuit,
                            IdProduct = p.IdProduct,
                            IdBrand = p.IdBrand,
                            IdSegment =  p.IdSegment,
                            Activation = p.Activation,
                            LoadDate = p.LoadDate,
                            IdDataPromotion = p.IdDataPromotion,
                            IdVehicle = p.IdVehicle,
                            DateBeginNum =  p.DateBeginNum,
                            DateEndNum = p.DateEndNum,
                            DateMediaNum = p.DateMediaNum,
                            PromotionVisual = p.PromotionVisual,
                            PromotionContent = p.PromotionContent,
                            ConditionText = p.ConditionText,
                            Script = p.Script,
                            ExcluWeb = p.ExcluWeb
                        };

            return query.ToList();
        }

        public List<LoadDateBE> GetLoadDates(DbManager db) {

            var query = from p in db.GetTable<DataPromotion>()
                        orderby p.LoadDate
                        group new { p.LoadDate } by new { p.LoadDate } into g
                        select new LoadDateBE {
                            LoadDate = g.Key.LoadDate
                        };

            return query.ToList();
        
        }
      
    }
}
