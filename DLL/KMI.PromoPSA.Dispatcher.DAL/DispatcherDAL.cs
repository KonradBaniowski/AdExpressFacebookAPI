using System;
using System.Collections.Generic;
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
                        where p.IdForm != null && p.Activation > 0
                        select new AdvertStatus
                            {
                                IdForm = p.IdForm,
                                Activation = p.Activation,
                                LoadDate = p.LoadDate,
                                IdDataPromotion = p.IdDataPromotion
                            };

            return query.ToList();
        }
    }
}
