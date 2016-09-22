using Facebook.Service.Core.DomainModels.RecpaSchema;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Mapping.Recap
{
    class RecapPluriSegmentMapping : EntityTypeConfiguration<RecapPluriSegment>
    {
        public RecapPluriSegmentMapping(string schema)
        {

            //TODO : demander
            HasKey(e => new { e.IdLanguageData });
            //HasKey(e => new { e.IdMedia });


            ToTable("RECAP_PLURI_SEGMENT", schema);
            Property(e => e.IdLanguageData).HasColumnName("ID_LANGUAGE_I");

            Property(e => e.Expenditure_Euro_N_1).HasColumnName("EXP_EURO_N_1");
            Property(e => e.Expenditure_Euro_N_2).HasColumnName("EXP_EURO_N_2");
            Property(e => e.Expenditure_Euro_N_3).HasColumnName("EXP_EURO_N_3");
            Property(e => e.Expenditure_Euro_N_4).HasColumnName("EXP_EURO_N_4");
            Property(e => e.Expenditure_Euro_N_5).HasColumnName("EXP_EURO_N_5");
            Property(e => e.Expenditure_Euro_N_6).HasColumnName("EXP_EURO_N_6");
            Property(e => e.Expenditure_Euro_N_7).HasColumnName("EXP_EURO_N_7");
            Property(e => e.Expenditure_Euro_N_8).HasColumnName("EXP_EURO_N_8");
            Property(e => e.Expenditure_Euro_N_9).HasColumnName("EXP_EURO_N_9");
            Property(e => e.Expenditure_Euro_N_10).HasColumnName("EXP_EURO_N_10");
            Property(e => e.Expenditure_Euro_N_11).HasColumnName("EXP_EURO_N_11");
            Property(e => e.Expenditure_Euro_N_12).HasColumnName("EXP_EURO_N_12");

            Property(e => e.Expenditure_Euro_N1_1).HasColumnName("EXP_EURO_N1_1");
            Property(e => e.Expenditure_Euro_N1_2).HasColumnName("EXP_EURO_N1_2");
            Property(e => e.Expenditure_Euro_N1_3).HasColumnName("EXP_EURO_N1_3");
            Property(e => e.Expenditure_Euro_N1_4).HasColumnName("EXP_EURO_N1_4");
            Property(e => e.Expenditure_Euro_N1_5).HasColumnName("EXP_EURO_N1_5");
            Property(e => e.Expenditure_Euro_N1_6).HasColumnName("EXP_EURO_N1_6");
            Property(e => e.Expenditure_Euro_N1_7).HasColumnName("EXP_EURO_N1_7");
            Property(e => e.Expenditure_Euro_N1_8).HasColumnName("EXP_EURO_N1_8");
            Property(e => e.Expenditure_Euro_N1_9).HasColumnName("EXP_EURO_N1_9");
            Property(e => e.Expenditure_Euro_N1_10).HasColumnName("EXP_EURO_N1_10");
            Property(e => e.Expenditure_Euro_N1_11).HasColumnName("EXP_EURO_N1_11");
            Property(e => e.Expenditure_Euro_N1_12).HasColumnName("EXP_EURO_N1_12");

            Property(e => e.Expenditure_Euro_N2_1).HasColumnName("EXP_EURO_N2_1");
            Property(e => e.Expenditure_Euro_N2_2).HasColumnName("EXP_EURO_N2_2");
            Property(e => e.Expenditure_Euro_N2_3).HasColumnName("EXP_EURO_N2_3");
            Property(e => e.Expenditure_Euro_N2_4).HasColumnName("EXP_EURO_N2_4");
            Property(e => e.Expenditure_Euro_N2_5).HasColumnName("EXP_EURO_N2_5");
            Property(e => e.Expenditure_Euro_N2_6).HasColumnName("EXP_EURO_N2_6");
            Property(e => e.Expenditure_Euro_N2_7).HasColumnName("EXP_EURO_N2_7");
            Property(e => e.Expenditure_Euro_N2_8).HasColumnName("EXP_EURO_N2_8");
            Property(e => e.Expenditure_Euro_N2_9).HasColumnName("EXP_EURO_N2_9");
            Property(e => e.Expenditure_Euro_N2_10).HasColumnName("EXP_EURO_N2_10");
            Property(e => e.Expenditure_Euro_N2_11).HasColumnName("EXP_EURO_N2_11");
            Property(e => e.Expenditure_Euro_N2_12).HasColumnName("EXP_EURO_N2_12");                                                                        
        }
    }
}
