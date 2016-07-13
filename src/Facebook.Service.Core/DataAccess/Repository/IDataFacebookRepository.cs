using Facebook.Service.Contract.BusinessModels;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataFacebookRepository: IGenericRepository<DataFacebook>
    {
        List<DateFacebookKPI> GetDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
        List<DataFacebook> GetKPIDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);

        List<DateFacebookKPI> GetKPIClassificationDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);

        /// <summary>
        /// PLURIMEDIA
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="Advertiser"></param>
        /// <param name="Brand"></param>
        /// <param name="idLanguage"></param>
        /// <returns></returns>
        List<DataFacebook> GetKPIPlurimediaDataFacebook(long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
    }
}
