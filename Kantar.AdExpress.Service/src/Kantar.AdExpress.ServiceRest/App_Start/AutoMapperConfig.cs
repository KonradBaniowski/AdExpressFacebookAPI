using AutoMapper;

namespace Kantar.AdExpress.ServiceRest.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            ConfigureMapping();
        }

        public static void ConfigureMapping()
        {
            //Mapper.Initialize(z =>
            //{
            //    z.AddProfile<AutoMapperConfigBL>();
            //});
        }
    }
}