using Facebook.DataAccess.Mapping;
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facebook.DataAccess.Repository
{
    public class OrderClientMediaRepository : GenericRepository<OrderClientMedia>, IOrderClientMediaRepository
    {
        public OrderClientMediaRepository(FacebookContext context) : base(context)
        {
            
        }

        public List<OrderClientMedia> GetMediaRights(int idLogin)
        {
            string query = string.Format("select OCM.ID_ORDER_CLIENT_MEDIA Id, mau01.listnum_to_char(list_media) ListMedia,exception,tm.id_type_media IdTypeMedia from mau01.order_client_media ocm, mau01.type_media tm where ocm.id_type_media = tm.id_type_media and id_login = {0} and id_project = 1 and ocm.activation < 50 and tm.activation < 50", idLogin);
            var test = context.Database.SqlQuery<OrderClientMedia>(query);
            var res = test.ToList();
            return res;
        }
    }
}