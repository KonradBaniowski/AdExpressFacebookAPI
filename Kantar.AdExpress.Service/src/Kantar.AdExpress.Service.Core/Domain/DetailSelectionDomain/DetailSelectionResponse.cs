using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain
{
    public class DetailSelectionResponse
    {
        public int SiteLanguage { get; set; }
        public string ModuleLabel { get; set; }
        public string NiveauDetailLabel { get; set; }
        public string UniteLabel { get; set; }
        public List<Tree> UniversMarket { get; set; }
        public List<Tree> UniversMedia { get; set; }
        public List<TextData> MediasSelected { get; set; }

        public string MediasSelectedLabel { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Dates { get; set; }

        public bool ShowDate { get; set; }
        public bool ShowUnivers { get; set; }
        public bool ShowUniversDetails { get; set; }
        public bool ShowMarket { get; set; }
        public bool ShowGenericlevelDetail { get; set; }
    }

    public class TextData
    {
        public long Id;
        public string Label;
    }
}
