using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.Aphrodite.DAL {
    /// <summary>
    /// Trends DAL
    /// </summary>
    public class TrendsDAL {

        /// <summary>
        /// Build Remove Query for a table;
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="queries"></param>
        private static void BuildRemoveQuery(string tableName,Int64 mediaTypeId,Queue<string> queries) {
            if(tableName.Length>0) {
                queries.Enqueue("Delete from "+tableName+" where id_vehicle="+mediaTypeId.ToString());
            }
        }



        /// <summary>
        /// Remove trends data for a media
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        public static void Remove(MediaTypeInformation mediaTypeInformation,IDataSource source) {

            Queue<string> queries=new Queue<string>(4);

            #region Buid queries
            BuildRemoveQuery(mediaTypeInformation.MonthTrendsTable,mediaTypeInformation.DatabaseId,queries);
            BuildRemoveQuery(mediaTypeInformation.TotalMonthTrendsTable,mediaTypeInformation.DatabaseId,queries);
            BuildRemoveQuery(mediaTypeInformation.WeekTrendsTable,mediaTypeInformation.DatabaseId,queries);
            BuildRemoveQuery(mediaTypeInformation.TotalWeekTrendsTable,mediaTypeInformation.DatabaseId,queries);
            if(queries.Count==0) throw(new AphroditeDALException("Impossible to build data remove queries: Maybe bad Media type xml file"));
            #endregion

            #region Remove data
            string query=string.Empty;
            try {
                source.Open();
                lock(queries) {
                    while(queries.Count>0) {
                        query=queries.Dequeue();
                        source.Delete(query);
                    }
                }
            }
            catch(System.Exception err) {
                throw (new AphroditeDALException("Impossible to remove trends data: "+query,err));
            }
            finally {
                source.Close();
            }
            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodBeginning"></param>
        /// <param name="periodEnding"></param>
        /// <param name="periodBeginningPrev"></param>
        /// <param name="periodEndingPrev"></param>
        /// <param name="periodId"></param>
        /// <param name="period"></param>
        /// <param name="year"></param>
        /// <param name="mediaTypeInformation"></param>
        /// <param name="cumul"></param>
        /// <param name="source"></param>
        public static void InsertMonth(string periodBeginning,string periodEnding,string periodBeginningPrev,string periodEndingPrev,string periodId,string period,string year,MediaTypeInformation mediaTypeInformation,string cumul,IDataSource source) {

            #region Variables

            string sql="";
            string getUnitsForTotal=mediaTypeInformation.GetTotalUnitsInsertSQLFields;
            string getUnitsForSelectCur=mediaTypeInformation.GetUnitsSelectSQLForCurrentYear;
            string getUnitsForSelectPrev=mediaTypeInformation.GetUnitsSelectSQLForPreviousYear;
            string getUnitForInset=mediaTypeInformation.GetUnitsInsertSQLFields;
            //string listMedia=SqlGenerator.GetListMedia(vehicle);
            //string listMedia=vehicle.MediaList;
            //string listExcludeProduct=TNS.AdExpress.Hathor.Constantes.Constantes.LIST_EXCLUDE_PRODUCT;

            //string tmp1="";
            string tmp2="";
            string tmp3="";
            string tmp4="";
            #endregion

            #region Construction de la requête
            sql+="insert into "+mediaTypeInformation.MonthTrendsTable+" (id_media ";
            sql+=",id_cumulative ";
            sql+=",date_period ";
            sql+=",id_pdm ";
            sql+=",id_period";
            sql+=",id_vehicle ";
            sql+=",id_category ";
            sql+=",category ";
            sql+=",media ";
            sql+=" ,year ";
            sql+=getUnitForInset;
            sql+=" ) ";

            #region tmp
            tmp2+=" date_media_num between "+periodBeginning+" and "+periodEnding+" ";
            tmp3+=" date_media_num between "+periodBeginningPrev+" and "+periodEndingPrev+" ";

            //tmp4+=" and wp.id_media in("+listMedia+")";
            //tmp4+=" and wp.id_product not in("+listExcludeProduct+")";
            tmp4+=" group by id_media,id_category ";

            #endregion


            sql+="select id_media,"+cumul+","+period+",";
            sql+=" "+TNS.AdExpress.Constantes.DB.Hathor.PDM_FALSE+","+periodId+","+mediaTypeInformation.DatabaseId+"";
            sql+=",id_category,category,media,"+year+",";
            sql+=getUnitsForTotal;
            sql+=" from((select id_media,'USE DB CLASSIFICATION' as media,id_category,'USE DB CLASSIFICATION' as category,";
            sql+=getUnitsForSelectCur;
            sql+=" from "+mediaTypeInformation.DataTable+" where ";
            //sql+=tmp1+tmp2+tmp4;
            sql+=tmp2+tmp4;
            // union
            sql+=" )union (select  id_media,'USE DB CLASSIFICATION'as media, id_category,'USE DB CLASSIFICATION' as category,";
            sql+=getUnitsForSelectPrev;
            sql+=" from "+mediaTypeInformation.DataTable+" where ";
            sql+=tmp3+tmp4;
            sql+=") )group by id_media,media,id_category,category";
            #endregion

            try {
                source.Insert(sql);
            }
            catch(System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert data :"+sql,err));
            }
        }


            #region Insertion des sous-totaux
		/// <summary>
		/// Insertion des Sous totaux dans la table
		/// </summary>
		/// <param name="datePeriode">periode YYYYMM</param>
		/// <param name="connection">Connection Oracle</param>
		/// <param name="vehicle">Vehicle</param>
		public static void InsertSubTotal(string datePeriode,IDataSource source,MediaTypeInformation mediaTypeInformation,bool total,string cumul,string type_tendency){
						
			#region Variables


			string sql=""; 
			string getUnitsForTotal=mediaTypeInformation.GetTotalUnitsInsertSQLFields;
			string getUnit=mediaTypeInformation.GetSubTotalUnitsSQLFields;
			string getUnitInsert=mediaTypeInformation.GetUnitsInsertSQLFields;
            string totalTrendsTableLabel=mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.')+1,mediaTypeInformation.TotalMonthTrendsTable.Length-mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.')-1);
            string totalTrendsTableLabelWithSchema=mediaTypeInformation.TotalMonthTrendsTable;
            string trendsTableLabelWithSchema=mediaTypeInformation.MonthTrendsTable;
			#endregion

			#region Construction de la requête

            sql+=" insert into "+totalTrendsTableLabelWithSchema+" ";
            sql+=" ( id_"+totalTrendsTableLabel+" ,id_pdm";
			sql+=" ,id_cumulative,id_period ";
			sql+=" ,id_type_tendency";
			if(!total){
				sql+=" ,id_category";
			}
			sql+=" ,id_vehicle";
			if(!total){
				sql+=" ,category";
			}
			sql+=" ,year,date_period";
			sql+=getUnitInsert;
			sql+=")";
			if(!total){
				sql+="select date_period||id_pdm||id_cumulative||"+type_tendency+"||id_category||id_vehicle";
			}
			else{
				sql+="select date_period||id_pdm||id_vehicle||id_cumulative||"+type_tendency+"";
			}		

			sql+=" ,id_pdm,id_cumulative,id_period";			
			sql+=","+type_tendency+"";
			if(!total){
				sql+=" ,id_category";
			}
			sql+=" ,id_vehicle";
			if(!total){
				sql+=",category";
			}
			sql+=" ,year,date_period,";
			sql+=getUnitsForTotal; 
			sql+=" from(select ";
			if(!total){
				sql+=" id_category,category, ";
			}
			sql+=" id_pdm,id_period, ";
			sql+="id_cumulative,date_period,id_vehicle,year";
			sql+=getUnit;
            sql+=" from "+trendsTableLabelWithSchema;
			sql+=" where id_vehicle="+mediaTypeInformation.DatabaseId;
			
			if(datePeriode!=TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE){
			sql+=" and date_period="+datePeriode;
			}
			sql+=" and id_pdm="+TNS.AdExpress.Constantes.DB.Hathor.PDM_FALSE;
			sql+=" and id_cumulative="+cumul;
			sql+=" group by "; 
			if(!total){
				sql+="	id_category,category, ";
			}
			sql+="id_pdm,id_cumulative,date_period,id_vehicle,year,id_period";
			sql+=" ) ";
			sql+=" group by ";
			if(!total){
				sql+=" id_category,category,";
			}
			sql+=" id_pdm,id_cumulative,date_period,id_vehicle,year,id_period";

			#endregion

			try {
                source.Insert(sql);
            }
            catch(System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert subtotal data :"+sql,err));
            }
		
		}
		#endregion


        public static void InsertPDM(MediaTypeInformation mediaTypeInformation,IDataSource source,bool total) {
            #region Variables

            string sql="";
            string getFirstSelect=GetFirstSelectPdm(mediaTypeInformation);
            string getSecondSelect=GetSecondSelectPdm(mediaTypeInformation);
            string getThirdSelect=GetThirdSelectPdm(mediaTypeInformation);
            string getUnitForInset=GetUnitsForInsertPdm(mediaTypeInformation);
            string trendsTableLabelWithSchema=mediaTypeInformation.MonthTrendsTable;
            string totalTrendsTableLabelWithSchema=mediaTypeInformation.TotalMonthTrendsTable;
            string totalTrendsTableLabel=mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.')+1,mediaTypeInformation.TotalMonthTrendsTable.Length-mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.')-1);

            #endregion

            #region Construction de la requête

            #region Insert tendency

            if(!total) {
                sql+="insert into "+trendsTableLabelWithSchema+" ( ";
                sql+=" id_media ,";
                sql+="id_cumulative ";
                sql+=",date_period ";
                sql+=",id_pdm ";
                sql+=",id_period";
                sql+=",id_vehicle ";
                sql+=",id_category ";
                sql+=",category ";
                sql+=",media ";
                sql+=" ,year ";
                sql+=getUnitForInset;
                sql+=" ) ";
            }
            #endregion

            #region Insert Total
            if(total) {
                sql+="insert into "+totalTrendsTableLabelWithSchema+" ( ";
                sql+=" id_"+totalTrendsTableLabel+" ";
                sql+=" ,id_type_tendency";
                sql+=" ,id_cumulative ";
                sql+=" ,date_period ";
                sql+=" ,id_pdm ";
                sql+=" ,id_period";
                sql+=" ,id_vehicle ";
                sql+=" ,id_category ";
                sql+=" ,category ";
                sql+=" ,year ";
                sql+=getUnitForInset;
                sql+=" ) ";
            }
            #endregion

            sql+=" select ";
            if(total) {
                sql+=" date_period||0||id_cumulative||id_type_tendency||id_category||id_vehicle ";
                sql+=" ,id_type_tendency,";
            }
            if(!total) {
                sql+=" id_media, ";
            }
            sql+=" id_cumulative ";
            sql+=" ,date_period,"+TNS.AdExpress.Constantes.DB.Hathor.PDM_TRUE+" ";
            sql+=" ,id_period ";
            sql+=" ,id_vehicle, id_category";
            sql+=" ,'USE DB CLASSIFICATION' as category";
            if(!total) {
                sql+=" ,'USE DB CLASSIFICATION' as media ";
            }
            sql+=" ,year";
            sql+=getFirstSelect;
            sql+=" from( ";
            sql+=" select ";
            if(!total) {
                sql+=" id_media ,";
            }

            sql+="id_type_tendency ";
            sql+=",id_cumulative ";
            sql+=" ,date_period,"+TNS.AdExpress.Constantes.DB.Hathor.PDM_TRUE+" ";
            sql+=" ,id_period ";
            sql+=" ,id_vehicle, id_category";
            sql+=" ,category";
            if(!total) {
                sql+=" ,media ";
            }
            sql+=" ,year";
            sql+=getSecondSelect;
            sql+=" from( ";

            sql+=" select ";
            if(!total) {
                sql+=""+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_media,";
            }

            sql+=" "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_cumulative ";

            if(total) {
                sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_type_tendency ";
            }
            else {
                sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".id_type_tendency ";
            }
            sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_vehicle,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_category";
            sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_period ";
            sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".category ";
            sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".year";
            sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".date_period ";
            if(!total) {
                sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".media ";
            }
            sql+=getThirdSelect;

            sql+=" from ";

            sql+=" "+totalTrendsTableLabelWithSchema+" "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+" ";
            if(!total) {
                sql+=","+trendsTableLabelWithSchema+" "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+" ";
            }
            else {
                sql+=","+totalTrendsTableLabelWithSchema+" "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+" ";
            }
            sql+=" where "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".id_type_tendency=0 ";
            sql+=" and "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".id_pdm=10 ";
            sql+=" and "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_vehicle="+mediaTypeInformation.DatabaseId;
            sql+=" and "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".id_vehicle="+mediaTypeInformation.DatabaseId;
            sql+=" and "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".date_period="+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".date_period ";
            sql+=" and "+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+".id_cumulative="+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+".id_cumulative ";
            sql+=" ) ";
            sql+=" ) ";

            #endregion

            try {
                source.Insert(sql);
            }
            catch(System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert PDM data :"+sql,err));
            }
        }

        #region Pdm
        /// <summary>
        /// Génére le code sql pour le select pour le calcul du pdm
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns></returns>
        internal static string GetFirstSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql="";
            int lenghtUnit=0;
            #endregion

            for(int i=0;i<mediaTypeInformation.ListCurrentUnit.Count;i++) {
                lenghtUnit=mediaTypeInformation.ListCurrentUnit[i].Length-4;
                sql+=","+mediaTypeInformation.ListCurrentUnit[i];
                sql+=","+mediaTypeInformation.ListPreviousUnit[i];
                sql+=","+" decode("+mediaTypeInformation.ListPreviousUnit[i]+",0,100,(("+mediaTypeInformation.ListCurrentUnit[i]+"-"+mediaTypeInformation.ListPreviousUnit[i]+")/"+mediaTypeInformation.ListPreviousUnit[i]+")*100) as "+mediaTypeInformation.ListCurrentUnit[i].Substring(0,lenghtUnit)+"_evol";
            }

            return sql;

        }

        /// <summary>
        /// Génère le code sql pour le deuxième select pour le pdm
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <returns></returns>
        internal static string GetSecondSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql="";
            #endregion

            for(int i=0;i<mediaTypeInformation.ListCurrentUnit.Count;i++) {
                sql+=", decode(total_"+mediaTypeInformation.ListCurrentUnit[i]+",0,100,"+mediaTypeInformation.ListCurrentUnit[i]+"/total_"+mediaTypeInformation.ListCurrentUnit[i]+"*100) as "+mediaTypeInformation.ListCurrentUnit[i]+"";
                sql+=", decode(total_"+mediaTypeInformation.ListPreviousUnit[i]+",0,100,"+mediaTypeInformation.ListPreviousUnit[i]+"/total_"+mediaTypeInformation.ListPreviousUnit[i]+"*100) as "+mediaTypeInformation.ListPreviousUnit[i]+"";
            }

            return sql;
        }

        /// <summary>
        /// Génère le code sql 
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <returns></returns>
        internal static string GetThirdSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql="";
            #endregion


            for(int i=0;i<mediaTypeInformation.ListCurrentUnit.Count;i++) {
                sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+"."+mediaTypeInformation.ListCurrentUnit[i]+" as total_"+mediaTypeInformation.ListCurrentUnit[i]+" ";
                sql+=" ,"+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+"."+mediaTypeInformation.ListCurrentUnit[i]+" as "+mediaTypeInformation.ListCurrentUnit[i]+" ";
                sql+=","+TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE+"."+mediaTypeInformation.ListPreviousUnit[i]+" as total_"+mediaTypeInformation.ListPreviousUnit[i]+" ";
                sql+=","+TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE+"."+mediaTypeInformation.ListPreviousUnit[i]+" as "+mediaTypeInformation.ListPreviousUnit[i]+" ";
            }

            return sql;

        }

        /// <summary>
        /// Génère le code sql pour l'insert
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns></returns>
        internal static string GetUnitsForInsertPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql="";
            int lenghtUnit=0;
            #endregion

            for(int i=0;i<mediaTypeInformation.ListCurrentUnit.Count;i++) {
                lenghtUnit=mediaTypeInformation.ListCurrentUnit[i].Length-4;
                sql+=","+mediaTypeInformation.ListCurrentUnit[i];
                sql+=","+mediaTypeInformation.ListPreviousUnit[i];
                sql+=","+mediaTypeInformation.ListCurrentUnit[i].Substring(0,lenghtUnit)+"_evol";
            }

            return sql;
        }
        #endregion
    }
}
