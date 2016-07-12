using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class WebPlanMediaMonthRepository: GenericRepository<WebPlanMediaMonth>, IWebPlanMediaMonthRepository
    {
        public WebPlanMediaMonthRepository(FacebookContext context) : base(context)
        {
        }
    }
}
