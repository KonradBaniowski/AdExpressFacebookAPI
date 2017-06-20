using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Cells.Drawing;
using Kantar.AdExpress.Service.Core.Domain;
using KM.Framework.Constantes;

namespace Km.AdExpressClientWeb.Models.PortfolioAlert
{
    public class PortfolioAlertViewModel
    {
        public PortfolioAlertResultResponse AlertDatas { get; set; }
        public int IdLanguage { get; set; }
        public Labels Labels { get; set; }


        //public string VisualsLabel { get; set; }
        //public string AdvertiserLabel { get; set; }
        //public string ProductLabel { get; set; }
        //public string SectorLabel { get; set; }
        //public string GroupLabel { get; set; }
        //public string MediaPagingLabel { get; set; }
        //public string AreaPageLabel { get; set; }
        //public string AreaMmcLabel { get; set; }
        //public string ExpenditureEuroLabel { get; set; }
        //public string LocationLabel { get; set; } // Descriptifs
        //public string FormatLabel { get; set; }
        //public string ColorLabel { get; set; }
        //public string RankSectorLabel { get; set; }
        //public string RankGroupLabel { get; set; }
        //public string RankMediaLabel { get; set; }
        //public string CouvPath { get; set; }

    }
}