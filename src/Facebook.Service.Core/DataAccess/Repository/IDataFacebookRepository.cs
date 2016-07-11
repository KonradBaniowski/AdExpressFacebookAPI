﻿using Facebook.Service.Contract.BusinessModels;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataFacebookRepository: IGenericRepository<DataFacebook>
    {
        List<DateFacebookContract> GetDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
        List<DataFacebook> GetKPIDataFacebook(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
    }
}