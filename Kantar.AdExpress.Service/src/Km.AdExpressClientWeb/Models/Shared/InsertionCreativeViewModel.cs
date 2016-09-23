using System.Collections.Generic;
using System;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class InsertionCreativeViewModel
    {
        public InsertionCreativeViewModel()
        {
            paramsUrl = new List<string>();
        }
        public List<string> paramsUrl { get; set; }
        public int SiteLanguage { get; set; }
        public Labels Labels { get; set; }
        public bool IsAdNetTrack { get; set; }
    }

}