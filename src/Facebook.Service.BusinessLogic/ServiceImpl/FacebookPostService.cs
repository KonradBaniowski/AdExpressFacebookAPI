using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.DataAccess;
using AutoMapper;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System.Linq;
using System.Globalization;

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


        public List<DataPostFacebookContract> GetDataPostFacebook(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, List<long> pages, int idLanguage)
        {
            var criteria = _rightsvc.GetCriteria(idLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var postsFacebook = _uow.DataPostFacebookRepository.GetDataPostFacebook(criteriaData, begin, end, advertisers, brands, pages, idLanguage);

            var postsFacebbok = postsFacebook.Select(p =>
           new DataPostFacebookContract
           {
               IdPost = p.IdPost,
               IdPostFacebook = p.IdPostFacebook,
               Advertiser = p.Advertiser,
               Brand = p.Brand,
               DateCreationPost = GetDate(p.DateCreationPost, idLanguage),//p.DateCreationPost.ToString(),
               Engagement = LastKPI(p.Commitments),
               NumberComment = LastKPI(p.NumberComments),
               NumberLike = LastKPI(p.NumberLikes),
               NumberShare = LastKPI(p.NumberShares),
               PageName = p.PageName
           }).ToList();

            return postsFacebbok;
        }

        public List<PostFacebookContract> GetTopPostFacebook(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, List<long> pages, int idLanguage)
        {
            var criteria = _rightsvc.GetCriteria(idLogin);
            var criteriaData = _mapper.Map<List<CriteriaData>>(criteria);
            var postsFacebook = _uow.DataPostFacebookRepository.GetDataPostFacebook(criteriaData, begin, end, advertisers, brands, pages, idLanguage);

            var postsFacebbok = postsFacebook.Select(p =>
           new
           {
               IdPost = p.IdPost,
               IdPostFacebook = p.IdPostFacebook,
               Advertiser = p.Advertiser,
               Brand = p.Brand,
               DateCreationPost = p.DateCreationPost,
               Commitment = LastKPI(p.Commitments),
               NumberComment = LastKPI(p.NumberComments),
               NumberLike = LastKPI(p.NumberLikes),
               NumberShare = LastKPI(p.NumberShares),
               PageName = p.PageName,
               NumberComments = p.NumberComments,
               NumberLikes = p.NumberLikes,
               NumberShares = p.NumberShares,
               Commitments = p.Commitments,
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

        public PostFacebookContract GetPostFacebook(long idPostFacebook, int idLanguage)
        {
            PostFacebookContract postFacebookContract  = new PostFacebookContract();
            var postsFacebook = _uow.DataPostFacebookRepository.GetDataPostFacebook(idPostFacebook, idLanguage);

            if (postsFacebook != null)
            {
               return _mapper.Map<PostFacebookContract>(postsFacebook);               
            }

            return postFacebookContract;
        }

        private long LastKPI(string values)
        {
            if (!string.IsNullOrWhiteSpace(values))
            {
                return values.Split(',').Select(p => Convert.ToInt64(p)).ToList().Last();
            }

            return 0;
        }

        private string GetDate(DateTime date, int idLanguage)
        {
            string format = (idLanguage == 33) ? "fr-Fr" : "en-GB";
            CultureInfo info = new CultureInfo(format);
            string result = date.ToString("d", info);
            return result;
        }
      
    }
}
