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
                e.CreateMap<DataFacebook, KPIPageFacebookContract>()
                .ForMember(a => a.Like, d => d.MapFrom(dst => dst.NumberLike))
                .ForMember(a => a.Month, d => d.MapFrom(dst => dst.DateMediaNum))
                .ForMember(a => a.Post, d => d.MapFrom(dst => dst.NumberPost))
                .ForMember(a => a.Share, d => d.MapFrom(dst => dst.NumberShare))
                .ForMember(a => a.Comment, d => d.MapFrom(dst => dst.NumberComment));

                e.CreateMap<DataFacebook, DataFacebookContract>();
                e.CreateMap<DataFacebookKPI, DataFacebookContract>();
                e.CreateMap<CriteriaData, Criteria>().ReverseMap();
                e.CreateMap<LevelItem, LevelItemContract>();
                e.CreateMap<PostFacebook, PostFacebookContract>();
            });
        }
    }
}
