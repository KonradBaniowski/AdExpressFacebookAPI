#region Information
/*
 * Author : D Mussuma
 * Creation : 13/03/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using DALClassif = TNS.AdExpress.DataAccess.Classification;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;

using TNS.AdExpress;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpressI.PresentAbsent.DAL;
using TNS.AdExpressI.PresentAbsent.Exceptions;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Collections;
using TNS.AdExpressI.PresentAbsent;

using TNS.AdExpressI.Classification.DAL;
#endregion






namespace TNS.AdExpressI.PresentAbsent.Russia {
	/// <summary>
	/// Russia Present/Absent reports
	/// </summary>
	public class PresentAbsentResult : PresentAbsent.PresentAbsentResult{

		#region Constructor
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		public PresentAbsentResult(WebSession session)
			: base(session) {
		}
		#endregion

		#region GetCrossTable
		/// <summary>
		/// Compute data
		/// </summary>
		/// <param name="session">User Session</param>
		/// <param name="groupMediaTotalIndex">List of indexes of groups selection</param>
		/// <param name="subGroupMediaTotalIndex">List of indexes of sub groups selections</param>
		/// <param name="mediaIndex">Media indexes</param>
		/// <param name="nbCol">Column number in result table</param>
		/// <param name="nbLineInNewTable">(out) Line number in table result</param>
		/// <param name="nbUnivers">(out) Univers number</param>
		/// <param name="mediaListForLabelSearch">(out)Media Ids list</param>
		/// <returns>Data table</returns>
		protected override ResultTable GetGrossTable(out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal) {

			#region Load data from data layer
			DataTable dt = null;
			DataSet dsMedia = null, ds = null;
			int levelNb = _session.GenericProductDetailLevel.GetNbLevels;
			DetailLevelItemInformation lowestLevel = _session.GenericProductDetailLevel[levelNb];
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
			object[] parameters = new object[1];
			parameters[0] = _session;
			IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = presentAbsentDAL.GetData();
			dsMedia = presentAbsentDAL.GetColumnDetails();
			dt = ds.Tables[lowestLevel.Id.ToString()];
			DataTable dtMedia = dsMedia.Tables[0];

			if (dt == null || dt.Rows.Count == 0) {
				universesSubTotal = null;
				elementsHeader = null;
				elementsSubTotal = null;
				return null;
			}
			#endregion

			#region Get Headers
			Dictionary<Int64, Int64> mediaToUnivers = null;
			Headers headers = GetHeaders(dtMedia, out elementsHeader, out elementsSubTotal, out universesSubTotal, out mediaToUnivers);
			#endregion

			#region Init ResultTable
			Int32 nbline = GetNbLine(dt);
			ResultTable tabData = new ResultTable(nbline, headers);
			#endregion

			#region Fill result table
			
			Int64[] oldIds = new Int64[levelNb];
			Int64[] cIds = new Int64[levelNb];
			CellLevel[] levels = new CellLevel[nbline];
			Int32 cLine = 0;
			for (int i = 0; i < levelNb; i++) { oldIds[i] = cIds[i] = -1; }
			CellUnitFactory cellFactory = _session.GetCellUnitFactory();
			SetLineDelegate setLine;
			switch (_session.Unit) {
				case CstWeb.CustomerSessions.Unit.versionNb:
					setLine = new SetLineDelegate(SetListLine);
					break;
				default:
					setLine = new SetLineDelegate(SetDoubleLine);
					break;
			}
			foreach (DataRow row in dt.Rows) {
				for (int i = 0; i < levelNb; i++) {
					cIds[i] = _session.GenericProductDetailLevel.GetIdValue(row, i + 1);
					if (cIds[i] >= 0 && cIds[i] != oldIds[i]) {
						oldIds[i] = cIds[i];
						for (int ii = i + 1; ii < levelNb; ii++) { oldIds[ii] = -1; }
						cLine = InitDoubleLine(tabData, row, cellFactory, i + 1, (i > 0) ? levels[i - 1] : null);
						levels[i] = (CellLevel)tabData[cLine, 1];
					}
				}
				setLine(tabData, elementsHeader, elementsSubTotal, cLine, row, cellFactory, mediaToUnivers);
			}
			#endregion

			return tabData;

		}
		#endregion

		#region Initialisation des indexes
		/// <summary>
		/// Init headers
		/// </summary>
		/// <param name="elementsHeaders">(ou) Header for each level element</param>
		/// <param name="dtMedia">List of medias with the detail level matching
		protected override Headers GetHeaders(DataTable dtMedia, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal, out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<Int64, Int64> idMediaToIdUnivers) {

			#region Extract Media lists
			string tmp = string.Empty;
			Int64[] tIds = null;
			int iUnivers = 1;
			DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
			DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);
			//DALClassif.ClassificationLevelListDataAccess levels = null;
			CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
			if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
			object[] param = new object[2];
			param[0] = _session.Source;
			param[1] = _session.DataLanguage;
			TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
			TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levels = null;
			
			//Elements by univers
			//Dictionary<Int64, string> currentColItems = new Dictionary<long, string>();
			Dictionary<Int64, List<Int64>> idsByUnivers = new Dictionary<Int64, List<Int64>>();
			//Media ids ==> id univers mapping
			idMediaToIdUnivers = new Dictionary<Int64, Int64>();
			//Init media univers mapping
			while (_session.CompetitorUniversMedia[iUnivers] != null) {
				idsByUnivers.Add(iUnivers, new List<Int64>());
				// Load media ids
				tmp = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[iUnivers], CstCustomer.Right.type.mediaAccess);
				tIds = Array.ConvertAll<string, Int64>(tmp.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); });
				//Init Media ids X univers
				foreach (Int64 l in tIds) {
					if (!idMediaToIdUnivers.ContainsKey(l)) {
						idMediaToIdUnivers.Add(l, iUnivers);
					}
				}
				iUnivers++;
			}
			iUnivers--;

			//Dispatch elements in current univers
			List<Int64> idElements = new List<Int64>();
			StringBuilder sIdElments = new StringBuilder();
			Int64 idElement = -1;
			Int64 idMedia = -1;
			foreach (DataRow row in dtMedia.Rows) {
				//idElement = Convert.ToInt64(row["columnDetailLevel"]);
				idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
				idMedia = Convert.ToInt64(row[mediaDetailLevelItemInformation.DataBaseIdField]);
				if (!idElements.Contains(idElement)) {
					idElements.Add(idElement);
					sIdElments.AppendFormat("{0},", idElement);
				}
				if (!idsByUnivers[idMediaToIdUnivers[idMedia]].Contains(idElement)) {
					idsByUnivers[idMediaToIdUnivers[idMedia]].Add(idElement);
				}				

			}
			if (sIdElments.Length > 0) sIdElments.Length -= 1;
			#endregion

			#region Load elements labels

			levels = factoryLevels.CreateClassificationLevelListDAL(columnDetailLevel, sIdElments.ToString());
			
			#endregion

			#region Build headers

			#region Current Columns
			// Product column
			Headers headers = new Headers();
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_HEADER_ID));


			// Add Creative coumn
			if (_vehicleInformation.ShowCreations &&
				_session.CustomerLogin.ShowCreatives(_vehicleInformation.Id) &&
				(_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))) {
				headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, _session.SiteLanguage), CREATIVE_HEADER_ID));
				_showCreative = true;
			}

			// Add insertion colmun
			if (_vehicleInformation.ShowInsertions &&
				(_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))) {
				headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, _session.SiteLanguage), INSERTION_HEADER_ID));
				_showInsertions = true;
			}

			// Add Media Schedule column
			if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector) ||
				_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
				) {
				headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_HEADER_ID));
				_showMediaSchedule = true;
			}
			#endregion

			#region Total column
			Header headerTmp = null;
			Header headerTotal = null;
			elementsHeader = new Dictionary<string, HeaderBase>();
			if (_session.CompetitorUniversMedia.Count > 1 || idElements.Count > 1) {
				headerTotal = new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage), TOTAL_HEADER_ID);
				elementsHeader.Add(TOTAL_HEADER_ID.ToString(), headerTotal);
				headers.Root.Add(headerTotal);
				_showTotal = true;
			}
			#endregion

			#region Elements groups
			HeaderGroup headerGroupTmp = null;
			Header headerGroupSubTotal = null;
			iUnivers = 1;
			elementsSubTotal = new Dictionary<string, HeaderBase>();
			universesSubTotal = new Dictionary<Int64, HeaderBase>();
			while (_session.CompetitorUniversMedia[iUnivers] != null) {
				//Group init
				if (iUnivers != 1) {
					headerGroupTmp = new HeaderGroup(string.Format("{0} {1}", GestionWeb.GetWebWord(1366, _session.SiteLanguage), iUnivers - 1), true, START_ID_GROUP + iUnivers);
				}
				else {
					headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1365, _session.SiteLanguage), true, START_ID_GROUP + iUnivers);
				}
				if (idsByUnivers[iUnivers].Count > 1 && _session.CompetitorUniversMedia.Count > 1) {
					headerGroupSubTotal = headerGroupTmp.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUB_TOTAL_HEADER_ID);
					universesSubTotal.Add(iUnivers, headerGroupSubTotal);
				}
				List<Header> heads = new List<Header>();
				foreach (Int64 id in idsByUnivers[iUnivers]) {
					headerTmp = new Header(true, levels[id], id);
					//headerTmp = new Header(true, currentColItems[id], id);
					heads.Add(headerTmp);
					elementsHeader.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
					if (!headerGroupTmp.ContainSubTotal) {
						if (!universesSubTotal.ContainsKey(iUnivers)) {
							if (iUnivers == 1 && idsByUnivers.Count < 2 && idsByUnivers[1].Count > 1) {
								universesSubTotal.Add(iUnivers, headerTotal);
							}
							else {
								universesSubTotal.Add(iUnivers, headerTmp);
							}
						}
						elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
					}
					else {
						elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerGroupSubTotal);
					}
				}
				heads.Sort(delegate(Header h1, Header h2) { return h1.Label.CompareTo(h2.Label); });
				foreach (Header h in heads) {
					headerGroupTmp.Add(h);
				}
				headers.Root.Add(headerGroupTmp);
				iUnivers++;
			}
			#endregion

			#endregion

			return headers;

		}
		#endregion

		/// <summary>
		/// Delegate to affect double values to the table
		/// </summary>
		/// <param name="tab">Table to fill</param>
		/// <param name="elementsHeader">Headers by element ids (media, interst centers...)</param>
		/// <param name="cLine">Current line</param>
		/// <param name="row">Data container</param>
		/// <param name="cellFactory">Cell Factory for double cells</param>
		/// <returns>Current line</returns>
		protected override Int32 SetDoubleLine(ResultTable tab, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Dictionary<Int64, Int64> mediaToUnivers) {
			DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
			DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);

			Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
			Int64 idMedia = Convert.ToInt64(row[mediaDetailLevelItemInformation.DataBaseIdField]);
			Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);
			string sIdElement = string.Format("{0}-{1}", mediaToUnivers[idMedia], idElement);
			tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[sIdElement].IndexInResultTable, value);
			// SubTotal if required (univers contains more than one element)
			if (elementsHeader[sIdElement] != elementsSubTotal[sIdElement]) {
				tab.AffectValueAndAddToHierarchy(1, cLine, elementsSubTotal[sIdElement].IndexInResultTable, value);
			}
			// Total if required
			if (elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString()) && elementsSubTotal[sIdElement] != elementsHeader[TOTAL_HEADER_ID.ToString()]) {
				tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[TOTAL_HEADER_ID.ToString()].IndexInResultTable, value);
			}
			return cLine;

		}
	}
}
