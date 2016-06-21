using AutoMapper;
using Facebook.Service.BusinessLogic;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Contract.ContractModels.RightService;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.MauSchema;

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
                e.CreateMap<OrderTemplateMedia, Right>()
                .ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia))
                .ForMember(a => a.IdTypeMedia, d => d.MapFrom(dst => dst.IdTypeMedia));
                e.CreateMap<OrderClientMedia, Right>()
                .ForMember(a => a.Rights, d => d.MapFrom(dst => dst.ListMedia))
                .ForMember(a => a.IdTypeMedia, d => d.MapFrom(dst => dst.IdTypeMedia));
                e.CreateMap<DataFacebook, DataFacebookContract>();
                e.CreateMap<Product, ProductContract>();
            });
        }
    }
}
