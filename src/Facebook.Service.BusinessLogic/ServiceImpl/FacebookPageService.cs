using AutoMapper;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class FacebookPageService : IFacebookPageService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        private readonly IRightService _rightsvc;
        public FacebookPageService(IFacebookUow uow, IMapper mapper, IRightService rightsvc)
        {
            _mapper = mapper;
            _uow = uow;
            _rightsvc = rightsvc;
        }

        public List<DataFacebookContract> GetDataFacebook()
        {
            var query = _uow.DataFacebookRepository.Find(e => e.IdMedia == 24862).ToList();
            var result = _mapper.Map<List<DataFacebookContract>>(query);
            return result;
        }

        public List<DataFacebookContract> GetDataFacebook(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> Brand)
        {


            var criteria = _rightsvc.GetCriteria(IdLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var query = _uow.DataFacebookRepository.GetDataFacebook(criteriaData, Begin, End, Advertiser, Brand);
            //var res = query.GroupBy(e => new { e.IdAdvertiser }).Select((c) => new DataFacebookContract
            //{
            //    ID = c.First().IdAdvertiser,
            //    PID = -1,
            //    IdPageFacebook = c.First().IdPageFacebook,
            //    AdvertiserLabel = c.First().Advertiser.AdvertiserLabel,
            //    NbPage = c.Count(),
            //    PageName = "X",
            //    NumberPost = c.Sum(e => e.NumberPost),
            //    NumberLike = c.Sum(e => e.NumberLike),
            //    NumberShare = c.Sum(e => e.NumberShare),
            //    Expenditure = c.Sum(e => e.Expenditure),
            //    NumberFan = c.Max(e => e.NumberFan),
                
            //    PageFacebookContracts = c.GroupBy(u=> u.IdPageFacebook).Select(u => new PageFacebookContract()
            //    {
            //        PID = u.Key,
            //        NumberFan = u.First().NumberFan,
            //        NumberPost = u.First().NumberPost,
            //        Expenditure = u.First().Expenditure,
            //        NumberLike = u.First().NumberLike,
            //        NumberShare = u.First().NumberShare,
            //        PageName = u.First().PageName,
            //        IdAdvertiser = u.First().IdAdvertiser,
            //        IdPage = u.First().IdPageFacebook
            //    }).ToList()
            //}).ToList();
            return new List<DataFacebookContract>();
        }
    }
}
