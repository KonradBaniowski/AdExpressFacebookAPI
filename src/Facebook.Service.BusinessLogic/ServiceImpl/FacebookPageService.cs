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

            var data = query.GroupBy(e => e.IdAdvertiser);
            List<DataFacebookContract> contract = new List<DataFacebookContract>();
            foreach (var i in data)
            {
                DataFacebookContract item = new DataFacebookContract();
                item.PID = -1;
                item.ID = i.FirstOrDefault().IdAdvertiser;
                item.AdvertiserLabel = i.FirstOrDefault().AdvertiserLabel;
                item.BrandLabel = i.FirstOrDefault().BrandLabel;
                item.Expenditure = i.Sum(e => e.Expenditure);
                item.NbPage = i.Count();
                item.NumberPost = i.Sum(a => a.NumberPost);
                item.NumberLike = i.Sum(a => a.NumberLike);
                item.NumberComment = i.Sum(a => a.NumberComment);
                item.NumberShare = i.Sum(a => a.NumberShare);
                item.NumberFan = i.Max(a => a.NumberFan);
                item.PageFacebookContracts = new List<PageFacebookContract>();
                foreach(var elem in i)
                {
                    PageFacebookContract page = new PageFacebookContract();
                    page.PID = i.FirstOrDefault().IdAdvertiser;
                    page.Expenditure = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).Expenditure;
                    page.IdPageFacebook = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).IdPageFacebook;
                    page.NumberPost = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).NumberPost;
                    page.NumberLike = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).NumberLike;
                    page.NumberComment = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).NumberComment;
                    page.NumberShare = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).NumberShare;
                    page.NumberFan = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).NumberFan;
                    page.PageName = i.FirstOrDefault(e => e.IdPageFacebook == elem.IdPageFacebook).PageName;
                    item.PageFacebookContracts.Add(page);
                }
                contract.Add(item);
            }
            return contract;
        }

        public List<DataPostFacebookContract> GetDataPostFacebook(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> Brand, List<long> Post)
        {
            var criteria = _rightsvc.GetCriteria(IdLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            throw new NotImplementedException();
        }
    }
}
