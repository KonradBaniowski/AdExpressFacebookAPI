﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class DataPostFacebookContract
    {
        public long IdPostFacebook { get; set; }
        public string Advertiser { get; set; }
        public string Brand { get; set; }
        public string PageName { get; set; }
        public string IdPost { get; set; }
        public string DateCreationPost { get; set; }
        public long NumberLike { get; set; }
        public long NumberComment { get; set; }
        public long NumberShare { get; set; }
        public long Engagement { get; set; }
      
    }

   
}
