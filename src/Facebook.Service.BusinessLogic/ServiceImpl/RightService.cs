using AutoMapper;
using Facebook.Service.Contract.BusinessModels;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class RightService : IRightService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        public RightService(IFacebookUow uow, IMapper mapper)
        {
            _mapper = mapper;
            _uow = uow;
        }

        private List<Criteria> GetMediaRight(int idLogin)
        {
            var svc = _uow.OrderClientMediaRepository.GetMediaRights(idLogin);
            List<Criteria> result = new List<Criteria>();
            foreach (var i in svc)
            {
                Criteria e = new Criteria();
                e.TypeCriteria = i.Exception == 0 ? TypeCriteria.Include : TypeCriteria.Exclude;
                e.LevelType = (LevelType)i.IdTypeMedia;
                e.TypeNomenclature = TypeNomenclature.Media;
                e.Filter = i.ListMedia.Split(',').Select(int.Parse).ToList();
                result.Add(e);
            }
            return result;
        }

        private List<Criteria> GetTemplateMediaRight(int idLogin)
        {
            var svc = _uow.OrderTemplateMediaRepository.GetTemplateMediaRight(idLogin);
            List<Criteria> result = new List<Criteria>();
            foreach (var i in svc)
            {
                Criteria e = new Criteria();
                e.TypeCriteria = i.Exception == 0 ? TypeCriteria.Include : TypeCriteria.Exclude;
                e.LevelType = (LevelType)i.IdTypeMedia;
                e.TypeNomenclature = TypeNomenclature.Media;
                e.Filter = i.ListMedia.Split(',').Select(int.Parse).ToList();
                result.Add(e);
            }
            return result;
        }

        private List<Criteria> GetProductRight(int idLogin)
        {
            var svc = _uow.OrderClientProductRepository.GetProductRights(idLogin);
            List<Criteria> result = new List<Criteria>();
            foreach (var i in svc)
            {
                Criteria e = new Criteria();
                e.TypeCriteria = i.Exception == 0 ? TypeCriteria.Include : TypeCriteria.Exclude;
                e.LevelType = (LevelType)i.IdTypeProduct;
                e.TypeNomenclature = TypeNomenclature.Product;
                e.Filter = i.ListMedia.Split(',').Select(int.Parse).ToList();
                result.Add(e);
            }
            return result;
        }

        private List<Criteria> GetTemplateProductRight(int idLogin)
        {
            var svc = _uow.OrderTemplateProductRepository.GetTemplateProductRight(idLogin);
            List<Criteria> result = new List<Criteria>();
            foreach (var i in svc)
            {
                Criteria e = new Criteria();
                e.TypeCriteria = i.Exception == 0 ? TypeCriteria.Include : TypeCriteria.Exclude;
                e.LevelType = (LevelType)i.IdTypeProduct;
                e.TypeNomenclature = TypeNomenclature.Product;
                e.Filter = i.ListMedia.Split(',').Select(int.Parse).ToList();
                result.Add(e);
            }
            return result;
        }

        public List<Criteria> GetCriteria(int idLogin)
        {
            var OTM = GetTemplateMediaRight(idLogin);
            var OCM = GetMediaRight(idLogin);
            var OTP = GetTemplateProductRight(idLogin);
            var OCP = GetProductRight(idLogin);
            OTM.AddRange(OCM);
            OTM.AddRange(OTP);
            OTM.AddRange(OCP);

            return OTM;
        }
    }
}
