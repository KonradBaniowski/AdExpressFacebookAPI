using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.DataAccess.Mapping
{
    public class AddressMapping : EntityTypeConfiguration<Address>
    {
        public AddressMapping(string schema)
        {
            HasKey(t => t.IdAddress);
            ToTable("ADDRESS", schema);
            Property(t => t.IdAddress).HasColumnName("ID_ADDRESS");
            Property(t => t.IdLanguageData).HasColumnName("ID_LANGUAGE_DATA_I");
            
        }
    }
}
