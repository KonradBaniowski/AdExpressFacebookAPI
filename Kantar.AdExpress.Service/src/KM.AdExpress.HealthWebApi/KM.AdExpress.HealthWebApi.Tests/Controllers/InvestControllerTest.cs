using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using KM.AdExpress.HealthWebApi;
using KM.AdExpress.HealthWebApi.Controllers;
using KM.AdExpress.Health.Core.Interfaces.Services;
using KM.AdExpress.Health.Infrastructure.Services;
using KM.AdExpress.Health.Infrastructure.App_Start;
using KM.AdExpress.Health.Core.Interfaces;
using KM.AdExpress.Health.Infrastructure;
using KM.AdExpress.Health.Infrastructure.Contract;
using System.Collections.Generic;

namespace KM.AdExpress.HealthWebApi.Tests.Controllers
{
    [TestClass]
    public class InvestControllerTest
    {
        [TestMethod]
        public void GetInvest()
        {

            
            IMapper mapper = new AutoMapperConfig().mapper;
            IHealthUow uow = new HealthUow(new HealthContext());


            ICostService _costsvc = new CostService(uow, mapper);


            UserCriteriaContract userCriter = new UserCriteriaContract
            {
                CanalIds = new List<double> { 1, 2, 3, 4, 5 },
                CategoryExcludeIds = null,
                CategoryIncludeIds = new List<double> {  },
                GrpPharmaExcludeIds = null,
                GrpPharmaIncludeIds= new List<double> {  },
                ProductExcludeIds = null,
                ProductIncludeIds = new List<double> {  },
                EndDate = new System.DateTime(2016, 12, 01),
                StartDate = new System.DateTime(2016, 01, 01)

            };

            var model = _costsvc.GetDataCost(userCriter);

        }
    }
}
