﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class MarketViewModel
    {
        public Labels Labels { get; set; }
        public Dimension Dimension { get; set; }
        public List<UniversBranch> Branches { get; set; }
        public List<SelectListItem> SavedUnivers { get; set; }
        public List<NavigationNode> NavigationBar { get; set; }
        public PresentationModel Presentation { get; set; }

    }

    public class Labels
    {
        public string KeyWordLabel { get; set; }// 972
        public string KeyWordDescription { get; set; }//phrase under search input 2287
        public string ErrorMessage { get; set; }//930
        public string KeyWord { get; set; }
        public string ElementLabel { get; set; }//2278
        public string BranchLabel { get; set; }//2272
        public string NoSavedUnivers { get; set; }
        public string UserSavedUniversLabel { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public string LoadUnivers { get; set; }
        public string Save { get; set; }
        public string Submit { get; set; }
        public string CleanSelectionMsg { get; set; }
    }

    public class UniversBranch
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int LabelId { get; set; }
        public bool IsSelected { get; set; }
        public List<UniversLevel> UniversLevels { get; set; }
    }

    public class UniversLevel
    {
        public long Id { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }
        public long Capacity { get; set; }
        public string OverLimitMessage { get; set; }//2286
        public string SecurityMessage { get; set; }//2285
        public string ExceptionMessage { get; set; }//922

        public long BranchId { get; set; }

        public List<UniversItem> UniversItems { get; set; }
    }

    public class UniversItem
    {
        
        public long Id { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }

    }

}