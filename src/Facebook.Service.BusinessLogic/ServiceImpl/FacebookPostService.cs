using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.DataAccess;
using AutoMapper;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System.Linq;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class FacebookPostService : IFacebookPostService
    {

        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        private readonly IRightService _rightsvc;
        const int NB_TOP_POST = 3;

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
            var postsFacebook = _uow.DataPostFacebookRepository.GetDataPostFacebook(criteriaData, begin, end, advertisers, brands, posts);

            var postsFacebbok = postsFacebook.Select(p =>
           new DataPostFacebookContract
           {
               IdPost = p.IdPost,
               IdPostFacebook = p.IdPostFacebook,
               Advertiser = p.Advertiser,
               Brand = p.Brand,
               DateCreationPost = p.DateCreationPost,
               Commitment = LastKPI(p.Commitment),
               NumberComment = LastKPI(p.NumberComment),
               NumberLike = LastKPI(p.NumberLike),
               NumberShare = LastKPI(p.NumberShare),
               PageName = p.PageName
           }).ToList();

            return postsFacebbok;
        }

        public List<PostFacebookContract> GetTopPostFacebook(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, List<long> posts)
        {
            var criteria = _rightsvc.GetCriteria(idLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var postsFacebook = _uow.DataPostFacebookRepository.GetDataPostFacebook(criteriaData, begin, end, advertisers, brands, posts);

            var postsFacebbok = postsFacebook.Select(p =>
           new
           {
               IdPost = p.IdPost,
               IdPostFacebook = p.IdPostFacebook,
               Advertiser = p.Advertiser,
               Brand = p.Brand,
               DateCreationPost = p.DateCreationPost,
               Commitment = LastKPI(p.Commitment),
               NumberComment = LastKPI(p.NumberComment),
               NumberLike = LastKPI(p.NumberLike),
               NumberShare = LastKPI(p.NumberShare),
               PageName = p.PageName,
               NumberComments = p.NumberComment,
               NumberLikes = p.NumberLike,
               NumberShares = p.NumberShare,
               Commitments = p.Commitment,
           }).OrderByDescending(p => p.Commitment).Take(NB_TOP_POST).Select(p => new PostFacebookContract
           {
               Advertiser = p.Advertiser,
               Brand = p.Brand,
               IdPost = p.IdPost,
               IdPostFacebook = p.IdPostFacebook,
               Commitments = p.Commitments,
               NumberComments = p.NumberComments,
               NumberLikes = p.NumberLikes,
               NumberShares = p.NumberShares,
               PageName = p.PageName
           }).ToList();

            return postsFacebbok;
        }



        private long LastKPI(string values)
        {
            if (!string.IsNullOrWhiteSpace(values))
            {
                return values.Split(',').Select(p => Convert.ToInt64(p)).ToList().Last();
            }

            return 0;
        }
    }
}
