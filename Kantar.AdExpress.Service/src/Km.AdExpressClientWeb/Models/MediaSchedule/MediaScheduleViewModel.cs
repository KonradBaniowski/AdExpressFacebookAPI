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

        public List<UniversBranch> Branches { get; set; }

        public List<Tree> Trees { get; set; }

        public PresentationModel Presentation { get; set; }
        public Labels Labels { get; set; }

    }

    public class Media
    {
        public string icon { get; set; }
        public Vehicles.names MediaEnum { get; set; }
        public int Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }


    public class ErrorMessage
    {
        public string EmptySelection { get; set; }

        public string SearchErrorMessage { get; set; }

        public string SocialErrorMessage { get; set; }

        public string UnitErrorMessage { get; set; }
    }

    
}