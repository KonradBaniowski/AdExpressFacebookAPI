using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.BusinessLogic.ServiceImpl;
using System.Collections.Generic;
using AutoMapper;
using Facebook.Service.BusinessLogic;
using Facebook.Service.Core.DataAccess;
using Facebook.DataAccess;

namespace Facebook.Test
{
    [TestClass]
    public class DataFacebookTableau1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IMapper mapper = new AutoMapperConfig().mapper;
            IFacebookUow uow = new FacebookUow(new FacebookContext());
            IRightService rsvc = new RightService(uow, mapper);
            IFacebookPageService _fbsvc = new FacebookPageService(uow, mapper,rsvc);
            long begin = 20150101;
            long end = 20160301;

            var model = _fbsvc.GetDataFacebook(1155, begin, end, new List<long> { 1060, 332860, 48750 }, null,33); 
        }
    }
}
