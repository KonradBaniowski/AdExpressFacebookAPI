using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class OrderClientProductRepository : GenericRepository<OrderClientProduct>, IOrderClientProductRepository
    {
        public OrderClientProductRepository(FacebookContext context) : base(context)
        {

        }

        public List<OrderClientProduct> GetProductRights(int idLogin)
        {
            string query = string.Format("select OCP.ID_ORDER_CLIENT_PRODUCT Id, mau01.listnum_to_char(list_product) ListMedia,exception Exception,tp.id_type_product IdTypeProduct from  mau01.order_client_product ocp, mau01.type_product tp  where ocp.id_type_product=tp.id_type_product and id_login= {0} and id_project = 1 and ocp.activation<50 and tp.activation<50", idLogin);
            var test = context.Database.SqlQuery<OrderClientProduct>(query);
            var res = test.ToList();
            return res;
        }
    }
}
