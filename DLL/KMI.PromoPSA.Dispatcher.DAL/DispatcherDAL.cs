using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.Dispatcher.DAL.DbType;

namespace KMI.PromoPSA.Dispatcher.DAL
{
    public class DispatcherDAL
    {
        public List<AdvertStatus> GetAdverts(DbManager db)
        {
            var query = from p in db.GetTable<DataPromotion>() 
                        where  p.IdForm>0 
                          && p.Activation != Constantes.Constantes.ACTIVATION_CODE_INACTIVE
                        select new AdvertStatus
                            {
                                IdForm = p.IdForm,
                                Activation = p.Activation,
                                LoadDate = p.LoadDate,
                                IdDataPromotion = p.IdDataPromotion
                            };

            return query.ToList();
        }

        public List<AdvertStatus> GetAdvertsByFormId(DbManager db, long formId) {
            var query = from p in db.GetTable<DataPromotion>()
                        where  p.IdForm == formId
                         && p.Activation != Constantes.Constantes.ACTIVATION_CODE_INACTIVE
                        orderby p.IdDataPromotion
                        select new AdvertStatus {
                            IdForm = p.IdForm,
                            Activation = p.Activation,
                            LoadDate = p.LoadDate,
                            IdDataPromotion = p.IdDataPromotion
                        };

            return query.ToList();
        }

        public void UpdateMonth(DbManager db, long loadDate,long activationCode,long targettedCode)
        {
           
                var query = new StringBuilder();
                var cultureInfo = new CultureInfo("fr-FR");

                query.Append("BEGIN ");
                query.AppendFormat(" UPDATE  {0}.DATA_PROMOTION SET ", Constantes.Db.PROMO_SCHEMA);            
                query.AppendFormat(" ACTIVATION = to_number('{0}') ", activationCode.ToString(cultureInfo));
                query.AppendFormat(" WHERE LOAD_DATE = to_number('{0}') ", loadDate.ToString(cultureInfo));
                query.AppendFormat(" AND ACTIVATION = to_number('{0}') AND ID_FORM > 0; "
                    , targettedCode);
                query.Append(" END; ");
                var dbCmd = db.SetCommand(query.ToString());
                dbCmd.ExecuteNonQuery();
           
        }
    }
}
