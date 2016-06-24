using AutoMapper;
using Facebook.Service.Contract.BusinessModels;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;

namespace Facebook.Service.BusinessLogic
{
    public class AutoMapperConfig
    {
        private MapperConfiguration _config;
        public IMapper mapper
        {
            get
            {
                return _config.CreateMapper();
            }
        }
        public AutoMapperConfig()
        {
            _config = new MapperConfiguration(e =>
            {
                //e.CreateMap<OrderTemplateMedia, Right>()
                //.ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia.Split(',').Select(int.Parse).ToList()))
                //.ForMember(a => a.IdType, d => d.MapFrom(dst => dst.IdTypeMedia));
                //e.CreateMap<OrderClientMedia, Right>()
                //.ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia.Split(',').Select(int.Parse).ToList()))
                //.ForMember(a => a.IdType, d => d.MapFrom(dst => dst.IdTypeMedia));
                //e.CreateMap<OrderClientProduct, Right>()
                //.ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia.Split(',').Select(int.Parse).ToList()))
                //.ForMember(a => a.IdType, d => d.MapFrom(dst => dst.IdTypeProduct));
                //e.CreateMap<OrderTemplateProduct, Right>()
                //.ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia.Split(',').Select(int.Parse).ToList()))
                //.ForMember(a => a.IdType, d => d.MapFrom(dst => dst.IdTypeProduct));
                //e.CreateMap<RightDomain, Right>().ReverseMap();
                e.CreateMap<DataFacebook, DataFacebookContract>();
                e.CreateMap<CriteriaData, Criteria>().ReverseMap();
                e.CreateMap<LevelItem, LevelItemContract>();
            });
        }
    }
}
