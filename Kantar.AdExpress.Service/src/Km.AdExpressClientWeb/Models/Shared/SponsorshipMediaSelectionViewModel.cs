﻿using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class SponsorshipMediaSelectionViewModel
    {
        public List<SponsorshipMediaList> SponsorshipMedias { get; set; }

        public NavigationBarViewModel NavigationBar { get; set; }

        public ErrorMessage ErrorMessage { get; set; }

        public List<UniversBranch> Branches { get; set; }

        public List<Tree> Trees { get; set; }

        public PresentationModel Presentation { get; set; }

        public Labels Labels { get; set; }

        public Dimension Dimension { get; set; }

        public UserUniversGroupsModel UniversGroups { get; set; }

        public long CurrentModule { get; set; }

        public bool CanRefineMediaSupport { get; set; }

        public bool Multiple { get; set; }
    }

    public class SponsorshipMediaList
    {
        public string Category { get; set; }

        public List<Kantar.AdExpress.Service.Core.Domain.Media> Medias { get; set; }

        public bool Multiple { get; set; }

        public List<int> IdMediasCommon { get; set; }
    }
}