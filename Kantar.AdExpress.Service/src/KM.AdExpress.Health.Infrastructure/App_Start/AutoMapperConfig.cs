using AutoMapper;
using KM.AdExpress.Health.Core.Model;
using KM.AdExpress.Health.Infrastructure.Contract;

namespace KM.AdExpress.Health.Infrastructure.App_Start
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
                e.CreateMap<DataCost, DataCostContract>()
                .ForMember(a => a.Canal, d => d.MapFrom(dst => dst.Canal.CanalLabel))
                .ForMember(a => a.Category, d => d.MapFrom(dst => dst.Category.CategoryLabel))
                .ForMember(a => a.Date, d => d.MapFrom(dst => dst.Date))
                .ForMember(a => a.Euro, d => d.MapFrom(dst => dst.Euro))
                .ForMember(a => a.Format, d => d.MapFrom(dst => dst.Format.FormatLabel))
                .ForMember(a => a.GrpPharma, d => d.MapFrom(dst => dst.GrpPharma.GrpPharmaLabel))
                .ForMember(a => a.IdCanal, d => d.MapFrom(dst => dst.IdCanal))
                .ForMember(a => a.IdCategory, d => d.MapFrom(dst => dst.IdCategory))
                .ForMember(a => a.IdFormat, d => d.MapFrom(dst => dst.IdFormat))
                .ForMember(a => a.IdGrpPharma, d => d.MapFrom(dst => dst.IdGrpPharma))
                .ForMember(a => a.IdLabratory, d => d.MapFrom(dst => dst.IdLaboratory))
                .ForMember(a => a.IdProduct, d => d.MapFrom(dst => dst.IdProduct))
                .ForMember(a => a.IdSpecialist, d => d.MapFrom(dst => dst.IdSpecialist))
                .ForMember(a => a.Laboratory, d => d.MapFrom(dst => dst.Laboratory.LaboratoryLabel))
                .ForMember(a => a.Product, d => d.MapFrom(dst => dst.Product.ProductLabel))
                .ForMember(a => a.Specialist, d => d.MapFrom(dst => dst.Specialist.SpecialistLabel));

            });
        }
    }

}
