﻿
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaScheduleService
    {
         List<Media> GetMedia(string idWebSession);
    }
}
