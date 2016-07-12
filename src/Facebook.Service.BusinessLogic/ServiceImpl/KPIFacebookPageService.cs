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
                Expenditure = e.Sum(a => a.Expenditure)
            }).OrderByDescending(a => a.DateMediaNum).ToList();
            var kpiResult = _mapper.Map<List<KPIPageFacebookContract>>(query);
            return kpiResult;
        }

        public List<KPIPercentPageFacebookContract> GetKPIPlurimediaPages(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> AdvertiserCon, List<long> Brand, List<long> BrandCon, int idLanguage)
        {
            IEnumerable<WebPlanMediaMonth> pluriAll = null;
            IEnumerable<WebPlanMediaMonth> pluriRef = null;
            IEnumerable<DataFacebook> facebookAll = null;
            IEnumerable<DataFacebook> facebookRef = null;
            List<KPIPercentPageFacebookContract> percentByMonth = new List<KPIPercentPageFacebookContract>();

            var YYYYMMDDBegin = long.Parse(Begin.ToString().Substring(0, 6));
            var YYYYMMDDEnd = long.Parse(End.ToString().Substring(0, 6));

            if ((Advertiser != null && Advertiser.Count() != 0) && (AdvertiserCon != null && AdvertiserCon.Count() != 0))
            {
                var allAdvertiser = Advertiser.Concat(AdvertiserCon).ToList();
                pluriAll = _uow.WebPlanMediaMonthRepository.Find(e => e.MonthMediaNum >= YYYYMMDDBegin && e.MonthMediaNum <= YYYYMMDDEnd && allAdvertiser.Contains(e.IdAdvertiser));

                pluriRef = _uow.WebPlanMediaMonthRepository.Find(e => e.MonthMediaNum >= YYYYMMDDBegin && e.MonthMediaNum <= YYYYMMDDEnd && Advertiser.Contains(e.IdAdvertiser));

                facebookAll = _uow.DataFacebookRepository.Find(e => e.DateMediaNum >= Begin && e.DateMediaNum <= End && allAdvertiser.Contains(e.IdAdvertiser) && e.IdLanguageData == idLanguage);

                facebookRef = _uow.DataFacebookRepository.Find(e => e.DateMediaNum >= Begin && e.DateMediaNum <= End && Advertiser.Contains(e.IdAdvertiser) && e.IdLanguageData == idLanguage);

            }
            else if ((Brand != null && Brand.Count() != 0) && (BrandCon != null && BrandCon.Count() != 0))
            {
                var allBrand = Brand.Concat(BrandCon).ToList();
                pluriAll = _uow.WebPlanMediaMonthRepository.Find(e => e.MonthMediaNum >= YYYYMMDDBegin && e.MonthMediaNum <= YYYYMMDDEnd && allBrand.Contains(e.IdAdvertiser));

                pluriRef = _uow.WebPlanMediaMonthRepository.Find(e => e.MonthMediaNum >= YYYYMMDDBegin && e.MonthMediaNum <= YYYYMMDDEnd && Brand.Contains(e.IdAdvertiser));

                facebookAll = _uow.DataFacebookRepository.Find(e => e.DateMediaNum >= Begin && e.DateMediaNum <= End && allBrand.Contains(e.IdAdvertiser) && e.IdLanguageData == idLanguage);

                facebookRef = _uow.DataFacebookRepository.Find(e => e.DateMediaNum >= Begin && e.DateMediaNum <= End && Brand.Contains(e.IdAdvertiser) && e.IdLanguageData == idLanguage);
            }

            pluriAll = pluriAll.GroupBy(e => e.MonthMediaNum)
                   .Select(e => new WebPlanMediaMonth
                   {
                       MonthMediaNum = e.FirstOrDefault().MonthMediaNum,
                       TotalUnite = e.Sum(a => a.TotalUnite)
                   }).OrderByDescending(r => r.MonthMediaNum).ToList();

            pluriRef = pluriRef.GroupBy(e => e.MonthMediaNum)
                    .Select(e => new WebPlanMediaMonth
                    {
                        MonthMediaNum = e.FirstOrDefault().MonthMediaNum,
                        TotalUnite = e.Sum(a => a.TotalUnite)
                    }).OrderByDescending(r => r.MonthMediaNum).ToList();


            facebookAll = facebookAll.GroupBy(e => e.DateMediaNum)
                     .Select(e => new DataFacebook
                     {
                         DateMediaNum = e.FirstOrDefault().DateMediaNum,
                         Expenditure = e.Sum(a => a.Expenditure)
                     }).OrderByDescending(r => r.DateMediaNum).ToList();

            facebookRef = facebookRef.GroupBy(e => e.DateMediaNum)
                     .Select(e => new DataFacebook
                     {
                         DateMediaNum = e.FirstOrDefault().DateMediaNum,
                         Expenditure = e.Sum(a => a.Expenditure)
                     }).OrderByDescending(r => r.DateMediaNum).ToList();

            foreach (var item in pluriAll)
            {
                var percentFind = new KPIPercentPageFacebookContract();
                var x = pluriRef.Where(e => e.MonthMediaNum == item.MonthMediaNum).FirstOrDefault();

                if (x != null && x.TotalUnite.HasValue && item.TotalUnite.HasValue)
                {
                    percentFind.ReferentPercent = (x.TotalUnite.Value * 100) / item.TotalUnite.Value;
                    percentFind.ConcurrentPercent = 100 - percentFind.ReferentPercent;
                    percentFind.Month = item.MonthMediaNum.ToString();
                }
                percentByMonth.Add(percentFind);
            }

            List<KPIPercentPageFacebookContract> percentFBByMonth = new List<KPIPercentPageFacebookContract>();
            foreach (var item in facebookAll)
            {
                var percentFind = new KPIPercentPageFacebookContract();
                var x = facebookRef.Where(e => e.DateMediaNum == item.DateMediaNum).FirstOrDefault();

                if (x != null)
                {
                    percentFind.ReferentFBPercent = (x.Expenditure * 100) / item.Expenditure;
                    percentFind.ConcurrentFBPercent = 100 - percentFind.ReferentFBPercent;
                    percentFind.Month = item.DateMediaNum.ToString();
                }
                percentFBByMonth.Add(percentFind);
            }
            var dico = percentFBByMonth.ToDictionary(e => long.Parse(e.Month.Substring(0, 6)).ToString(), x => x);

            foreach (var item in percentByMonth)
            {
                KPIPercentPageFacebookContract elem = null;
                if (dico.TryGetValue(item.Month, out elem))
                {
                    item.ReferentFBPercent = elem.ReferentFBPercent;
                    item.ConcurrentFBPercent = elem.ConcurrentFBPercent;
                }
            }

            return percentByMonth;
        }
    }
}
