using Kantar.AdExpress.Service.Core.Domain.Mau;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.MauMapping
{
    public class LoginMappingMau: EntityTypeConfiguration<Login>
    {
        public LoginMappingMau(string schema)
        {
            HasKey(t => t.Id);
            ToTable("LOGIN", schema);
            Property(t => t.Id).HasColumnName("ID_LOGIN");
            Property(t => t.IdType).HasColumnName("ID_TYPE_LOGIN");
            Property(t => t.IdContact).HasColumnName("ID_CONTACT");
            Property(t => t.LoginName).HasColumnName("LOGIN");
            Property(t => t.Password).HasColumnName("PASSWORD");
            Property(t => t.DateExpiration).HasColumnName("DATE_EXPIRED");
            Property(t => t.Activation).HasColumnName("ACTIVATION");
        }
    }
}
