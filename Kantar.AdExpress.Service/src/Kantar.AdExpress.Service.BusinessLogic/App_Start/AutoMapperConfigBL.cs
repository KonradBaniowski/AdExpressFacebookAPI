using AutoMapper;
using Kantar.AdExpress.Service.Contracts;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.BusinessLogic.App_Start
{
    public class AutoMapperConfigBL : Profile
    {
        public AutoMapperConfigBL()
        {
            var res = new Profile("ServiceImplMapping");
        }

        protected override void Configure()
        {
            Mapper.CreateMap<SignInStatus, SignInStatusModel>();
        }
    }
}
