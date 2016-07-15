﻿using Facebook.Service.Contract.BusinessModels;
using System.Collections.Generic;

namespace Facebook.Service.Core.BusinessService
{
    public interface IRightService
    {
        List<Criteria> GetCriteria(int idLogin);
        List<Criteria> GetCriteria2(int idLogin);

        //List<Criteria> GetTemplateMediaRight(int idLogin);
        //List<Criteria> GetMediaRight(int idLogin);

        //List<Criteria> GetProductRight(int idLogin);

        //List<Criteria> GetTemplateProductRight(int idLogin);
    }
}
