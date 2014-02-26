#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Reflection;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute portofolio detail's results
	/// </summary>
	public class PortofolioDetailEngine : Engine {
		
		#region Constantes
		protected const int PROD_COL = 1164;
		protected const int CREATIVES_COL = 1994;
		protected const int PM_COL = 751;
		protected const int PAN_COL = 1604;
        protected const int INSERTIONS_LIST_COL = 2245;
        #endregion

        #region Variables
        /// <summary>
		/// Define if show creatives
		/// </summary>
		protected bool _showCreatives = false;
		/// <summary>
		/// Define if show insertions
		/// </summary>
		protected bool _showInsertions = false;

        /// <summary>
        /// Define if show media schedule Link
        /// </summary>
        protected bool _showMediaSchedule = false;

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
		public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool showInsertions, bool showCreatives)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
			_showCreatives = showCreatives;
			_showInsertions = showInsertions;
            _showMediaSchedule = webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)!=null ? true : false;
		}

		#endregion

		#region Protected methods

		#region Abstract methods implementation

		#region ComputeResultTable
		/// <summary>
		/// Get Result Table for portofolio detail
		/// </summary>
		/// <returns>ResultTable</returns>
		protected override ResultTable ComputeResultTable() {			

			#region Variables
			ResultTable tab = null;
			DataTable dt = null;
			Headers headers = null;
			CellUnitFactory[] cellFactories = null;
            AffectLine[] lineDelegates = null;
            AdExpressCellLevel[] cellLevels;
			LineType[] lineTypes = new LineType[5] { LineType.total, LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
			string[] columnsName = null;
			int iCurLine = 0;
			int iNbLine = 0;
			int iNbLevels = 0;
            int insertions = 0, creatives = 0;
			#endregion

			// Get Data
			dt = GetDataForResultTable().Tables[0];
			if (dt == null || dt.Rows.Count == 0) {
				return null;
			}
			// Table nb lines
			iNbLine = GetPortofolioSize(dt);

			#region Initialisation du tableau de résultats
			if (_showInsertions) insertions = 1;
			if (_showCreatives) creatives = 1;
          
            GetPortofolioHeaders(out headers, out cellFactories, out lineDelegates, out columnsName);
			tab = new ResultTable(iNbLine, headers);
			#endregion

			#region Traitement du tableau de résultats

			#region Intialisation des totaux
			iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
			cellLevels = new AdExpressCellLevel[iNbLevels + 1];
			tab.AddNewLine(LineType.total);
			tab[iCurLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, iCurLine, _webSession);
			//Creatives
			if (_showCreatives) tab[iCurLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession, _webSession.GenericProductDetailLevel);
			if (_showInsertions) tab[iCurLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession, _webSession.GenericProductDetailLevel);
            if (_showMediaSchedule) tab[iCurLine, 2 + creatives + insertions] = new CellMediaScheduleLink(cellLevels[0], _webSession);
			
            AffectPortefolioLine(cellFactories, lineDelegates, columnsName, null, tab, iCurLine, false);
			#endregion

			int i = 1;
			long dCurLevel = 0;
			foreach (DataRow row in dt.Rows) {
				//pour chaque niveau
				for (i = 1; i <= iNbLevels; i++) {
					//nouveau niveau i
					dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row, i);
					if (dCurLevel >= 0 && (cellLevels[i] == null || dCurLevel != cellLevels[i].Id)) {
						for (int j = i + 1; j < cellLevels.Length; j++) {
							cellLevels[j] = null;
						}
						iCurLine++;
						tab.AddNewLine(lineTypes[i]);
						tab[iCurLine, 1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, _webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i - 1], i, iCurLine, _webSession);
						if (row.Table.Columns.Contains("id_address") && row["id_address"] != System.DBNull.Value) {
							cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
						}
						//Creatives
						if (creatives > 0) tab[iCurLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession, _webSession.GenericProductDetailLevel);
						if (insertions > 0) tab[iCurLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                        if (_showMediaSchedule) tab[iCurLine, 2 + creatives + insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession);
                        //feuille ou niveau parent?
                        if(i != iNbLevels) {
                            AffectPortefolioLine(cellFactories, lineDelegates, columnsName, null, tab, iCurLine, false);
                        }
                    }
				}
                AffectPortefolioLine(cellFactories, lineDelegates, columnsName, row, tab, iCurLine, true);
            }
			#endregion

			return tab;
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

		#region Affect Portefolio Line
		/// <summary>
		/// Affect Portefolio Line
		/// </summary>
		/// <param name="tCellFactories">Cell Factory</param>
		/// <param name="columnsName">Column names table</param>
		/// <param name="dr">DataRow</param>
		/// <param name="oTab">Result table</param>
		/// <param name="iLineIndex">Line Index</param>
		/// <param name="isLeaf">Is Leaf</param>
		protected void AffectPortefolioLine(CellUnitFactory[] tCellFactories, AffectLine[] tDelegates, string[] columnsName, DataRow dr, ResultTable oTab, int iLineIndex, bool isLeaf) {
			for (int i = 0; i < tCellFactories.Length; i++) {
				if (tCellFactories[i] != null) {
                    tDelegates[i](oTab, iLineIndex, i + 1, tCellFactories[i], (dr!=null)?dr[columnsName[i]]:null, isLeaf);
				}
			}
		}
        protected delegate void AffectLine(ResultTable oTab, int cLine, int cCol, CellUnitFactory cellFactory, object value, bool isLeaf);
        protected virtual void AffectDoubleLine(ResultTable oTab, int cLine, int cCol, CellUnitFactory cellFactory, object value, bool isLeaf) {
            if(oTab[cLine, cCol] == null) {
                oTab[cLine, cCol] = cellFactory.Get(0.0);
            }
            if(value != null) {
                //Affect value
                if(isLeaf) {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, cCol, Convert.ToDouble(value));
                }
            }
        }
        protected virtual void AffectListLine(ResultTable oTab, int cLine, int cCol, CellUnitFactory cellFactory, object value, bool isLeaf) {
            if(oTab[cLine, cCol] == null) {
                oTab[cLine, cCol] = cellFactory.Get(0);
            }
            if (value != null && !string.IsNullOrEmpty(value.ToString()) && isLeaf)
            {
                //Get values
                string[] tIds = value.ToString().Split(',');
                //Affect value
                for(int i = 0; i < tIds.Length; i++) {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, cCol, Convert.ToInt64(tIds[i]));
                }
            }
        }
        #endregion

		#region Portofolio headers
		/// <summary>
		/// Portofolio Headers and Cell factory
		/// </summary>
		/// <returns></returns>
		protected virtual void GetPortofolioHeaders(out Headers headers, out CellUnitFactory[] cellFactories, out AffectLine[] lineDelegates, out string[] columnsName) {
			int insertions = 0;
			int creatives = 0;
            int mediaSchedule = 0;
            int nbColumsProduct = 1;
			int iNbCol = 0;
            int columnIndex = (_showMediaSchedule) ? nbColumsProduct+1 : nbColumsProduct;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
            Type type;
            Cell cellUnit;

			headers = new TNS.FrameWork.WebResultUI.Headers();
			// Product column
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
			// Creatives column     
			if (_showCreatives) {
				headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(CREATIVES_COL, _webSession.SiteLanguage), CREATIVES_COL));
				creatives = 1;
			}
			// Insertions column
			if (_showInsertions) {
				headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(INSERTIONS_LIST_COL, _webSession.SiteLanguage), INSERTIONS_LIST_COL));
				insertions = 1;
			}
            if (_showMediaSchedule)
            {
                // Media schedule column
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
                mediaSchedule = 1;
            }

            iNbCol = nbColumsProduct + creatives + insertions + mediaSchedule + _webSession.GetValidUnitForResult().Count;

            cellFactories = new CellUnitFactory[iNbCol];
            lineDelegates = new AffectLine[iNbCol];
            columnsName = new string[iNbCol];
			cellFactories[0] = null;
			cellFactories[1] = null;
            if (_showCreatives) columnsName[nbColumsProduct + creatives] = null;
            if (_showInsertions) columnsName[nbColumsProduct + creatives + insertions] = null;

            switch (_vehicleInformation.Id) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.indoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.instore:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.cinema:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.czinternet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.mailValo:
                case DBClassificationConstantes.Vehicles.names.mms:

                    foreach (UnitInformation currentUnit in _webSession.GetValidUnitForResult()) {
                        headers.Root.Add(new Header(true, 
                            GestionWeb.GetWebWord(currentUnit.WebTextId, _webSession.SiteLanguage), currentUnit.WebTextId));
                        type = assembly.GetType(currentUnit.CellType);
                        cellUnit = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
						cellUnit.StringFormat = currentUnit.StringFormat;
                        columnsName[columnIndex + creatives + insertions] = currentUnit.Id.ToString();
                        cellFactories[columnIndex + creatives + insertions] = new CellUnitFactory((CellUnit)cellUnit);
                        if(cellUnit is CellIdsNumber) {
                            lineDelegates[columnIndex + creatives + insertions] = AffectListLine;
                        }
                        else {
                            lineDelegates[columnIndex + creatives + insertions] = AffectDoubleLine;
                        }

                        columnIndex++;
                    }
                    break;

                default:
                    throw new PortofolioException("Vehicle unknown.");
            }

		}
		#endregion

        #region GetDataForResultTable
        /// <summary>
		/// Get data for ResultTable result
		/// </summary>
		/// <returns></returns>
		protected virtual DataSet GetDataForResultTable() {
			DataSet ds = null;
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			return portofolioDAL.GetData();			
		}
		#endregion

		#region GetPortofolioSize
		/// <summary>
		/// Get lines number for the portofolio result
		/// </summary>
		/// <param name="dt">Data table</param>
		/// <returns>Lines number</returns>
		protected virtual int GetPortofolioSize(DataTable dt) {

			#region Variables
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

			if (dt != null && dt.Rows.Count > 0) {
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
				}
			}
			if ((nbL1Id > 0) || (nbL2Id > 0) || (nbL3Id > 0)) {
				nbLine = nbL1Id + nbL2Id + nbL3Id + 1;
			}
			return (int)nbLine;
		}
		#endregion

		#endregion
	}
}
