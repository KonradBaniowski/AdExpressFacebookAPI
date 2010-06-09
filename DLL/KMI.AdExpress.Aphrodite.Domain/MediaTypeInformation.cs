#region Information
//Auteur : A.Obermeyer
// Date de création : 22/03/05
// Date de modification
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace KMI.AdExpress.Aphrodite.Domain {

	/// <summary>
	/// classe vehicle
	/// </summary>
	public class MediaTypeInformation{

		#region Variables
		/// <summary>
		/// Source table name
		/// </summary>
		protected string _dataTable="";
		/// <summary>
		/// Month Trends table
		/// </summary>
		protected string _monthTrendsTable="";
        /// <summary>
        /// Week Trends table
        /// </summary>
        protected string _weekTrendsTable="";
		/// <summary>
        /// Month Trends Total table
		/// </summary>
		protected string _totalMonthTrendsTable="";
        /// <summary>
        /// Week Trends Total table
        /// </summary>
        protected string _totalWeekTrendsTable="";
		/// <summary>
		/// Liste des unites de l'année courante
		/// </summary>
        protected List<string> _listCurrentUnit=new List<string>();
		/// <summary>
		/// Liste des unites de l'année précédentes
		/// </summary>
        protected List<string> _listPreviousUnit=new List<string>();
		/// <summary>
		/// Liste des unites dans les tables data
		/// </summary>
        protected List<string> _listDataUnit=new List<string>();
		/// <summary>
		/// Liste des période
		/// </summary>	
		protected ArrayList _listPeriod=new ArrayList();
		/// <summary>
		/// Type de vehicle
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.DB.Vehicles.names _vehicleId;
        /// <summary>
        /// dataBase Id
        /// </summary>
        protected Int64 _databaseId;
		/// <summary>
		/// Identifiant de la Liste des media à traiter
		/// </summary>
		protected Int64 _mediaListId=0;
		/// <summary>
		/// Liste des media à traiter
		/// </summary>
		protected string _mediaList=String.Empty; 

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaTypeInformation(){		
		}

		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient/définit la table avec les données désagrégées
		/// </summary>
		public string DataTable{
			get{return _dataTable;}
			set{_dataTable=value;}
		}
		
		/// <summary>
		/// Get/Set Month trends table
		/// </summary>
        public string MonthTrendsTable {
			get{return _monthTrendsTable;}
            set { _monthTrendsTable=value; }
		}

        /// <summary>
        /// Get/Set Week trends table
        /// </summary>
        public string WeekTrendsTable {
            get { return _weekTrendsTable; }
            set { _weekTrendsTable=value; }
        }

		/// <summary>
        /// Get/Set Total Month trends table
		/// </summary>
		public string TotalMonthTrendsTable{
			get{return _totalMonthTrendsTable;}
            set { _totalMonthTrendsTable=value; }
		}

        /// <summary>
        /// Get/Set Total Week trends table
        /// </summary>
        public string TotalWeekTrendsTable {
            get { return _totalWeekTrendsTable; }
            set { _totalWeekTrendsTable=value; }
        }

		/// <summary>
		/// Obtient/définit la liste des unités de l'année courante à étudier
		/// </summary>
        public List<string> ListCurrentUnit {
			get{return _listCurrentUnit;}
			set{_listCurrentUnit=value;}
		}

        /// <summary>
        /// Get  Units list for current year select
        /// </summary>
        public string GetUnitsSelectSQLForCurrentYear {
            get {

                #region Variables
                string sql="";
                #endregion

                if(_listDataUnit.Count<1) throw (new System.Exception("Data Units are not defined"));
                if(_listPreviousUnit.Count<1) throw (new System.Exception("Previous Units are not defined"));

                for(int i=0;i<_listDataUnit.Count;i++) {
                    sql+="sum("+_listDataUnit[i].ToString()+") as "+_listCurrentUnit[i].ToString()+",";
                }

                foreach(string prevUnit in _listPreviousUnit) sql+=" 0 as "+prevUnit+",";
                sql=sql.Substring(0,sql.Length-1);
               

                return sql;
            
            }
        }

        /// <summary>
        /// Get  Units list for current year select
        /// </summary>
        public string GetUnitsSelectSQLForPreviousYear {
            get {

                #region Variables
                string sql="";
                #endregion

                if(_listDataUnit.Count<1) throw (new System.Exception("Data Units are not defined"));
                if(_listPreviousUnit.Count<1) throw (new System.Exception("Previous Units are not defined"));

                foreach(string currentUnit in _listCurrentUnit) sql+=" 0 as "+currentUnit+",";

                for(int i=0;i<_listDataUnit.Count;i++) {
                    sql+="sum("+_listDataUnit[i].ToString()+") as "+_listPreviousUnit[i].ToString()+",";
                }

                sql=sql.Substring(0,sql.Length-1);


                return sql;

            }
        }


        /// <summary>
        /// Get SQL Units list for fields
        /// </summary>
        public string GetUnitsInsertSQLFields {
            get {
                string sql="";
                int lenghtUnit=0;
                if(_listCurrentUnit.Count<1) throw (new System.Exception("Current Units are not defined"));
                if(_listPreviousUnit.Count<1) throw (new System.Exception("Previous Units are not defined"));
                foreach(string unit in _listCurrentUnit) {
                    sql+=","+unit;
                }
                foreach(string unit in _listPreviousUnit) {
                    sql+=","+unit;
                }
                foreach(string unit in _listCurrentUnit) {
                    lenghtUnit=unit.Length-4;
                    sql+=","+unit.Substring(0,lenghtUnit)+"_evol";
                }

                return sql;
            }
        }

        /// <summary>
        /// Get SQL unit fields for subtotals
        /// </summary>
        public string GetSubTotalUnitsSQLFields {
            get {
                #region Variables
                string sql="";
                #endregion
                if(_listCurrentUnit.Count<1) throw (new System.Exception("Current Units are not defined"));
                if(_listPreviousUnit.Count<1) throw (new System.Exception("Previous Units are not defined"));
                for(int i=0;i<_listCurrentUnit.Count;i++) {
                    sql+=",sum("+_listCurrentUnit[i]+") as "+_listCurrentUnit[i]+"";
                    sql+=",sum("+_listPreviousUnit[i]+") as "+_listPreviousUnit[i]+"";
                }
                return sql;
            }
        
        }

        /// <summary>
        /// Get SQL Units list for total select
        /// </summary>
        public string GetTotalUnitsInsertSQLFields{
            get {
                #region Variables
                string sql="";
                int lenghtUnit=0;
                #endregion

                if(_listCurrentUnit.Count<1) throw (new System.Exception("Current Units are not defined"));
                if(_listPreviousUnit.Count<1) throw (new System.Exception("Previous Units are not defined"));

                foreach(string unit in _listCurrentUnit) {
                    sql+="sum("+unit+") as "+unit+",";
                }
                foreach(string unit in _listPreviousUnit) {
                    sql+="sum("+unit+") as "+unit+",";
                }

                for(int i=0;i<_listCurrentUnit.Count;i++) {
                    lenghtUnit=_listCurrentUnit[i].ToString().Length-4;
                    if(i<_listCurrentUnit.Count-1)
                        sql+=" decode(sum("+_listPreviousUnit[i].ToString()+"),0,100,(sum("+_listCurrentUnit[i].ToString()+")-sum("+_listPreviousUnit[i].ToString()+"))/sum("+_listPreviousUnit[i].ToString()+")*100) as "+_listCurrentUnit[i].ToString().Substring(0,lenghtUnit)+"_evol,";
                    else
                        sql+=" decode(sum("+_listPreviousUnit[i].ToString()+"),0,100,(sum("+_listCurrentUnit[i].ToString()+")-sum("+_listPreviousUnit[i].ToString()+"))/sum("+_listPreviousUnit[i].ToString()+")*100) as "+_listCurrentUnit[i].ToString().Substring(0,lenghtUnit)+"_evol";
                }
                return sql;
            
            }
        }

		/// <summary>
		/// Obtient/définit la liste des unités à étudier
		/// </summary>
        public List<string> ListPreviousUnit {
			get{return _listPreviousUnit;}
			set{_listPreviousUnit=value;}
		}


		/// <summary>
		/// Obtient/définit la liste des unités à étudier dans la table data (presse,radio,télé)
		/// </summary>
        public List<string> ListDataUnit {
			get{return _listDataUnit;}
			set{_listDataUnit=value;}
		}
		
		/// <summary>
		/// Obtient/définit la liste des périodes
		/// </summary>
		public ArrayList ListPeriod{
			get{return _listPeriod;}
			set{_listPeriod=value;}
		}
		
		/// <summary>
		/// Obtient/définit le type de vehicle
		/// </summary>
        public TNS.AdExpress.Constantes.Classification.DB.Vehicles.names VehicleId {
            get { return _vehicleId; }
            set { _vehicleId=value; }
        }

        /// <summary>
        /// Get/Set Database media type Id
        /// </summary>
        public Int64 DatabaseId {
            get { return _databaseId; }
            set { _databaseId=value; }
        }

		/// <summary>
		/// Obtient/définit l'identifiant de la liste des media à traiter
		/// </summary>
		public Int64 MediaListId{
			get{return _mediaListId;}
			set{_mediaListId=value;}
		}

		/// <summary>
		/// Obtient la liste des media à traiter
		/// </summary>
        //public string MediaList{
        //    get{
        //        if(_mediaList==string.Empty)
        //            _mediaList=TNS.Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(int.Parse (_mediaListId.ToString()),33).GetLevelIds(TNS.Baal.ExtractList.Constantes.Levels.media);
        //        return(_mediaList);
        //    }
        //}
		#endregion

	}
}
