using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using AutoMapper;
using Facebook.Service.Core.DataAccess;
using Facebook.Service.Core.DomainModels.BusinessModel;
using Facebook.Service.Core.DomainModels.AdExprSchema;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    
    public class KPIFacebookPageService : IKPIFacebookPageService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        private readonly IRightService _rightsvc;

        public KPIFacebookPageService(IFacebookUow uow, IMapper mapper, IRightService rightsvc)
        {
            _mapper = mapper;
            _uow = uow;
            _rightsvc = rightsvc;
        }
        public List<KPIPageFacebookContract> GetKPIPages(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage)
        {
            var criteria = _rightsvc.GetCriteria(IdLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var res = _uow.DataFacebookRepository.GetKPIDataFacebook(criteriaData, Begin, End, Advertiser, Brand, idLanguage);
            var query = res.GroupBy(e => e.DateMediaNum).Select(e => new DataFacebook
            {
                DateMediaNum = e.FirstOrDefault().DateMediaNum,
                NumberLike = e.Sum(a => a.NumberLike),
                NumberShare = e.Sum(a => a.NumberShare),
                NumberComment = e.Sum(a => a.NumberComment),
                NumberPost = e.Sum(a => a.NumberPost),
            }).ToList();
            var kpiResult = _mapper.Map<List<KPIPageFacebookContract>>(query);
            return kpiResult;
        }
    }
}
