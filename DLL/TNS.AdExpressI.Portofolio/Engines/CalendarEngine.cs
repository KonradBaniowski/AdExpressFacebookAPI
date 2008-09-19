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
			Int64 iCurLine = 0;
			int iNbLine = 0;
			int iNbLevels = 0;
			ArrayList parutions = new ArrayList();

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
			tab[iCurLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, iCurLine, _webSession);
			tab[iCurLine, 2] = new CellMediaScheduleLink(cellLevels[0], _webSession);
            
            //// totaux
            //if(!_webSession.Percentage) tab[iCurLine, 3] = cellFactory.Get(0.0);
            //else tab[iCurLine, 3] = new CellPDM(0.0, null);
            //// pourcentage
            //tab[iCurLine, 4] = new CellPercent(0.0, null);
            //// initialisation des colonnes
            //for(i = 5; i < 5 + parutions.Count; i++) {
            //    if(!_webSession.Percentage) tab[iCurLine, i] = cellFactory.Get(0.0);
            //    else tab[iCurLine, i] = new CellPDM(0.0, (CellUnit)tab[iCurLine, 3]);
            //}

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
						tab[iCurLine, 1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, _webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i - 1], i, iCurLine, _webSession);
						if (row.Table.Columns.Contains("id_address") && row["id_address"] != System.DBNull.Value) {
							cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
						}
						level = _webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(i);
						//PM
						if (level != DetailLevelItemInformation.Levels.agency && level != DetailLevelItemInformation.Levels.groupMediaAgency) {
							tab[iCurLine, 2] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession);
						}
						else {
							tab[iCurLine, 2] = new CellEmpty();
						}

                        ////total
                        //if (!_webSession.Percentage) tab[iCurLine, 3] = cellFactory.Get(0.0);
                        //else tab[iCurLine, 3] = new CellPDM(0.0, null);
                        ////pourcentage
                        //tab[iCurLine, 4] = new CellPercent(0.0, (CellUnit)tab[cellLevels[i - 1].LineIndexInResultTable, 4]);
                        ////initialisation des autres colonnes
                        //for (int j = 5; j < 5 + parutions.Count; j++) {
                        //    if (!_webSession.Percentage) tab[iCurLine, j] = cellFactory.Get(0.0);
                        //    else tab[iCurLine, j] = new CellPDM(0.0, (CellUnit)tab[iCurLine, 3]);
                        //}

                        initLine(tab, iCurLine, cellFactory, cellLevels[i-1]);
					}

				}
                ////feuille ou niveau parent?
                //lCol = tab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                //valu = Convert.ToDouble(row[_webSession.GetSelectedUnit().Id.ToString()]);
                //tab.AffectValueAndAddToHierarchy(1, iCurLine, lCol, valu);
                //tab.AffectValueAndAddToHierarchy(1, iCurLine, 4, valu);
                //tab.AffectValueAndAddToHierarchy(1, iCurLine, 3, valu);

                setLine(tab, iCurLine, row);
			}
			#endregion

			return tab;
		}
		#endregion

        #region InitLine
        protected delegate void InitLine(ResultTable oTab, Int64 cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent);
        protected void InitDoubleLine(ResultTable oTab, Int64 cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent) {
            //total
            if(!_webSession.Percentage) oTab[cLine, 3] = cellFactory.Get(0.0);
            else oTab[cLine, 3] = new CellPDM(0.0, null);

            //pourcentage
            if(parent == null) oTab[cLine, 4] = new CellPercent(0.0, null);
            else oTab[cLine, 4] = new CellPercent(0.0, (CellUnit)oTab[parent.LineIndexInResultTable, 4]);

            //initialisation des autres colonnes
            for(int j = 5; j < oTab.DataColumnsNumber + 1; j++) {
                if(!_webSession.Percentage) oTab[cLine, j] = cellFactory.Get(0.0);
                else oTab[cLine, j] = new CellPDM(0.0, (CellUnit)oTab[cLine, 3]);
            }
        }
        protected void InitListLine(ResultTable oTab, Int64 cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent) {
            //total
            if(!_webSession.Percentage) oTab[cLine, 3] = cellFactory.Get(0.0);
            else oTab[cLine, 3] = new CellVersionNbPDM(null);

            //pourcentage
            if(parent == null) oTab[cLine, 4] = new CellVersionNbPDM(null);
            else oTab[cLine, 4] = new CellVersionNbPDM((CellIdsNumber)oTab[parent.LineIndexInResultTable, 3]);

            //initialisation des autres colonnes
            for(int j = 5; j < oTab.DataColumnsNumber + 1; j++) {
                if(!_webSession.Percentage) oTab[cLine, j] = cellFactory.Get(0.0);
                else oTab[cLine, j] = new CellVersionNbPDM((CellIdsNumber)oTab[cLine, 3]);
            }
        }
        #endregion

        #region SetLine
        protected delegate void SetLine(ResultTable oTab, Int64 iLineIndex, DataRow dr);
        protected void SetDoubleLine(ResultTable oTab, Int64 cLine, DataRow row) {
            if(row != null) {
                long lCol = oTab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                double valu = Convert.ToDouble(row[_webSession.GetSelectedUnit().Id.ToString()]);
                //Affect value
                oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, valu);
                oTab.AffectValueAndAddToHierarchy(1, cLine, 4, valu);
                oTab.AffectValueAndAddToHierarchy(1, cLine, 3, valu);
            }
        }
        protected void SetListLine(ResultTable oTab, Int64 cLine, DataRow row) {
            if(row != null) {
                long lCol = oTab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                //Get values
                string[] tIds = row[_webSession.GetSelectedUnit().Id.ToString()].ToString().Split(',');
                //Affect value
                for(int i = 0; i < tIds.Length; i++) {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 4, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 3, Convert.ToInt64(tIds[i]));
                }
            }
        }
        #endregion

        /// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			throw new PortofolioException("The method or operation is not implemented.");
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
		protected virtual int GetCalendarSize(DataTable dt, ArrayList parutions) {

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
				if (!parutions.Contains(dr["date_media_num"])) {
					parutions.Add(dr["date_media_num"]);
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
		protected virtual void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, ArrayList parutions) {
			headers = new Headers();
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
			headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage), TOTAL_COL));
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

			//une colonne par date de parution
			parutions.Sort();
			foreach (Int32 parution in parutions) {
				headers.Root.Add(new Header(true, DateString.YYYYMMDDToDD_MM_YYYY(parution.ToString(), _webSession.SiteLanguage), (long)parution));
			}
			if (!_webSession.Percentage) {
                cellFactory = _webSession.GetCellUnitFactory();
			}
			else {
				cellFactory = new CellUnitFactory(new CellPDM(0.0));
			}
		}
		#endregion

	}
}
