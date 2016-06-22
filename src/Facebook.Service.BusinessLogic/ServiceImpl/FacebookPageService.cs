﻿using AutoMapper;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class FacebookPageService : IFacebookPageService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        public FacebookPageService(IFacebookUow uow, IMapper mapper)
        {
            _mapper = mapper;
            _uow = uow;
        }

        public List<DataFacebookContract> GetDataFacebook()
        {
            var query = _uow.DataFacebookRepository.Find(e => e.IdMedia == 24862).ToList();
            var result = _mapper.Map<List<DataFacebookContract>>(query);
            return result;
        }

        public List<DataFacebookContract> GetDataFacebook(DateTime Begin, DateTime End, List<int> Advertiser, List<int> Brand)
        {
            return new List<DataFacebookContract>();
        }
    }
}
