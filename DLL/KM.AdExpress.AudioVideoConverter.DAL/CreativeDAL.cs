using BLToolkit.Data.DataProvider;
using KM.AdExpress.AudioVideoConverter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.DAL
{
    public class CreativeDAL
    {
        const int INACTIVATION_CODE = 10;
        const int ACTIVATION_CODE = 0;
        const int LANGUAGE_CODE = 33;
        const string PITAGOR_SCHEMA = "PITAFR01";
        const int ID_VEHICLE_RADIO = 2;
        const int ID_VEHICLE_TV = 3;
        const int FORM_INTER = 10;

         public AudioVideoConvertDbConnectionFactory AudioVideoConverterDB { get; set; }

         public CreativeDAL(String connexionString, String providerName)
        {
            AudioVideoConverterDB = new AudioVideoConvertDbConnectionFactory(connexionString, new GenericDataProvider(providerName));
           
        }

         public List<CreativeInformation> GetCreative(AudioVideoConverterDB db, DateTime dateCreationBegin,DateTime dateCreationEnd,long idVehicle)
         {
             StringBuilder sqlQuery = new StringBuilder();

             sqlQuery.Append("SELECT DISTINCT mm.id_multimedia as IdMultimedia");
             sqlQuery.Append(",to_number(to_char(mm.date_creation,'YYYY')) as MediaYear ");
             sqlQuery.Append(" ,file_name as FileName, file_path as FilePath");

             sqlQuery.Append(" FROM PITAFR01.MULTIMEDIA  mm,  PITAFR01.MULTIMEDIA_FILE  mf");

             sqlQuery.AppendFormat(" WHERE mm.date_creation >= to_date('{0}','DD/MM/YYYY')", dateCreationBegin.ToString("dd/MM/yyyy"));
             sqlQuery.AppendFormat(" and mm.date_creation < to_date('{0}','DD/MM/YYYY')", dateCreationEnd.AddDays(1).ToString("dd/MM/yyyy"));
             sqlQuery.AppendFormat(" and mm.ID_VEHICLE_I = {0} and mm.activation < {1}", idVehicle, INACTIVATION_CODE);            
             sqlQuery.AppendFormat(" and mm.ID_LANGUAGE_DATA_I  = {0} and mm.ID_CATEGORY_MULTIMEDIA in ", LANGUAGE_CODE);
             sqlQuery.Append(" ( ");
             sqlQuery.Append("  SELECT distinct  ID_CATEGORY_MULTIMEDIA ");
             sqlQuery.AppendFormat(" FROM PITAFR01.CATEGORY_MULTIMEDIA where ID_VEHICLE_I = {0} ",idVehicle);
             sqlQuery.AppendFormat(" and ID_LANGUAGE_DATA  = {0} and activation < {1}",LANGUAGE_CODE,INACTIVATION_CODE);
             sqlQuery.Append(" ) ");
             sqlQuery.Append(" and  mm.id_multimedia = mf.id_multimedia  ");
             sqlQuery.AppendFormat(" and  mf.ID_LANGUAGE_DATA_I  = {0}  and mf.activation < {1} ",LANGUAGE_CODE,INACTIVATION_CODE);
              
             var dbCmd = db.SetCommand(sqlQuery.ToString());
             return db.ExecuteList<CreativeInformation>();
         }

         public List<CreativeInformation> GetRadioCreative(AudioVideoConverterDB db, DateTime dateMediaBeginning, DateTime dateMediaEnd, DateTime dateCreation, string idMultimedias =null)
        {
           

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("select distinct IdMultimedia,MediaYear,FileName,FilePath from ( ");

            sqlQuery.Append("select distinct dv.id_multimedia as IdMultimedia, to_number(to_char(dr.date_media,'YYYY')) as MediaYear ");
            sqlQuery.Append(" ,file_name as FileName, file_path as FilePath ");
           // sqlQuery.Append(",ROW_NUMBER() OVER(PARTITION BY dv.id_multimedia ORDER BY to_number(to_char(dr.date_media,'YYYY'))) rowNumber ");
            sqlQuery.AppendFormat(" from {0}.DATA_RADIO  dr,{0}.DATA_VERSION dv", PITAGOR_SCHEMA);
            sqlQuery.AppendFormat("  , {0}.MULTIMEDIA  mm,  {0}.MULTIMEDIA_FILE  mf ", PITAGOR_SCHEMA);
            sqlQuery.Append(" where ");
            sqlQuery.AppendFormat(" dr.date_creation >= to_date('{0}','DD/MM/YYYY')",dateCreation.ToString("dd/MM/yyyy"));
            //sqlQuery.Append(" and dr.date_creation < to_date('01/06/2015','DD/MM/YYYY') ");//A Enlever apres la fin du traitement

            sqlQuery.AppendFormat(" and dr.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
                , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and dv.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
              , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));

            sqlQuery.AppendFormat(" and dv.id_data_version = dr.id_data_version ");
            sqlQuery.AppendFormat(" and dv.ID_LANGUAGE_DATA_I  =  {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and dr.id_language_data_i = dv.ID_LANGUAGE_DATA_I ");
            sqlQuery.AppendFormat(" and dv.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.AppendFormat(" and dr.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.Append(" and dv.id_multimedia = mm.id_multimedia ");
            sqlQuery.AppendFormat(" and mm.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mm.activation < {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and mm.id_multimedia = mf.id_multimedia ");
            sqlQuery.AppendFormat(" and mf.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mf.activation <{0} ", INACTIVATION_CODE);
            if (!string.IsNullOrEmpty(idMultimedias)) sqlQuery.AppendFormat(" and mm.id_multimedia in ({0})", idMultimedias);
            sqlQuery.Append(" and dv.id_multimedia not in ( ");
            sqlQuery.Append(" select to_number(substr(id_strike,2,LENGTH(id_strike)-1)) as id_multimedia  ");
            sqlQuery.AppendFormat(" from form@ORQUAF01.PIGE   where date_creation >=to_date('{0}','dd/mm/yyyy')"
                , dateCreation.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and  id_vehicle = {0} and activation = {1} and form_inter<={2} "
                , ID_VEHICLE_RADIO, ACTIVATION_CODE,FORM_INTER);
            sqlQuery.Append(" ) ");

            sqlQuery.Append(" group by   dv.id_multimedia,to_number(to_char(dr.date_media,'YYYY')),file_name ,file_path");
            //sqlQuery.Append(" )   where  rowNumber = 1");
            sqlQuery.Append(" )  ");

            var dbCmd = db.SetCommand(sqlQuery.ToString());
            return db.ExecuteList<CreativeInformation>();
        }

         public List<CreativeInformation> GetTvSponsorshipCreative(AudioVideoConverterDB db, DateTime dateMediaBeginning, DateTime dateMediaEnd, DateTime dateCreation, string idMultimedias = null)
        {


            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("select distinct IdMultimedia,MediaYear,FileName,FilePath from ( ");

            sqlQuery.Append("select distinct dv.id_multimedia as IdMultimedia, to_number(to_char(ds.date_media,'YYYY')) as MediaYear ");
            sqlQuery.Append(" ,file_name as FileName, file_path as FilePath ");
           // sqlQuery.Append(",ROW_NUMBER() OVER(PARTITION BY dv.id_multimedia ORDER BY to_number(to_char(ds.date_media,'YYYY'))) rowNumber ");
            sqlQuery.AppendFormat(" from {0}.DATA_SPONSORSHIP  ds,{0}.DATA_VERSION dv", PITAGOR_SCHEMA);
            sqlQuery.AppendFormat("  , {0}.MULTIMEDIA  mm,  {0}.MULTIMEDIA_FILE  mf ", PITAGOR_SCHEMA);
            sqlQuery.Append(" where ");
            sqlQuery.AppendFormat(" ds.date_creation >= to_date('{0}','DD/MM/YYYY')", dateCreation.ToString("dd/MM/yyyy"));
            //sqlQuery.Append(" and dv.date_creation < to_date('01/06/2015','DD/MM/YYYY') ");//A Enlever apres la fin du traitement

            sqlQuery.AppendFormat(" and ds.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
                , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and dv.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
              , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));

            sqlQuery.AppendFormat(" and dv.id_data_version = ds.id_data_version ");
            sqlQuery.AppendFormat(" and dv.ID_LANGUAGE_DATA_I  =  {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and ds.id_language_data_i = dv.ID_LANGUAGE_DATA_I ");
            sqlQuery.AppendFormat(" and dv.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.AppendFormat(" and ds.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.Append(" and dv.id_multimedia = mm.id_multimedia ");
            sqlQuery.AppendFormat(" and mm.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mm.activation < {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and mm.id_multimedia = mf.id_multimedia ");
            sqlQuery.AppendFormat(" and mf.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mf.activation <{0} ", INACTIVATION_CODE);
            if (!string.IsNullOrEmpty(idMultimedias)) sqlQuery.AppendFormat(" and mm.id_multimedia in ({0})", idMultimedias);
            sqlQuery.Append(" and dv.id_multimedia not in ( ");
            sqlQuery.Append(" select to_number(substr(id_strike,2,LENGTH(id_strike)-1)) as id_multimedia  ");
            sqlQuery.AppendFormat(" from form@ORQUAF01.PIGE   where date_creation >=to_date('{0}','dd/mm/yyyy')"
                , dateCreation.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and  id_vehicle = {0} and activation = {1} and form_inter<={2} "
                , ID_VEHICLE_TV, ACTIVATION_CODE, FORM_INTER);
            sqlQuery.Append(" ) ");

            sqlQuery.Append(" group by   dv.id_multimedia,to_number(to_char(ds.date_media,'YYYY')),file_name ,file_path");
            //sqlQuery.Append(" )   where  rowNumber = 1");
            sqlQuery.Append(" )  ");
           

            var dbCmd = db.SetCommand(sqlQuery.ToString());
            return db.ExecuteList<CreativeInformation>();
        }

         public List<CreativeInformation> GetTvCreative(AudioVideoConverterDB db, DateTime dateMediaBeginning, DateTime dateMediaEnd, DateTime dateCreation,  string idMultimedias = null)
        {
            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.Append("select IdMultimedia,MediaYear,FileName,FilePath from ( ");

            sqlQuery.Append("select distinct dv.id_multimedia as IdMultimedia, to_number(to_char(dt.date_media,'YYYY')) as MediaYear ");
            sqlQuery.Append(" ,file_name as FileName, file_path as FilePath ");
           // sqlQuery.Append(",ROW_NUMBER() OVER(PARTITION BY dv.id_multimedia ORDER BY to_number(to_char(dt.date_media,'YYYY'))) rowNumber ");
            sqlQuery.AppendFormat(" from {0}.DATA_TV  dt,{0}.DATA dv", PITAGOR_SCHEMA);
            sqlQuery.AppendFormat("  , {0}.MULTIMEDIA  mm,  {0}.MULTIMEDIA_FILE  mf ", PITAGOR_SCHEMA);
            sqlQuery.Append(" where ");
            sqlQuery.AppendFormat(" dt.date_creation >= to_date('{0}','DD/MM/YYYY')", dateCreation.ToString("dd/MM/yyyy"));
            //sqlQuery.Append(" and dt.date_creation < to_date('01/06/2015','DD/MM/YYYY') ");//A Enlever apres la fin du traitement     
            sqlQuery.AppendFormat(" and dt.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
                , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and dv.date_media between to_date('{0}','DD/MM/YYYY') and to_date('{1}','DD/MM/YYYY')"
              , dateMediaBeginning.ToString("dd/MM/yyyy"), dateMediaEnd.ToString("dd/MM/yyyy"));

            sqlQuery.AppendFormat(" and dv.id_data = dt.id_data ");
            sqlQuery.AppendFormat(" and dv.id_media = dt.id_media ");
            sqlQuery.AppendFormat(" and dv.ID_LANGUAGE_DATA_I  =  {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and dt.id_language_data_i = dv.ID_LANGUAGE_DATA_I ");
            sqlQuery.AppendFormat(" and dv.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.AppendFormat(" and dt.activation < {0} ", INACTIVATION_CODE);
            sqlQuery.Append(" and dv.id_multimedia = mm.id_multimedia ");
            sqlQuery.AppendFormat(" and mm.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mm.activation < {0} ", LANGUAGE_CODE);
            sqlQuery.Append(" and mm.id_multimedia = mf.id_multimedia ");
            sqlQuery.AppendFormat(" and mf.ID_LANGUAGE_DATA_I  = {0} ", LANGUAGE_CODE);
            sqlQuery.AppendFormat(" and mf.activation <{0} ", INACTIVATION_CODE);
            if (!string.IsNullOrEmpty(idMultimedias)) sqlQuery.AppendFormat(" and mm.id_multimedia in ({0})", idMultimedias);
            sqlQuery.Append(" and dv.id_multimedia not in ( ");
            sqlQuery.Append(" select to_number(substr(id_strike,2,LENGTH(id_strike)-1)) as id_multimedia  ");
            sqlQuery.AppendFormat(" from form@ORQUAF01.PIGE   where date_creation >=to_date('{0}','dd/mm/yyyy')"
                , dateCreation.ToString("dd/MM/yyyy"));
            sqlQuery.AppendFormat(" and  id_vehicle = {0} and activation = {1} and form_inter<={2} "
                , ID_VEHICLE_TV, ACTIVATION_CODE, FORM_INTER);
            sqlQuery.Append(" ) ");

            sqlQuery.Append(" group by   dv.id_multimedia,to_number(to_char(dt.date_media,'YYYY')),file_name ,file_path");
            //sqlQuery.Append(" )   where  rowNumber = 1");
            sqlQuery.Append(" )  ");

            var dbCmd = db.SetCommand(sqlQuery.ToString());
            return db.ExecuteList<CreativeInformation>();
        }
    }
}
