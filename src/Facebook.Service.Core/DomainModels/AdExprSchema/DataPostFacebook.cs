using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class DataPostFacebook
    {
        public long IdPostFacebook { get; set; }
        public long IdPageFacebook { get; set; }
        public string IdPost { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string PostLink { get; set; }
        public DateTime DateCreationPost { get; set; }
        public string StatusType { get; set; }
        public string Message { get; set; }
        public string Message2 { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string OriginalPicture { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string AssociatedFile { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateModification { get; set; }
        public string Commentary { get; set; }
        public long Activation { get; set; }
        public string VideoSource { get; set; }
        public string AssociatedFileVideo { get; set; }
        public long IdTypePost { get; set; }
        public long IdLanguageData { get; set; }
        public long NumberLike { get; set; }
        public long NumberComment { get; set; }
        public long NumberShare { get; set; }
    }
}
