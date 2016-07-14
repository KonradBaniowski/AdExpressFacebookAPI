
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class MarketViewModel
    {
        public Labels Labels { get; set; }
        public Dimension Dimension { get; set; }
        public List<UniversBranch> Branches { get; set; }
        public List<NavigationNode> NavigationBar { get; set; }
        public PresentationModel Presentation { get; set; }
        public UserUniversGroupsModel UniversGroups { get; set; }
        public List<Tree> Trees { get; set; }
        public long CurrentModule { get;set;}
        public int MaxUniverseItems { get; set; }

    }


    public class Tree
    {
        public long LabelId { get; set; }
        public int Id { get; set; }
        public TNS.Classification.Universe.AccessType AccessType { get; set; }
        public List<UniversLevel> UniversLevels { get; set; }

        public TNS.FrameWork.DB.Constantes.Activation MyProperty { get; set; }

        public string Label { get; set; }
        public bool IsDefaultActive {get;set;}
    }


    public class UserUniversGroup
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public List<UserUnivers> UserUnivers { get; set; }
        public int FirstColumnSize { get; set; }
        public int SecondeColumnSize { get; set; }
    }

    public class UserUnivers
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<UniversLevel> Levels { get; set; }
    }

    public class UserUniversGroupsModel
    {
        public int SiteLanguage { get; set; }

        public List<UserUniversGroup> UserUniversGroups { get; set; }

        public long SaveUniversCode { get; set; }
        public long LoadUniversCode { get; set; }
        public long ModuleCode { get; set; }
        public long UserUniversCode { get; set; }
        public long ErrorMsgCode { get; set; }
        public long ModuleDecriptionCode { get; set; }
        public bool ShowUserSavedGroups { get; set; }
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