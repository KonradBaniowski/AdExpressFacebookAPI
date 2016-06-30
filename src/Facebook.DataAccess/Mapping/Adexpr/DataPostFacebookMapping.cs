using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class DataPostFacebookMapping : EntityTypeConfiguration<DataPostFacebook>
    {
        public DataPostFacebookMapping(string schema)
        {
            HasKey(e => e.IdPostFacebook);
            ToTable("DATA_POST_FACEBOOK", schema);
            Property(e => e.IdPostFacebook).HasColumnName("ID_POST_FACEBOOK");
            Property(e => e.IdPageFacebook).HasColumnName("ID_PAGE_FACEBOOK");
            Property(e => e.IdPost).HasColumnName("ID_POST");
            Property(e => e.ProfilePictureUrl).HasColumnName("PROFILE_PICTURE_URL");
            Property(e => e.PostLink).HasColumnName("POST_LINK");
            Property(e => e.DateCreationPost).HasColumnName("DATE_CREATION_POST");
            Property(e => e.StatusType).HasColumnName("STATUS_TYPE");
            Property(e => e.Message).HasColumnName("MESSAGE");
            Property(e => e.Message2).HasColumnName("MESSAGE2");
            Property(e => e.Name).HasColumnName("NAME");
            Property(e => e.Picture).HasColumnName("PICTURE");
            Property(e => e.OriginalPicture).HasColumnName("ORIGINAL_PICTURE");
            Property(e => e.Link).HasColumnName("LINK");
            Property(e => e.Description).HasColumnName("DESCRIPTION");
            Property(e => e.Description2).HasColumnName("DESCRIPTION2");
            Property(e => e.AssociatedFile).HasColumnName("ASSOCIATED_FILE");
            Property(e => e.DateCreation).HasColumnName("DATE_CREATION");
            Property(e => e.DateModification).HasColumnName("DATE_MODIFICATION");
            Property(e => e.Commentary).HasColumnName("COMMENTARY");
            Property(e => e.Activation).HasColumnName("ACTIVATION");
            Property(e => e.VideoSource).HasColumnName("VIDEO_SOURCE");
            Property(e => e.AssociatedFileVideo).HasColumnName("ASSOCIATED_FILE_VIDEO");
            Property(e => e.IdTypePost).HasColumnName("ID_TYPE_POST");
            Property(e => e.IdLanguageData).HasColumnName("ID_LANGUAGE_DATA_I");
            Property(e => e.NumberLike).HasColumnName("NUMBER_LIKE");
            Property(e => e.NumberComment).HasColumnName("NUMBER_COMMENT");
            Property(e => e.NumberShare).HasColumnName("NUMBER_SHARE");
            Property(e => e.Commitment).HasColumnName("COMMITMENT");
            Property(e => e.IdBrand).HasColumnName("ID_BRAND");
            Property(e => e.IdAdvertiser).HasColumnName("ID_ADVERTISER");
        }
    }
}
