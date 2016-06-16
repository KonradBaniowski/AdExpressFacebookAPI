using AutoMapper;
using Facebook.Service.Contract.ContractModels.RightService;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using System.Collections.Generic;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class RightService: IRightService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        public RightService(IFacebookUow uow, IMapper mapper)
        {
            _mapper = mapper;
            _uow = uow;
        }

        public List<Right> GetMediaRight(int idLogin)
        {
            var svc = _uow.OrderClientMediaRepository.GetMediaRights(idLogin);
            var res = _mapper.Map<List<Right>>(svc);
            return res;
        }

        public List<Right> GetProductRight(int idLogin)
        {
            var model = _uow.OrderTemplateMediaRepository.Get(idLogin);
            var res = _mapper.Map<List<Right>>(model);
            return res;
        }
    }
}
