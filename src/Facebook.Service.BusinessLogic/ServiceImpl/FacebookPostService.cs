using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.DataAccess;
using AutoMapper;
using Facebook.Service.Core.DomainModels.BusinessModel;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class FacebookPostService : IFacebookPostService
    {

        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        private readonly IRightService _rightsvc;

        public FacebookPostService(IFacebookUow uow, IMapper mapper, IRightService rightsvc)
        {
            _mapper = mapper;
            _uow = uow;
            _rightsvc = rightsvc;
        }

     
        public List<DataPostFacebookContract> GetDataPostFacebook(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, List<long> posts)
        {
            var criteria = _rightsvc.GetCriteria(idLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var query = _uow.DataPostFacebookRepository.GetDataPostFacebook(criteriaData, begin, end, advertisers, brands, posts);
            return new List<DataPostFacebookContract>();
        }
    }
}
