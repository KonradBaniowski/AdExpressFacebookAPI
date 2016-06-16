using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.MauSchema
{
    public class OrderTemplateMedia
    {
        public int IdOrderTemplateMedia { get; set; }
        public int IdTypeMedia { get; set; }
        public int Exception { get; set; }
        public string ListMedia { get; set; }
        public virtual int IdTemplate { get; set; }
        public virtual TypeMedia TypeMedia { get; set; }
    }
}
