using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMI.PromoPSA.Web.Domain.Translation {
    /// <summary>
    /// Interface indicates that the class use the language parameter
    /// </summary>
    public interface ITranslation {
        int Language {
            get;
            set;
        }
    }
}
