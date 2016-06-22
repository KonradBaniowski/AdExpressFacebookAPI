using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class OrderTemplateProductRepository: GenericRepository<OrderTemplateProduct>, IOrderTemplateProductRepository
    {
        public OrderTemplateProductRepository(FacebookContext context) : base(context)
        {

        }

        public List<OrderTemplateProduct> GetTemplateProductRight(int idLogin)
        {
            string query = string.Format("select mau01.listnum_to_char(list_product) ListMedia, exception,tp.id_type_product IdTypeProduct from  mau01.order_template_product otp, mau01.type_product tp, mau01.template t,  mau01.template_assignment ta  where otp.id_type_product=tp.id_type_product and otp.id_template=t.id_template and t.id_template=ta.id_template  and id_login = {0} and ta.id_project=1  and otp.activation<50  and tp.activation<50  and t.activation<50  and ta.activation<50", idLogin);
            var test = context.Database.SqlQuery<OrderTemplateProduct>(query);
            var res = test.ToList();
            return res;
        }
    }
}
