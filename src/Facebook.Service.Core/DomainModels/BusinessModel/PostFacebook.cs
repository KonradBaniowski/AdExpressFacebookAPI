﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.BusinessModel
{
    public class PostFacebook
    {
        public long IdPostFacebook { get; set; }
        public string Advertiser { get; set; }
        public string PageName { get; set; }
        public string IdPost { get; set; }
        public DateTime DateCreationPost { get; set; }
        public string NumberLikes { get; set; }
        public string NumberComments { get; set; }
        public string NumberShares { get; set; }
        public string Commitments { get; set; }
        public string Brand { get; set; }

      
    }
}
