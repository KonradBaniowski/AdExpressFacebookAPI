using AutoMapper;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VM = Km.AdExpressClientWeb.Models.MediaSchedule;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            //var config = new MapperConfiguration(d =>
            //    {
            //        d.CreateMap<Domain.UniversBranch, VM.UniversBranch>();
            //    });            
            Mapper.CreateMap<Domain.UniversLevel, VM.UniversLevel>();
            Mapper.CreateMap<Domain.UniversBranch, VM.UniversBranch>();
            Mapper.CreateMap<Domain.UserUnivers, VM.UserUnivers>();
            Mapper.CreateMap<Domain.UserUniversGroup, VM.UserUniversGroup>();
            Mapper.CreateMap<AccessType, AccessType>();
            Mapper.CreateMap<Domain.Tree, VM.Tree>();  //Error while mapping enum      
            Mapper.CreateMap<VM.Tree,Domain.Tree>();
            Mapper.CreateMap<VM.UniversLevel, Domain.UniversLevel>();
            Mapper.CreateMap<VM.UniversItem, Domain.UniversItem>();
            Mapper.CreateMap<Domain.UniversItem, VM.UniversItem>();
        }
    }
}