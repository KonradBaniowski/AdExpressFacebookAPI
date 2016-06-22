using Facebook.DataAccess.Mapping;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class OrderTemplateMediaRepository : GenericRepository<OrderTemplateMedia>, IOrderTemplateMediaRepository
    {
        public OrderTemplateMediaRepository(FacebookContext context) : base(context)
        {

        }
        
        public List<OrderTemplateMedia> GetTemplateMediaRight(int idLogin)
        {
            string query = string.Format("select id_order_template_media Id, mau01.listnum_to_char(list_media) ListMedia, EXCEPTION, tm.id_type_media IdTypeMedia from mau01.order_template_media otm, mau01.type_media tm, mau01.template t, mau01.template_assignment ta where otm.id_type_media = tm.id_type_media and otm.id_template = t.id_template and t.id_template = ta.id_template and id_login = {0} and ta.id_project = 1 and otm.activation < 50 and tm.activation < 50 and t.activation < 50 and ta.activation < 50", idLogin);
            var test = context.Database.SqlQuery<OrderTemplateMedia>(query);
            var res = test.ToList();
            return res;
        }
    }
}
