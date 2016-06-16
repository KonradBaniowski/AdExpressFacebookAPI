using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.MauSchema
{
    public class TemplateAssignment
    {
        public int IdTemplate { get; set; }
        public int IdProject { get; set; }
        public int IdLogin { get; set; }
        public int Activation { get; set; }

        public virtual Template Template { get; set; }
    }
}
