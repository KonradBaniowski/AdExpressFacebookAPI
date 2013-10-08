﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                        && p.IdForm>0
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
                         && p.IdForm > 0
                        select p;

            return query.Count();

           
        }

        public List<Advert> GetOneAdvert(DbManager db, long promotionId)
        {
            var query = from p in db.GetTable<DataPromotion>()
                        where p.IdDataPromotion == promotionId
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
                            PromotionBrand = p.PromotionBrand,
                            PromotionVisual = p.PromotionVisual,
                            PromotionContent = p.PromotionContent,
                            ConditionText = p.ConditionText,
                            Script = p.Script,
                            ExcluWeb = p.ExcluWeb,
                            National = p.National
                        };

            return query.ToList();
        }

        public List<LoadDateBE> GetLoadDates(DbManager db) {

            var query = from p in db.GetTable<DataPromotion>()
                        where  p.IdForm >0
                        orderby p.LoadDate
                        group new { p.LoadDate } by new { p.LoadDate } into g
                        select new LoadDateBE {
                            LoadDate = g.Key.LoadDate
                        };

            return query.ToList();
        
        }

        public void UpdateCodification(DbManager db, Advert advert)
        {
            if (advert != null)
            {
                var query = new StringBuilder();
                var cultureInfo = new CultureInfo("fr-FR");

                query.Append("BEGIN ");
                query.AppendFormat(" UPDATE  {0}.DATA_PROMOTION SET ", Constantes.Db.PROMO_SCHEMA);

                query.AppendFormat(" ID_SEGMENT = to_number('{0}') "
                     , advert.IdSegment.ToString(cultureInfo));

                query.AppendFormat(" ,ID_CATEGORY = to_number('{0}') "
                   , advert.IdCategory.ToString(cultureInfo));

                query.AppendFormat(" ,ID_CIRCUIT = to_number('{0}') "
                  , advert.IdCircuit.ToString(cultureInfo));

                query.AppendFormat(" ,ID_PRODUCT = to_number('{0}') "
                    , advert.IdProduct.ToString(cultureInfo));

                query.AppendFormat(" ,ID_BRAND = to_number('{0}') "
                   , advert.IdBrand.ToString(cultureInfo));

                query.AppendFormat(" ,DATE_BEGIN_NUM = to_number('{0}') "
                  , advert.DateBeginNum.ToString(cultureInfo));

                query.AppendFormat(" ,DATE_END_NUM = to_number('{0}') "
                , advert.DateEndNum.ToString(cultureInfo));

                query.AppendFormat(" ,EXCLU_WEB = to_number('{0}') "
               , advert.ExcluWeb.ToString(cultureInfo));

                query.AppendFormat(" ,ACTIVATION = to_number('{0}') "
             , advert.Activation.ToString(cultureInfo));

                query.AppendFormat(" ,NATIONAL = to_number('{0}') "
               , advert.National.ToString(cultureInfo));

                if (!string.IsNullOrEmpty(advert.PromotionBrand))
                {
                    var promotionBrand = Regex.Replace(advert.PromotionBrand, "[']", "''");
                    query.AppendFormat(" ,PROMOTION_BRAND='{0}'", promotionBrand);
                }
                if (!string.IsNullOrEmpty(advert.PromotionContent))
                {
                    var promotionContent =  Regex.Replace(advert.PromotionContent, "[']", "''");
                    query.AppendFormat(" ,PROMOTION_CONTENT='{0}'", promotionContent);
                }
                if (!string.IsNullOrEmpty(advert.ConditionText))
                {
                    var conditionText = Regex.Replace(advert.ConditionText, "[']", "''");
                    query.AppendFormat(" ,CONDITION_TEXT='{0}'", conditionText);
                }
                if (!string.IsNullOrEmpty(advert.Script))
                {
                    var script = Regex.Replace(advert.Script, "[']", "''");
                    query.AppendFormat(" ,SCRIPT='{0}'", script);
                }
                query.AppendFormat(" WHERE ID_DATA_PROMOTION = to_number('{0}'); ", advert.IdDataPromotion);
                query.Append(" END; ");
                var dbCmd = db.SetCommand(query.ToString());
                dbCmd.ExecuteNonQuery();
              
            }
        }

        public void UpdateCodification(DbManager db, long promotionId, long activationCode)
        {
            var query = new StringBuilder();
            var cultureInfo = new CultureInfo("fr-FR");

            query.Append("BEGIN ");
            query.AppendFormat(" UPDATE  {0}.DATA_PROMOTION SET ", Constantes.Db.PROMO_SCHEMA);
            query.AppendFormat(" ACTIVATION = to_number('{0}') "
         , activationCode.ToString(cultureInfo));
            query.AppendFormat(" WHERE ID_DATA_PROMOTION = to_number('{0}'); ", promotionId);
            query.Append(" END; ");
            var dbCmd = db.SetCommand(query.ToString());
            dbCmd.ExecuteNonQuery();
        }
    }
}
