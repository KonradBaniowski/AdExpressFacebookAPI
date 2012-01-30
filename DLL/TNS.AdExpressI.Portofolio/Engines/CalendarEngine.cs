#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Result;
namespace TNS.AdExpressI.Portofolio.Engines {

	/// <summary>
	/// Compute portofolio calendar of advertising activity' results
	/// </summary>
	public class CalendarEngine : Engine {

		#region Constantes
		protected const int PROD_COL = 1164;
		protected const int PM_COL = 751;		
		protected const int TOTAL_COL = 1401;
		protected const int POURCENTAGE_COL = 1236;
        protected const int PRODUCT = 1;
        protected const int MEDIA_SCHEDULE = 2;
        protected const int TOTAL = 2;
        protected const int PERCENTAGE = 3;
        protected const int DATA = 4;
		#endregion

        #region Enum
        /// <summary>
        /// Column name : Product, media schedule, total ...
        /// </summary>
        protected enum ColumnName { 
            /// <summary>
            /// Product colomn
            /// </summary>
            product,
            /// <summary>
            /// Media schedule link column
            /// </summary>
            mediaSchedule,
            /// <summary>
            /// Total column
            /// </summary>
            total,
            /// <summary>
            /// Percentage
            /// </summary>
            percentage,
            /// <summary>
            /// data
            /// </summary>
            data
        }
        #endregion

        #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		#endregion

		#region Abstract methods implementation

		#region calendar of advertising activity
		/// <summary>
		/// Get result table of calendar of advertising activity 
		/// </summary>
		/// <returns>Result Table</returns>
		protected override ResultTable ComputeResultTable() {

			#region Variables
			ResultTable tab = null;
			DataSet ds = null;
			DataTable dt = null;
			CellUnitFactory cellFactory = null;
			AdExpressCellLevel[] cellLevels;
			LineType[] lineTypes = new LineType[5] { LineType.total, LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
			Headers headers = null;
            int iCurLine = 0;
			int iNbLine = 0;
			int iNbLevels = 0;
            List<Int32> parutions = new List<Int32>();

            InitLine initLine = null;
            SetLine setLine = null;
			#endregion

			#region Chargement des données
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = portofolioDAL.GetData();//GetDataCalendar

			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) dt = ds.Tables[0];
			else return (tab);
			#endregion

			#region Nombre de lignes du tableau du tableau
			iNbLine = GetCalendarSize(dt, parutions);
			#endregion

			#region Headers
			GetCalendarHeaders(out headers, out cellFactory, parutions);
			#endregion

			#region Initialisation du tableau de résultats
			tab = new ResultTable(iNbLine, headers);
			#endregion

            #region Initialisation du type de ligne
            if(_webSession.GetSelectedUnit().Id == WebCst.CustomerSessions.Unit.versionNb) {
                initLine = new InitLine(InitListLine);
                setLine = new SetLine(SetListLine);
            }
            else {
                initLine = new InitLine(InitDoubleLine);
                setLine = new SetLine(SetDoubleLine);
            } 
            #endregion

            #region Traitement du tableau de résultats
            int i = 1;

			#region Intialisation des totaux
			iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
			cellLevels = new AdExpressCellLevel[iNbLevels + 1];
			tab.AddNewLine(LineType.total);
			tab[iCurLine, GetColumnIndex(ColumnName.product)] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, iCurLine, _webSession);
            /* If the customer don't have the right to media schedule module, we don't show the MS column
             * */
            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
			    tab[iCurLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink(cellLevels[0], _webSession);                      

            initLine(tab, iCurLine, cellFactory, cellLevels[0]);
			#endregion

			i = 1;
			long dCurLevel = 0;
			DetailLevelItemInformation.Levels level;
			foreach (DataRow row in dt.Rows) {
				//pour chaque niveau
				for (i = 1; i <= iNbLevels; i++) {
					//nouveau niveau i
					dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row, i);
					if (dCurLevel >= 0 && (cellLevels[i] == null || dCurLevel != cellLevels[i].Id)) {
						for (int j = i + 1; j < cellLevels.Length; j++) {
							cellLevels[j] = null;
						}
						iCurLine = tab.AddNewLine(lineTypes[i]);
						tab[iCurLine, GetColumnIndex(ColumnName.product)] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, _webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i - 1], i, iCurLine, _webSession);
						if (row.Table.Columns.Contains("id_address") && row["id_address"] != System.DBNull.Value) {
							cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
						}
						level = _webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(i);
						//PM
                        //if (level != DetailLevelItemInformation.Levels.agency && level != DetailLevelItemInformation.Levels.groupMediaAgency) {
                        /* If the customer don't have the right to media schedule module, we don't show the MS link column
                        * */
                        if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
							tab[iCurLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession);
                        //}
                        //else {
                        //    tab[iCurLine, 2] = new CellEmpty();
                        //}                      

                        initLine(tab, iCurLine, cellFactory, cellLevels[i-1]);
					}

				}               

                setLine(tab, iCurLine, row);
			}
			#endregion

			return tab;
		}
		#endregion

        #region InitLine
        protected delegate void InitLine(ResultTable oTab, int cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent);
        protected virtual void InitDoubleLine(ResultTable oTab, int cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent)
        {
            //total
            if (!_webSession.Percentage) oTab[cLine, GetColumnIndex(ColumnName.total)] = cellFactory.Get(null);
            else
            {
                oTab[cLine, GetColumnIndex(ColumnName.total)] = GetCellPDM(null);
            }

            //pourcentage
            if (parent == null) oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellPercent(null, null);
            else oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellPercent(null, (CellUnit)oTab[parent.LineIndexInResultTable, GetColumnIndex(ColumnName.percentage)]);

            ((CellPercent)oTab[cLine, GetColumnIndex(ColumnName.percentage)]).StringFormat = "{0:percentWOSign}";

            //initialisation des autres colonnes
            for (int j = GetColumnIndex(ColumnName.data); j < oTab.DataColumnsNumber + 1; j++) {
                if (!_webSession.Percentage) oTab[cLine, j] = cellFactory.Get(null);
                else {
                    oTab[cLine, j] = GetCellPDM((CellUnit)oTab[cLine, GetColumnIndex(ColumnName.total)]);
                }
            }
        }
        protected virtual void InitListLine(ResultTable oTab, int cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent)
        {
            //total
            if (!_webSession.Percentage) oTab[cLine, GetColumnIndex(ColumnName.total)] = cellFactory.Get(null);
            else {
                oTab[cLine, GetColumnIndex(ColumnName.total)] = new CellVersionNbPDM(null);
                ((CellVersionNbPDM)oTab[cLine, GetColumnIndex(ColumnName.total)]).StringFormat = "{0:percentWOSign}";
            }

            //pourcentage
            if (parent == null) oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellVersionNbPDM(null);
            else oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellVersionNbPDM((CellIdsNumber)oTab[parent.LineIndexInResultTable, GetColumnIndex(ColumnName.total)]);

            ((CellVersionNbPDM)oTab[cLine, GetColumnIndex(ColumnName.percentage)]).StringFormat = "{0:percentWOSign}";

            //initialisation des autres colonnes
            for (int j = GetColumnIndex(ColumnName.data); j < oTab.DataColumnsNumber + 1; j++) {
                if (!_webSession.Percentage) oTab[cLine, j] = cellFactory.Get(null);
                else {
                    oTab[cLine, j] = new CellVersionNbPDM((CellIdsNumber)oTab[cLine, GetColumnIndex(ColumnName.total)]);
                    ((CellVersionNbPDM)oTab[cLine, j]).StringFormat = "{0:percentWOSign}";
                }
            }
        }
        #endregion

        #region SetLine
        protected delegate void SetLine(ResultTable oTab, int iLineIndex, DataRow dr);
        protected virtual void SetDoubleLine(ResultTable oTab, int cLine, DataRow row)
        {
            if(row != null) {
                int lCol = oTab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                double valu = Convert.ToDouble(row[_webSession.GetSelectedUnit().Id.ToString()]);
                //Affect value
                oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, valu);
                oTab.AffectValueAndAddToHierarchy(1, cLine, GetColumnIndex(ColumnName.percentage), valu);
                oTab.AffectValueAndAddToHierarchy(1, cLine, GetColumnIndex(ColumnName.total), valu);
            }
        }
        protected virtual void SetListLine(ResultTable oTab, int cLine, DataRow row)
        {
            if(row != null) {
                int lCol = oTab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                //Get values
                string[] tIds = row[_webSession.GetSelectedUnit().Id.ToString()].ToString().Split(',');
                //Affect value
                for(int i = 0; i < tIds.Length; i++) {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, GetColumnIndex(ColumnName.percentage), Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, GetColumnIndex(ColumnName.total), Convert.ToInt64(tIds[i]));
                }
            }
        }
        #endregion

        #region BuildHtmlResult
        /// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			throw new PortofolioException("The method or operation is not implemented.");
		}
		#endregion

        #endregion

        #region GetCellPDM
        /// <summary>
        /// Get Cell PDM
        /// </summary>
        /// <param name="unitValue">Value</param>
        /// <param name="reference">Reference Value</param>
        /// <returns>Cell PDM object</returns>
        protected virtual CellUnit GetCellPDM(CellUnit reference) {
            CellPDM cellPdm = new CellPDM(null, reference);
            cellPdm.StringFormat = "{0:percentWOSign}";
            return cellPdm;
        }
        #endregion

        #region GetCalendarSize
        /// <summary>
		/// Calcul la taille du tableau de résultats d'un calendrier d'action
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt">table de données</param>
		/// <returns>nombre de ligne du tableau de résultats</returns>
		/// <param name="parutions">Parutions</param>
        protected virtual int GetCalendarSize(DataTable dt, List<Int32> parutions) {

			#region Variable
			Int64 OldL1Id = -1;
			Int64 cL1Id = 0;
			Int64 nbL1Id = 0;
			Int64 OldL2Id = -1;
			Int64 cL2Id = 0;
			Int64 nbL2Id = 0;
			Int64 OldL3Id = -1;
			Int64 cL3Id = 0;
			Int64 nbL3Id = 0;
			Int64 nbLine = 0;
			#endregion

			foreach (DataRow dr in dt.Rows) {
				cL1Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 1);
				if (cL1Id >= 0 && cL1Id != OldL1Id) {
					nbL1Id++;
					OldL1Id = cL1Id;
					OldL2Id = OldL3Id = -1;
				}
				cL2Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 2);
				if (cL2Id >= 0 && OldL2Id != cL2Id) {
					nbL2Id++;
					OldL2Id = cL2Id;
					OldL3Id = -1;
				}
				cL3Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 3);
				if (cL3Id >= 0 && OldL3Id != cL3Id) {
					nbL3Id++;
					OldL3Id = cL3Id;
				}
                Int32 dateMediaNum = Int32.Parse(dr["date_media_num"].ToString());
                if (!parutions.Contains(dateMediaNum)) {
                    parutions.Add(dateMediaNum);
				}
			}

			if ((nbL1Id > 0) || (nbL2Id > 0) || (nbL3Id > 0)) {
				nbLine = nbL1Id + nbL2Id + nbL3Id + 1;
			}
			return (int)nbLine;
		}
		#endregion

		#region Calendar headers
		/// <summary>
		/// Calendar Headers and Cell factory
		/// </summary>
		/// <returns></returns>
        protected virtual void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, List<Int32> parutions) {
            headers = new Headers();
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
            /* If the customer don't have the right to media schedule module, we don't show the MS column
             * */
            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
			    headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage), TOTAL_COL));
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

			//une colonne par date de parution
			parutions.Sort();
			foreach (Int32 parution in parutions) {
				headers.Root.Add(new Header(true, Dates.YYYYMMDDToDD_MM_YYYY(parution.ToString(), _webSession.SiteLanguage), (long)parution));
			}
			if (!_webSession.Percentage) {
                cellFactory = _webSession.GetCellUnitFactory();
			}
			else {

                cellFactory = new CellUnitFactory(GetCellPDM(null));
			}
		}
		#endregion

        #region GetColumnIndex
        /// <summary>
        /// Get the column index depending on the column name
        /// <remarks>This method is used to get dynamically the index of a column and to treat the case when we don't have the right for the media schedule module</remarks>
        /// </summary>
        /// <param name="columnName">Column name (product, meddia schedule link ...)</param>
        /// <returns>Column index</returns>
        protected int GetColumnIndex(ColumnName columnName) {

            int showMediaSchedule = 0;

            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                showMediaSchedule = 1;

            switch(columnName){
                case ColumnName.product:
                    return PRODUCT;
                case ColumnName.mediaSchedule:
                    return MEDIA_SCHEDULE;
                case ColumnName.total:
                    return TOTAL + showMediaSchedule; 
                case ColumnName.percentage:
                    return PERCENTAGE + showMediaSchedule;
                case ColumnName.data:
                    return DATA + showMediaSchedule;
                default:
                    throw new PortofolioException("The column name does not exist.");
            }

        }
        #endregion

    }
}
