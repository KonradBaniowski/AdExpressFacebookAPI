﻿using Facebook.Service.Core.DomainModels.AdExprSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Adexpr
{
    public class WebPlanMediaMonthMapping: EntityTypeConfiguration<WebPlanMediaMonth>
    {
        public WebPlanMediaMonthMapping(string schema)
        {
            HasKey(e => new { e.IdMedia, e.IdProduct, e.MonthMediaNum });
            ToTable("WEB_PLAN_MEDIA_MONTH", schema);
            Property(e => e.IdMedia).HasColumnName("ID_MEDIA");
            Property(e => e.TotalUnite).HasColumnName("TOTALUNITE");
            Property(e => e.IdProduct).HasColumnName("ID_PRODUCT");
            Property(e => e.MonthMediaNum).HasColumnName("MONTH_MEDIA_NUM");
            Property(e => e.IdSegment).HasColumnName("ID_SEGMENT");
            Property(e => e.IdGroup).HasColumnName("ID_GROUP_");
            Property(e => e.IdSubSector).HasColumnName("ID_SUBSECTOR");
            Property(e => e.IdSector).HasColumnName("ID_SECTOR");
            Property(e => e.IdAdvertiser).HasColumnName("ID_ADVERTISER");
            Property(e => e.IdBrand).HasColumnName("ID_BRAND");
            Property(e => e.IdCategory).HasColumnName("ID_CATEGORY");
            Property(e => e.IdVehicle).HasColumnName("ID_VEHICLE");
        }
    }
}
