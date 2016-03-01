﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class MediaSelectionViewModel
    {
        public List<Kantar.AdExpress.Service.Core.Domain.Media> Medias { get; set; }
        public bool Multiple { get; set; }
        public List<int> IdMediasCommon { get; set; }

        public List<NavigationNode> NavigationBar { get; set; }

        public ErrorMessage ErrorMessage { get; set; }

        public PresentationModel Presentation { get; set; }

    }

    public class Media
    {
        public string icon { get; set; }
        public Vehicles.names MediaEnum { get; set; }
        public int Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }

    public class NavigationNode
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public int Position { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string IconCssClass { get; set; }



    }

    public class ErrorMessage
    {
        public string EmptySelection { get; set; }

        public string SearchErrorMessage { get; set; }

        public string SocialErrorMessage { get; set; }

        public string UnitErrorMessage { get; set; }
    }

    #region Temp classes
    public class  GenericClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AdvertiserVM :GenericClass
    {
    }

    public class Company :GenericClass
    {
    }
    public class Product : GenericClass
    {
    }
    public class AdvertiserModel
    {
         public Company HoldingCompany { get; set; }
        public AdvertiserVM Advertiser { get; set; }
        public Product Product { get; set; }
    }

    public class BrandModel
    {
        public BrandVM Brand { get; set; }
        public Product Product { get; set; }

    }
     public class BrandVM:GenericClass
    { }
    public class SectorModel
    {
        public GenericClass Sector { get; set; }//Famille
        public GenericClass SubSector { get; set; }//Classe

        public GenericClass Group { get; set; }//Groupe

        public GenericClass Segment { get; set; }//Variete
    }

    public class MarketModel
    {
        public List<SelectListItem> SearchCriteria { get; set; }
        public List<AdvertiserModel> Advertisers { get; set; }

        public List<BrandModel> Brands { get; set; }

        public List<SectorModel> Sectors { get; set; }

        public List<NavigationNode> NavigationBar { get; set;}

    }
    #endregion
}