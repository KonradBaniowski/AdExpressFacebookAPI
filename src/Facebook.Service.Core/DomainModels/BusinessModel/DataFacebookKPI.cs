﻿using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.BusinessModel
{
    public class DateFacebookContract
    {
        public long IdAdvertiser { get; set; }
        public long NumberPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberComment { get; set; }
        public long NumberShare { get; set; }
        public long NumberFan { get; set; }
        public long Expenditure { get; set; }
        public string AdvertiserLabel { get; set; }
        public string BrandLabel { get; set; }
        public string PageName { get; set; }
        public long IdPageFacebook { get; set; }
    }
}
