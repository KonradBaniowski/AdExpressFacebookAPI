using AutoMapper;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VM = Km.AdExpressClientWeb.Models.Shared;
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
            Mapper.CreateMap<TNS.Alert.Domain.Alert, Domain.Alert>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlertId));
            Mapper.CreateMap<TNS.Alert.Domain.AlertOccurence, Domain.Occurence>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlertOccurrenceId))
                .ForMember(dest => dest.AlertId, opt => opt.MapFrom(src => src.AlertId))
                .ForMember(dest => dest.SendDate, opt => opt.MapFrom(src => src.DateSend))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.DateBeginStudy))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.DateEndStudy));
        }
    }
}