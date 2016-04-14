using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain
{
    public class DetailSelectionResponse
    {
        public string ModuleLabel { get; set; }
        public string NiveauDetailLabel { get; set; }
        public string UniteLabel { get; set; }
        public List<Tree> UniversMarche { get; set; }
        public List<TextData> MediasSelected { get; set; }
    }

    public class TextData
    {
        public long Id;
        public string Label;
    }
}
