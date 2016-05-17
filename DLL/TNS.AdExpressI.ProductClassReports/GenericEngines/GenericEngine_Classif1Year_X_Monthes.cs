using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines {
	/// <summary>
	/// Implement an engine to build a report presented as GenericEngine_Classif1Year X Monthes
	/// </summary>
	public class GenericEngine_Classif1Year_X_Monthes : GenericEngine {

		#region Attributes
		/// <summary>
		/// Determine if the table must be computed with a monthly cumul
		/// </summary>
		protected bool _plusMonthesCumul;
		#endregion

		#region Accessors
		/// <summary>
		/// Get / Set if the table must be computed with a monthly cumul
		/// </summary>
		public bool PlusMonthesCumul {
			get {
				return _plusMonthesCumul;
			}
			set {
				_plusMonthesCumul = value;
			}
		}
		#endregion

		#region Constants
		protected const Int32 ID_PRODUCT = -1;
        protected const Int32 ID_TOTAL = -2;
        protected const Int32 ID_YEAR_N = -3;
        protected const Int32 ID_YEAR_N1 = -4;
        protected const Int32 ID_EVOL = -5;
        protected const Int32 ID_PDV_N = -6;
        protected const Int32 ID_PDV_N1 = -7;
        protected const Int32 ID_PDM_N = -6;
        protected const Int32 ID_PDM_N1 = -7;
		#endregion

		#region Constructor
		/// <summary>
		/// Defualt constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="result">Report type</param>
		public GenericEngine_Classif1Year_X_Monthes(WebSession session, int result) : base(session, result) { }

		/// <summary>
		/// Defualt constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="result">Report type</param>
		/// <param name="plusMonthesCumul">Determine if should compute total periods</param>
		public GenericEngine_Classif1Year_X_Monthes(WebSession session, int result,bool plusMonthesCumul) : base(session, result) {
			_plusMonthesCumul = plusMonthesCumul;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Compute data
		/// </summary>
		/// <param name="data">data set</param>
		/// <returns>Result table</returns>
		protected override ResultTable ComputeData(DataSet data) {
			ProductClassResultTable tab = null;

			#region Data
			DataTable dtData = data.Tables[0];
			if (dtData.Rows.Count <= 0) return null;
			#endregion

			#region Indexes

			#region Compute indexes : first numerical column, product or media line indexes, dates columns indexes
			int FIRST_DATA_INDEX = 0;
			List<Int32> DATA_CLASSIF_INDEXES = new List<int>();
			for (int i = 0; i < dtData.Columns.Count; i = i + 2) {
				if (dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0 || dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0) {
					DATA_CLASSIF_INDEXES.Add(i);
				}
				else {
					FIRST_DATA_INDEX = i;
					break;
				}
			}
			#endregion

			#region Personal advertisers
			_isPersonalized = 0;
			if (dtData.Columns.Contains("inref")) {
				_isPersonalized = 3;
			}
			#endregion

			//delete useless lines
			CleanDataTable(dtData, FIRST_DATA_INDEX);
			if (dtData.Rows.Count <= 0) return null;

			#region Periods
			DateTime begin = FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];

            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string periodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);
			
            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
			int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
			Int32 DATA_FIRST_MONTH_COLUMN = -1;
			Int32 DATA_LAST_MONTH_COLUMN = -1;
			Int32 DATA_YEAR_N = -1;
			Int32 DATA_YEAR_N1 = -1;
			int firstMonth = Convert.ToInt32(_session.PeriodBeginningDate.Substring(4, 2));
			int lastMonth = Convert.ToInt32(periodEnd.Substring(4, 2));
			Int32 nbMonthes = (lastMonth - firstMonth) + 1;
			DATA_FIRST_MONTH_COLUMN = (_plusMonthesCumul) ? FIRST_DATA_INDEX + 1 : FIRST_DATA_INDEX;
			if (yearN1 > -1) {
				DATA_FIRST_MONTH_COLUMN = (_plusMonthesCumul) ? DATA_FIRST_MONTH_COLUMN + 1 : DATA_FIRST_MONTH_COLUMN;
			}
			DATA_LAST_MONTH_COLUMN = DATA_FIRST_MONTH_COLUMN + (lastMonth - firstMonth);
			if (yearN1 > -1) {
				DATA_LAST_MONTH_COLUMN = DATA_FIRST_MONTH_COLUMN + (nbMonthes * 2 - 1);
			}
			if (_plusMonthesCumul) {
				DATA_YEAR_N = FIRST_DATA_INDEX;
				if (yearN1 > -1) {
					DATA_YEAR_N1 = (yearN1 > -1) ? FIRST_DATA_INDEX + 1 : -1;
				}
			}
			Int32 RES_YEAR_N_OFFSET = 1;
			List<Int64> keyYearN = new List<Int64>(); keyYearN.Add(ID_YEAR_N);
			Int32 RES_YEAR_N1_OFFSET = (yearN1 > 0) ? 2 : -1;
			List<Int64> keyYearN1 = new List<Int64>(); keyYearN1.Add(ID_YEAR_N1);
			Int32 RES_EVOL_OFFSET = (_session.Evolution && RES_YEAR_N1_OFFSET > 0) ? RES_YEAR_N1_OFFSET + 1 : -1;
			List<Int64> keyEvol = new List<Int64>(); keyEvol.Add(ID_EVOL);
			Int32 RES_PDV_N_OFFSET = -1;
			List<Int64> keyPdvYearN = new List<Int64>(); keyPdvYearN.Add(ID_PDV_N);
			Int32 RES_PDV_N1_OFFSET = -1;
			List<Int64> keyPdvYearN1 = new List<Int64>(); keyPdvYearN1.Add(ID_PDV_N1);
			Int32 RES_PDM_N_OFFSET = -1;
			List<Int64> keyPdmYearN = new List<Int64>(); keyPdmYearN.Add(ID_PDM_N);
			Int32 RES_PDM_N1_OFFSET = -1;
			List<Int64> keyPdmYearN1 = new List<Int64>(); keyPdmYearN1.Add(ID_PDM_N1);
			switch (_tableType) {
				case (int)CstTblFormat.productYear_X_Cumul:
				case (int)CstTblFormat.productYear_X_Mensual:
					 RES_PDV_N_OFFSET = (_session.PDV) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
					 RES_PDV_N1_OFFSET = (yearN1 > 0 && _session.PDV) ? RES_PDV_N_OFFSET + 1 : -1;
					break;
				case (int)CstTblFormat.mediaYear_X_Mensual:
				case (int)CstTblFormat.mediaYear_X_Cumul:
					 RES_PDM_N_OFFSET = (_session.PDM) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
					 RES_PDM_N1_OFFSET = (yearN1 > 0 && _session.PDM) ? RES_PDM_N_OFFSET + 1 : -1;
					break;
				default:
					throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
			}
			//string labelN = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.currentYear);
			//string labelN1 = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.previousYear);
			string labelEvol = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
			string labelPDMN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
			string labelPDMN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
			string labelPDVN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
			string labelPDVN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
			#endregion

			
			#endregion

			#region Build headers
			Headers headers = new Headers();
			switch (_tableType) {
				case (int)CstTblFormat.productYear_X_Cumul:
				case (int)CstTblFormat.productYear_X_Mensual:
					headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
					break;
				case (int)CstTblFormat.mediaYear_X_Mensual:
				case (int)CstTblFormat.mediaYear_X_Cumul:
					headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1357, _session.SiteLanguage), ID_PRODUCT));
					break;
				default:
					throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
			}
			//TOTAL
			if (_plusMonthesCumul)
				headers.Root.Add(new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage).ToUpper(), ID_TOTAL));
			//Months			
			CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
			DateTime cMonth = begin;
			//Dictionary<Int32, Header> RES_MONTHES_HEADERS = new Dictionary<Int32, Header>();
			Header[] MONTHES_HEADERS_IDS = new Header[nbMonthes];
			for (int j = 0; j < nbMonthes; j++) {
				MONTHES_HEADERS_IDS[j]= new Header(true, cMonth.ToString("MMMM", cInfo),cMonth.Month);
				headers.Root.Add((Header)MONTHES_HEADERS_IDS[j]);
				cMonth = cMonth.AddMonths(1);
			}
			#endregion

			#region Init Result Table
			tab = new ProductClassResultTable(dtData.Rows.Count * 2, headers);
			tab.SortSubLevels = false;
			CellUnitFactory cellFactory = _session.GetCellUnitFactory();
			CellUnit cell = cellFactory.Get(0.0);
			cell.DisplayContent = false;
			CellUnitFactory cellHiddenFactory = new CellUnitFactory(cell);
			#endregion

			#region Total line
			CellLevel[] parents = new CellLevel[DATA_CLASSIF_INDEXES.Count + 1];
			List<Int64> keys = new List<Int64>();
			keys.Add(ID_TOTAL);
			//Total
			int cLine = 0;
			cLine = tab.AddNewLine(LineType.total, keys, parents[0] = new CellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine));
			//YearN
			tab.AddNewLine(LineType.subTotal1, keys, keyYearN, new CellLevel(ID_YEAR_N, yearN.ToString(), (CellLevel)tab[cLine, 1], 0, cLine + RES_YEAR_N_OFFSET));
			//YearN1
			if (RES_YEAR_N1_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyYearN1, new CellLevel(ID_YEAR_N1, yearN1.ToString(), 0, cLine + RES_YEAR_N1_OFFSET));
			}
			//Evol
			if (RES_EVOL_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, 0, cLine + RES_EVOL_OFFSET));
			}
			//PDV N
			if (RES_PDV_N_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, 0, cLine + RES_PDV_N_OFFSET));
			}
			//PDV N1
			if (RES_PDV_N1_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, 0, cLine + RES_PDV_N1_OFFSET));
			}
			//PDM N
			if (RES_PDM_N_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, 0, cLine + RES_PDM_N_OFFSET));
			}
			//PDM N1
			if (RES_PDM_N1_OFFSET > 0) {
				tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, 0, cLine + RES_PDM_N1_OFFSET));
			}
			for (int i = 2; i <= tab.DataColumnsNumber; i++) {
				//Total
				tab[cLine, i] = cellHiddenFactory.Get(0.0);
				//YearN
				tab[cLine + RES_YEAR_N_OFFSET, i] = cellFactory.Get(0.0);
				//YearN1
				if (RES_YEAR_N1_OFFSET > 0) {
					tab[cLine + RES_YEAR_N1_OFFSET, i] = cellFactory.Get(0.0);
				}
				//Evol
				if (RES_EVOL_OFFSET > 0) {
					tab[cLine + RES_EVOL_OFFSET, i] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, i], tab[cLine + RES_YEAR_N1_OFFSET, i]);
					((CellEvol)tab[cLine + RES_EVOL_OFFSET, i]).StringFormat = "{0:percentage}";
				}
				//PDV N
				if (RES_PDV_N_OFFSET > 0) {
					tab[cLine + RES_PDV_N_OFFSET, i] = new CellPDM(0.0, null);
					((CellPDM)tab[cLine + RES_PDV_N_OFFSET, i]).StringFormat = "{0:percentage}";
				}
				//PDV N1
				if (RES_PDV_N1_OFFSET > 0) {
					tab[cLine + RES_PDV_N1_OFFSET, i] = new CellPDM(0.0, null);
					((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, i]).StringFormat = "{0:percentage}";
				}
				//PDM N
				if (RES_PDM_N_OFFSET > 0) {
					tab[cLine + RES_PDM_N_OFFSET, i] = new CellPDM(0.0, null);
					((CellPDM)tab[cLine + RES_PDM_N_OFFSET, i]).StringFormat = "{0:percentage}";
				}
				//PDM N1
				if (RES_PDM_N1_OFFSET > 0) {
					tab[cLine + RES_PDM_N1_OFFSET, i] = new CellPDM(0.0, null);
					((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, i]).StringFormat = "{0:percentage}";
				}
			}
			#endregion

			#region Fill table
			Int64 cId = -1;          
            List<LineType> lTypes = new List<LineType> { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            List<LineType> lSubTypes = new List<LineType> { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
			Double valueN = 0;
			Double valueN1 = 0;
			List<DetailLevelItemInformation> levels = null;
			
			switch (_tableType) {
				case (int)CstTblFormat.productYear_X_Cumul:
				case (int)CstTblFormat.productYear_X_Mensual:
					levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
					levels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));
					break;
				case (int)CstTblFormat.mediaYear_X_Mensual:
				case (int)CstTblFormat.mediaYear_X_Cumul:
					levels = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
					break;
				default:
					throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
			}

			foreach (DataRow row in dtData.Rows) {
				for (int i = 0; i < DATA_CLASSIF_INDEXES.Count; i++) {
					cId = Convert.ToInt64(row[DATA_CLASSIF_INDEXES[i]]);
					if (parents[i + 1] == null || cId != parents[i + 1].Id) {
						for (int j = parents.Length - 1; j > i; j--) {
							parents[j] = null;
							if (keys.Count > j) {
								keys.RemoveAt(j);
							}
						}
						keys.Add(cId);

						#region Init cells
						//Total
						cLine = tab.AddNewLine(lTypes[i], keys, parents[i + 1] = new CellLevel(cId, row[DATA_CLASSIF_INDEXES[i] + 1].ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 1], i + 1, cLine));
						//YearN
						tab.AddNewLine(lSubTypes[i], keys, keyYearN, new CellLevel(ID_YEAR_N, yearN.ToString(), (CellLevel)tab[cLine, 1], i + 1, cLine + RES_YEAR_N_OFFSET));
						//YearN1
						if (RES_YEAR_N1_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyYearN1, new CellLevel(ID_YEAR_N1, yearN1.ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 1], i + 1, cLine + RES_YEAR_N1_OFFSET));
						}
						//Evol
						if (RES_EVOL_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, i + 1, cLine + RES_EVOL_OFFSET));
						}
						//PDV N
						if (RES_PDV_N_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, 1], i + 1, cLine + RES_PDV_N_OFFSET));
						}
						//PDV N1
						if (RES_PDV_N1_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, 1], i + 1, cLine + RES_PDV_N1_OFFSET));
						}
						//PDM N
						if (RES_PDM_N_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, 1], i + 1, cLine + RES_PDM_N_OFFSET));
						}
						//PDM N1
						if (RES_PDM_N1_OFFSET > 0) {
							tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, 1], i + 1, cLine + RES_PDM_N1_OFFSET));
						}
						for (int k = 2; k <= tab.DataColumnsNumber; k++) {
							//Total
							tab[cLine, k] = cellHiddenFactory.Get(0.0);
							//YearN
							tab[cLine + RES_YEAR_N_OFFSET, k] = cellFactory.Get(0.0);
							//YearN1
							if (RES_YEAR_N1_OFFSET > 0) {
								tab[cLine + RES_YEAR_N1_OFFSET, k] = cellFactory.Get(0.0);
							}
							//Evol
							if (RES_EVOL_OFFSET > 0) {
								tab[cLine + RES_EVOL_OFFSET, k] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, k], tab[cLine + RES_YEAR_N1_OFFSET, k]);
								((CellEvol)tab[cLine + RES_EVOL_OFFSET, k]).StringFormat = "{0:percentage}";
							}
							//PDV N
							if (RES_PDV_N_OFFSET > 0) {
								tab[cLine + RES_PDV_N_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, k]);
								((CellPDM)tab[cLine + RES_PDV_N_OFFSET, k]).StringFormat = "{0:percentage}";
							}
							//PDV N1
							if (RES_PDV_N1_OFFSET > 0) {
								tab[cLine + RES_PDV_N1_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, k]);
								((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, k]).StringFormat = "{0:percentage}";
							}
							//PDM N
							if (RES_PDM_N_OFFSET > 0) {
								tab[cLine + RES_PDM_N_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, k]);
								((CellPDM)tab[cLine + RES_PDM_N_OFFSET, k]).StringFormat = "{0:percentage}";
							}
							//PDM N1
							if (RES_PDM_N1_OFFSET > 0) {
								tab[cLine + RES_PDM_N1_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, k]);
								((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, k]).StringFormat = "{0:percentage}";
							}
						}						
						#endregion

					}
				}

				#region Add Values
				if (DATA_YEAR_N > -1) 
				valueN = Convert.ToDouble(row[DATA_YEAR_N]);
				if (DATA_YEAR_N1 > -1) {
					valueN1 = Convert.ToDouble(row[DATA_YEAR_N1]);
				}
				Int32 CURRENT_MONTH_COLUMN = DATA_FIRST_MONTH_COLUMN;				
				for (int i = 0; i < nbMonthes; i++) {
					//N
					tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN]));
					//N1
					if (RES_YEAR_N1_OFFSET > -1) {
						tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N1_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN + 1]));
					}
					//PDV N
					if (RES_PDV_N_OFFSET > -1) {
						tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN]));
					}
					//PDV N1
					if (RES_PDV_N1_OFFSET > -1) {
						tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N1_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN + 1]));
					}
					//PDM N
					if (RES_PDM_N_OFFSET > -1) {
						tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN]));
					}
					//PDM N1
					if (RES_PDM_N1_OFFSET > -1) {
						tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N1_OFFSET, ((Header)MONTHES_HEADERS_IDS[i]).IndexInResultTable, Convert.ToDouble(row[CURRENT_MONTH_COLUMN + 1]));
					}
					if (yearN1 > -1) {
						CURRENT_MONTH_COLUMN += 2;
					}
					else {
						CURRENT_MONTH_COLUMN++;
					}
				}
				//N
				if (DATA_YEAR_N > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N_OFFSET, 2, valueN);				
				//N1
				if (RES_YEAR_N1_OFFSET > -1) {
					if (DATA_YEAR_N1 > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N1_OFFSET, 2, valueN1);					
				}
				//PDV N
				if (RES_PDV_N_OFFSET > -1) {
					if (DATA_YEAR_N > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N_OFFSET, 2, valueN);					
				}
				//PDV N1
				if (RES_PDV_N1_OFFSET > -1) {
					if (DATA_YEAR_N1 > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N1_OFFSET, 2, valueN1);					
				}
				//PDM N
				if (RES_PDM_N_OFFSET > -1) {
					if (DATA_YEAR_N > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N_OFFSET, 2, valueN);					
				}
				//PDM N1
				if (RES_PDM_N1_OFFSET > -1) {
					if (DATA_YEAR_N1 > -1) tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N1_OFFSET, 2, valueN1);					
				}
				#endregion

				#region Advertisers univers
				if (_isPersonalized > 0) {
					for (int i = parents.Length - 1; i >= 0; i--) {
						SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable, row, levels[i].Id);
						SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, row, levels[i].Id);
						//YearN1
						if (RES_YEAR_N1_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, row, levels[i].Id);
						}
						//Evol
						if (RES_EVOL_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_EVOL_OFFSET, row, levels[i].Id);
						}
						//PDV N
						if (RES_PDV_N_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, row, levels[i].Id);
						}
						//PDV N1
						if (RES_PDV_N1_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, row, levels[i].Id);
						}
						//PDM N
						if (RES_PDM_N_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, row, levels[i].Id);
						}
						//PDM N1
						if (RES_PDM_N1_OFFSET > 0) {
							SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, row, levels[i].Id);
						}
					}
				}
				#endregion

			}		
			#endregion          

            #region Hide lines if required
            if (_session.PersonalizedElementsOnly && _isPersonalized > 0)
            {
                HideNonCustomisedLines(tab, lTypes, lSubTypes);
            }
            #endregion

			return tab;

		}
		#endregion

		


	}
}
