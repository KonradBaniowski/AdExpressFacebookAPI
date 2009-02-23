#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Data;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using ClassificationDA=TNS.AdExpress.DataAccess.Classification;
using ClassificationTable = TNS.AdExpress.Constantes.Classification.DB.Table;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Controls.Results;
using TNS.AdExpress.Web.Controls.Buttons;
using TNS.AdExpress.Web.Controls.Translation;
using AjaxPro;
//using obout_ASPTreeView_2_NET;
using UniverseAccessType = TNS.Classification.Universe.AccessType;
using TNS.FrameWork.Exceptions;
using TNS.Classification;
using TNS.Classification.Universe;
using TNS.Classification.WebControls;
using TNS.AdExpress.Domain.Web.Navigation;
using CoreSelection=TNS.AdExpress.Web.Core.Selection;
using FrameWorkSelection = TNS.AdExpress.Constantes.FrameWork.Selection;
using Oracle.DataAccess.Client;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Control to  select Items in Classification
	/// </summary>
	[ToolboxData("<{0}:SelectItemsInClassificationWebControl runat=server></{0}:SelectItemsInClassificationWebControl>")]
	public class SelectItemsInClassificationWebControl : TNS.Classification.WebControls.BaseSelectItemsInClassificationWebControl {

		#region Variables

		/// <summary>
		/// Object session
		/// </summary>
		protected WebSession _webSession = null;
		/// <summary>
		/// Id session
		/// </summary>
		protected string _idSession = "";
		/// <summary>
		/// Is for selection page
		/// </summary>
		protected bool _forSelectionPage = true;
		/// <summary>
		/// Event target
		/// </summary>
		protected int _eventTarget = 0;
		/// <summary>
		/// Code error
		/// </summary>
		protected int _errorCode = 0;
		#endregion

		#region Accesseurs

		/// <summary>
		/// Get / Set client session 
		/// </summary>
		public WebSession CustomerWebSession {
			get { return _webSession; }
			set { _webSession = value; }
		}

		/// <summary>
		/// Get / Set session Id 
		/// </summary>
		public string IdSession {
			get { return _idSession; }
			set { _idSession = value; }
		}

		/// <summary>
		/// Get/Set Is the conrol is in selection page (else optional selection page)
		/// </summary>
		/// <remarks>Used for levels and branch loading mechanism</remarks>
		public bool ForSelectionPage {
			get { return _forSelectionPage; }
			set { _forSelectionPage = value; }
		}

		/// <summary>
		/// Get/Set Event target ( ex. Reload universe, save,...)
		/// </summary>
		public int EventTarget_ {
			get { return _eventTarget; }
			set { _eventTarget = value; }
		}

		/// <summary>
		/// Get/Set Error code
		/// </summary>
		public int ErrorCode {
			get { return _errorCode; }
			set { _errorCode = value; }
		}
		#endregion

		#region JavaScript

		#region SaveParametersScript
		/// <summary>
		/// Save parameters for Ajax functions
		/// </summary>
		/// <returns></returns>
		protected override string SaveParametersScript() {
			StringBuilder js = new StringBuilder(3000);
			js.Append((base.SaveParametersScript()));
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\n\nfunction SaveSessionParametersScript(obj){");
			js.Append("\r\n\t obj.IdSession = '" + _idSession + "';");
			js.Append("\r\n }");
			js.Append("\r\n\t SaveSessionParametersScript(savedParameters);");
			js.Append("\r\n-->\r\n</SCRIPT>");
			return (js.ToString());
		}

		/// <summary>
		/// Loadind saved parameters between client and server
		/// </summary>
		/// <param name="o">javascript's parameters array</param>
		protected override void LoadSavedParameters(AjaxPro.JavaScriptObject o) {
			base.LoadSavedParameters(o);
			if (o.Contains("IdSession")) {
				this._idSession = o["IdSession"].Value.Replace("\"", "");
			}
		}
		#endregion

		#endregion

		#region Load
		/// <summary>
		/// Loading
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			AjaxPro.Utility.RegisterTypeForAjax(typeof(SelectItemsInClassificationWebControl));
			base.OnLoad(e);
		}
		#endregion		

		#region OnInit
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			//Init allowed branch and levels loading
			LoadBranchAndLevelsForCurrentPage(_webSession.CustomerLogin.GetModule(_webSession.CurrentModule));
		}
		#endregion

		#region Public Methods

		#region GetSelection
		/// <summary>
		/// Get univese selection 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="page">curent page</param>
		/// <param name="dimension">dimension (product or advertiser)</param>
		/// <param name="security">security level odf univers object</param>
		/// <returns>AdExpressUniverse</returns>
		public virtual  TNS.AdExpress.Classification.AdExpressUniverse GetSelection(WebSession webSession, Page page, TNS.Classification.Universe.Dimension dimension, TNS.Classification.Universe.Security security) {
			return GetSelection(webSession, page, dimension, security, null);
		}

		/// <summary>
		/// Get univese selection 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="page">curent page</param>
		/// <param name="dimension">dimension (product or advertiser)</param>
		/// <param name="security">security level odf univers object</param>
		/// <param name="universeLabel">Universe label</param>
		/// <returns>AdExpressUniverse</returns>
		public virtual TNS.AdExpress.Classification.AdExpressUniverse GetSelection(WebSession webSession, Page page, TNS.Classification.Universe.Dimension dimension, TNS.Classification.Universe.Security security, string universeLabel) {
			try {
				string selectedIds = "";
				string[] st = null;
				string[] tempArr = null, tempArr2= null;
				long levelId = -1;
				UniverseAccessType accessType;
				string oldTreeViewId = "";
				TNS.Classification.Universe.NomenclatureElementsGroup group = null;
				TNS.AdExpress.Classification.AdExpressUniverse universe = null;
				bool first = true;

				universe =  (universeLabel != null && universeLabel.Length>0) ? new TNS.AdExpress.Classification.AdExpressUniverse(universeLabel,dimension) : new TNS.AdExpress.Classification.AdExpressUniverse(dimension);

				foreach (string currentKey in page.Request.Form.AllKeys) {
					if (currentKey.IndexOf("TreeLevelSelectedIds") > -1) {
						st = page.Request.Form.GetValues(currentKey);
						if (st != null && st.Length > 0 && st[0].Length > 0) {
							selectedIds = st[0].ToString();
							tempArr = currentKey.Split('_');

							accessType = (UniverseAccessType)Int64.Parse(tempArr[1].ToString());
							if (!oldTreeViewId.Equals(tempArr[0].ToString())) {
								if (!first) universe.AddGroup(universe.Count(), group);
								group = new TNS.Classification.Universe.NomenclatureElementsGroup(Int64.Parse(tempArr[1].ToString()), accessType);
							}

							levelId = Int64.Parse(tempArr[2].ToString());
							tempArr2 = selectedIds.Split(',');
							if (tempArr2 != null && tempArr2.Length > _nbMaxItemByLevel) throw new TNS.Classification.Universe.CapacityException("Dépassement du nombre d'éléments autorisés pour un niveau");
							
							group.AddItems(levelId, selectedIds);
							oldTreeViewId = tempArr[0].ToString();
							first = false;
						}
					}
				}
				if (group != null && group.Count() > 0)
					universe.AddGroup(universe.Count(), group);
				
				return universe;

				
			}			
			catch (Exception err) {
				if (err.GetType() == typeof(TNS.Classification.Universe.SecurityException) ||
					err.GetBaseException().GetType() == typeof(TNS.Classification.Universe.SecurityException)) {
				 throw new TNS.Classification.Universe.SecurityException("Permission refusée pour la sauvegarde des univers", err);
				}
				else if (err.GetType() == typeof(TNS.Classification.Universe.CapacityException)) {
					throw new TNS.Classification.Universe.CapacityException("Dépassement du nombre d'éléments autorisés pour un niveau", err);
				}
				else if (err.GetType() == typeof(TNS.AdExpress.Web.Exceptions.NoDataException)) {
					throw new TNS.AdExpress.Web.Exceptions.NoDataException();
				}
				throw new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible de sauvegarder la selection d'univers.", err);
			}
		}
		#endregion

		#region ShowUniverse
		/// <summary>
		/// Show Universe selected
		/// </summary>
		/// <param name="adExpressUniverse">adExpress Universe</param>
		/// <param name="dataLanguage">language</param>
		/// <param name="connection">connection</param>
		/// <returns>Html code of universe selected</returns>
		public string ShowUniverse(TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source) {

			DataSet ds = null;
			int treeViewId = 0;
			string nodeCurrentText = "", childNodeCss = "";
			UniverseAccessType accessType;
			List<NomenclatureElementsGroup> groups = null;
            TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess universeItems = null;
			string childId = "";
			StringBuilder html = new StringBuilder();
			int code = 0, headerCode = 0;
			string treeFrameCss = "", treeFrameBodyCss = "", treeFrameHeaderCss = "";
			ArrayList itemIdList = null;
			obout_ASPTreeView_2_NET.Tree oTree = null;
			string nodeCss = "";

			//Groups of items excludes
			groups = adExpressUniverse.GetExludes();

			if (groups != null && groups.Count > 0) {
				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();
					accessType = UniverseAccessType.excludes;
					childNodeCss = (accessType == UniverseAccessType.includes) ? _childNodeIncludeCss : _childNodeExcludeCss;
					if (accessType == UniverseAccessType.includes) {
						code = _includeTextCode;
						headerCode = _includeElementsTextCode;
						treeFrameCss = _treeIncludeFrameCss;
						treeFrameBodyCss = _treeIncludeFrameBodyCss;
						treeFrameHeaderCss = _treeIncludeFrameHeaderCss;
					}
					else {
						code = _excludeTextCode;
						headerCode = _excludeElementsTextCode;
						treeFrameCss = _treeExcludeFrameCss;
						treeFrameBodyCss = _treeExcludeFrameBodyCss;
						treeFrameHeaderCss = _treeExcludeFrameHeaderCss;
					}

					if (levelIdsList != null) {
						nodeCss = _parentNodeChildExcludeCss;

						oTree = new obout_ASPTreeView_2_NET.Tree();
						oTree.id = "treeViewId" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString();

						for (int j = 0; j < levelIdsList.Count; j++) {

                            universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName,groups[i].GetAsString(levelIdsList[j]),dataLanguage,source);
							if (universeItems != null) {
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {
									nodeCurrentText = "<div class=\"" + nodeCss + "\">" + GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, _siteLanguage) + "</div>";
									oTree.Add("root", "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + levelIdsList[j].ToString(), nodeCurrentText, false, null, null);

									for (int k = 0; k < itemIdList.Count; k++) {
										nodeCurrentText = "<div class=" + childNodeCss + " nowrap>" + universeItems[Int64.Parse(itemIdList[k].ToString())];
										nodeCurrentText += "</div>";
										childId = treeViewId + "_" + levelIdsList[j] + "_" + j.ToString();
										oTree.Add("t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + levelIdsList[j].ToString(), childId, nodeCurrentText, false, null, null);
									}
								}
							}
						}
						oTree.FolderIcons = _treeViewIcons;
						oTree.FolderScript = _treeViewScripts;
						oTree.FolderStyle = _treeViewStyles;
						oTree.ShowIcons = false;
						oTree.Height = _treeViewHeight;
						oTree.Width = _treeViewWidth;
						oTree.ShowRootPlusMinus = false;
						//Debut Boite  arbe element selectionnés  

						html.Append("<td valign=\"top\">");
						html.Append("<table class=\"" + treeFrameCss + "\" cellpadding=\"0\" cellspacing=\"0\">");
						html.Append("<tr class=\"" + treeFrameHeaderCss + "\"><td>&nbsp;" + GetWebWord(headerCode, _siteLanguage) + "</td><tr>");
						html.Append(" <tr class=\"" + treeFrameBodyCss + "\"> <td>");
						html.Append(oTree.HTML());
						html.Append("</td></tr>");
						html.Append("</table>");
						html.Append("</td>");//</tr>
						//Fin Boite  arbe element selectionés 

						treeViewId++;
					}
				}
			}

			//Groups of items includes
			groups = adExpressUniverse.GetIncludes();

			if (groups != null && groups.Count > 0) {
				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();
					accessType = UniverseAccessType.includes;
					childNodeCss = (accessType == UniverseAccessType.includes) ? _childNodeIncludeCss : _childNodeExcludeCss;
					if (accessType == UniverseAccessType.includes) {
						code = _includeTextCode;
						headerCode = _includeElementsTextCode;
						treeFrameCss = _treeIncludeFrameCss;
						treeFrameBodyCss = _treeIncludeFrameBodyCss;
						treeFrameHeaderCss = _treeIncludeFrameHeaderCss;
					}
					else {
						code = _excludeTextCode;
						headerCode = _excludeElementsTextCode;
						treeFrameCss = _treeExcludeFrameCss;
						treeFrameBodyCss = _treeExcludeFrameBodyCss;
						treeFrameHeaderCss = _treeExcludeFrameHeaderCss;
					}

					if (levelIdsList != null) {
						nodeCss = _parentNodeChildIncludeCss;
						oTree = new obout_ASPTreeView_2_NET.Tree();
						oTree.id = "treeViewId" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString();

						for (int j = 0; j < levelIdsList.Count; j++) {

                            universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName,groups[i].GetAsString(levelIdsList[j]),dataLanguage,source);
							if (universeItems != null) {
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {
									nodeCurrentText = "<div class=\"" + nodeCss + "\">" + GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, _siteLanguage) + "</div>";
									oTree.Add("root", "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + levelIdsList[j].ToString(), nodeCurrentText, false, null, null);

									for (int k = 0; k < itemIdList.Count; k++) {
										nodeCurrentText = "<div class=" + childNodeCss + " nowrap>" + universeItems[Int64.Parse(itemIdList[k].ToString())];
										nodeCurrentText += "</div>";
										childId = treeViewId + "_" + levelIdsList[j] + "_" + j.ToString();
										oTree.Add("t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + levelIdsList[j].ToString(), childId, nodeCurrentText, false, null, null);
									}
								}
							}
						}
						oTree.FolderIcons = _treeViewIcons;
						oTree.FolderScript = _treeViewScripts;
						oTree.FolderStyle = _treeViewStyles;
						oTree.ShowIcons = false;
						oTree.Height = _treeViewHeight;
						oTree.Width = _treeViewWidth;
						oTree.ShowRootPlusMinus = false;
						//Debut Boite  arbe element selectionnés  
						//html.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
						html.Append("<td valign=\"top\">");//<tr>
						html.Append("<table class=\"" + treeFrameCss + "\" cellpadding=\"0\" cellspacing=\"0\">");
						html.Append("<tr class=\"" + treeFrameHeaderCss + "\"><td>&nbsp;" + GetWebWord(headerCode, _siteLanguage) + "</td><tr>");
						html.Append(" <tr class=\"" + treeFrameBodyCss + "\"> <td>");
						html.Append(oTree.HTML());
						html.Append("</td></tr>");
						html.Append("</table>");
						html.Append("</td>");//</tr>
						//Fin Boite  arbe element selectionés 
						//html.Append("</table>");
						treeViewId++;
					}
				}
			}

			return "<table cellpadding=\"0\" cellspacing=\"1\"><tr>" + html.ToString() + "</tr></table>";
		}
		#endregion

		#endregion

		#region Private Methods

		#region GetMessageError
		/// <summary>
		/// Error message
		/// </summary>
		/// <param name="customerSession">customer's session</param>
		/// <param name="code">Code message</param>
		/// <returns> Error message string</returns>
		protected string GetMessageError(WebSession customerSession, int code) {
			string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\">";
			if (customerSession != null)
				errorMessage += GetWebWord(code, _siteLanguage) + ". " + GetWebWord(2099, _siteLanguage);
			else
				errorMessage += GetWebWord(code, 33) + ". " + GetWebWord(2099, 33);

			errorMessage += "</div>";
			return errorMessage;
		}
		#endregion

		#region LoadBranchAndLevelsForCurrentPage
		/// <summary>
		/// Load allowed branches and levels id for the current Page
		/// </summary>
		protected virtual void LoadBranchAndLevelsForCurrentPage(Module module) {
			CoreSelection.ILevelsRules levelsRules = null;
			List<int> tempBranchIds = null;
			List<UniverseLevel> tempLevels = null;
			if (_forSelectionPage) {
				foreach (SelectionPageInformation currentPage in module.SelectionsPages) {
					if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath)) {
																		
						//Apply rigth Rules for getting levels and branches
						levelsRules = new CoreSelection.AdExpressLevelsRules(_webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds),_dimension);
						tempBranchIds = levelsRules.GetAuthorizedBranches();
						tempLevels = levelsRules.GetAuthorizedLevels();
						if (tempBranchIds != null && tempBranchIds.Count > 0)
							_allowedBranchesIds = tempBranchIds;
						if (tempLevels != null && tempLevels.Count > 0)
							_allowedLevels = tempLevels;
					}			
				}
			}else{
				foreach (OptionalPageInformation currentPage in module.OptionalsPages) {
					if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath)) {
						#region Old version
						//if (currentPage.AllowedLevelsIds != null && currentPage.AllowedLevelsIds.Count > 0)
						//    _allowedLevels = UniverseLevels.GetList(currentPage.AllowedLevelsIds);
						//if (currentPage.AllowedBranchesIds != null && currentPage.AllowedBranchesIds.Count > 0)
						//    _allowedBranchesIds = currentPage.AllowedBranchesIds;

						#endregion
						//Apply rigth Rules for getting levels and branches
						levelsRules = new CoreSelection.AdExpressLevelsRules(_webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds), _dimension);
						tempBranchIds = levelsRules.GetAuthorizedBranches();
						tempLevels = levelsRules.GetAuthorizedLevels();
						if (tempBranchIds != null && tempBranchIds.Count > 0)
							_allowedBranchesIds = tempBranchIds;
						if (tempLevels != null && tempLevels.Count > 0)
							_allowedLevels = tempLevels;
					}
				}
			}
		}		
		#endregion

		#region Render UniverseLevel TreeView
		/// <summary>
		///TreeView Html Render
		/// </summary>
		/// <param name="html">ouput html</param>
		/// <param name="listId">list Id  of levels</param>
		protected override void RenderUniverseLevelTreeView(StringBuilder html, string listId) {
			try {
				
				Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = null;
				int indexUnivers = 0;
				switch (_eventTarget) {
					case FrameWorkSelection.eventSelection.LOAD_EVENT :
					case FrameWorkSelection.eventSelection.SAVE_EVENT:
					//case FrameWorkSelection.eventSelection.NEXT_EVENT:
						switch (_dimension) {
							case TNS.Classification.Universe.Dimension.product:
								universes = _webSession.PrincipalProductUniverses;
								break;
                            case TNS.Classification.Universe.Dimension.media:
                                if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA)
                                    universes = _webSession.SecondaryMediaUniverses;
                                else
                                    universes = _webSession.PrincipalMediaUniverses;
                                break;
							default: break;
						}

						if ((FrameWorkSelection.eventSelection.NEXT_EVENT == _eventTarget) && (universes != null) && universes.Count>0) 
							indexUnivers = universes.Count - 1;

						if (universes != null && universes.ContainsKey(indexUnivers) && universes[indexUnivers].Count() > 0) {
							RenderUniverseLevelTreeView(html, listId, universes[indexUnivers]);
						}
						else {
							if (_errorCode == FrameWorkSelection.error.SECURITY_EXCEPTION
								|| _errorCode == FrameWorkSelection.error.MAX_ELEMENTS
								|| _errorCode == FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE)
								BuildUniverseLevelTreeView(html);
							else base.RenderUniverseLevelTreeView(html, listId);
						}
						break;
					default :
						if(_errorCode == FrameWorkSelection.error.SECURITY_EXCEPTION
							|| _errorCode == FrameWorkSelection.error.MAX_ELEMENTS
							|| _errorCode == FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE)
						 BuildUniverseLevelTreeView(html);
						else base.RenderUniverseLevelTreeView(html, listId);
						break;
				}
				
			}
			catch (System.Exception err) {
				throw (new Exceptions.SelectItemsInClassificationWebControlException("Impossible de rendre le code html du Treeview Obout", err));
			}
		}

		/// <summary>
		///Obout control Treeview html render method
		/// </summary>
		/// <param name="html">html builder </param>		
		/// <param name="listId">Id list</param>
		/// <param name="adExpressUniverse">adexpress universe</param>
		protected void RenderUniverseLevelTreeView(StringBuilder html,string listId, TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse) {
			int treeviewId = 0, nbCurrentTree =0;
			//int code = 0, headerCode = 0;
			int nbTotalTree = 0;
			//string treeFrameCss = "", treeFrameBodyCss = "", treeFrameHeaderCss = "";
			int coutTree = 0, dif=0;
			List<NomenclatureElementsGroup> elementsGroups = null;

			if (adExpressUniverse != null && adExpressUniverse.Count() > 0) {
				html.Append("<table with=100%><tr valign=\"bottom\">");
				
				nbTotalTree = _nbMaxExcludeTree + _nbMaxIncludeTree;
				if (nbTotalTree==0) nbTotalTree = adExpressUniverse.Count();
				 elementsGroups = adExpressUniverse.GetExludes();

				//Build exclude tree
				 if (elementsGroups != null && elementsGroups.Count > 0) {
					 for (int j = 0; j < elementsGroups.Count; j++) {
						BuildUniverseLevelTreeView(html, ref treeviewId, elementsGroups[j].AccessType, elementsGroups[j], listId, ref nbCurrentTree, nbTotalTree);
						coutTree++;
					}
					dif = _nbMaxExcludeTree - coutTree;
					if (dif>0) {
						for (int j = 0; j < dif; j++) {
							BuildUniverseLevelTreeView(html, ref treeviewId, UniverseAccessType.excludes, null, listId, ref nbCurrentTree, nbTotalTree);
						}
					}
				}
				else {
					for (int j = 0; j < _nbMaxExcludeTree; j++) {
						BuildUniverseLevelTreeView(html, ref treeviewId, UniverseAccessType.excludes, null, listId, ref nbCurrentTree, nbTotalTree);
						coutTree++;
					}
				}

				//Build Include trees
				coutTree = 0;
				dif = 0;
				elementsGroups = adExpressUniverse.GetIncludes();
				if (elementsGroups != null && elementsGroups.Count > 0) {
					for (int j = 0; j < elementsGroups.Count; j++) {
						BuildUniverseLevelTreeView(html, ref treeviewId, elementsGroups[j].AccessType, elementsGroups[j], listId, ref nbCurrentTree, nbTotalTree);
						coutTree++;
					}
					dif = _nbMaxIncludeTree - coutTree;
					if (dif > 0) {
						for (int j = 0; j < dif; j++) {
							BuildUniverseLevelTreeView(html, ref treeviewId, UniverseAccessType.includes, null, listId, ref nbCurrentTree, nbTotalTree);
						}
					}
				}
				else {
					for (int j = 0; j < _nbMaxIncludeTree; j++) {
						BuildUniverseLevelTreeView(html, ref treeviewId, UniverseAccessType.includes, null, listId, ref nbCurrentTree, nbTotalTree);
					}
				}				

				html.Append("</tr></table>");
			}
		}
		#endregion

		#region BuildUniverseLevelTreeView
		/// <summary>
		/// Build universe level reeview
		/// </summary>
		/// <param name="html">html string builder</param>
		protected virtual void BuildUniverseLevelTreeView(StringBuilder html) {
			
			obout_ASPTreeView_2_NET.Tree oTree = null;
			string selectedIds = "";
			string[] st = null;
			string[] tempArr = null;
			long levelId = -1;
			UniverseAccessType accessType = UniverseAccessType.excludes, oldAccessType = UniverseAccessType.excludes;
			string oldTreeViewId = "";
			int treeViewId = 0, oldIntTreeViewId = 0;
			string nodeCurrentText = "";
			List<long> oldLevelsId = new List<long>();
			string nodeCss = "";
			string childNodeCss = "";
			UniverseLevel currentLevel = null;
			DataSet ds = null;
			//long groupKey = 1;
			bool first = true;
			string childId = "", temp="";
			string levelIds = "", listId = "";
			string hiddenFieldIds = "", inputHiddenFields = ""; int code = 0, headerCode = 0;
			string treeFrameCss = "", treeFrameBodyCss = "", treeFrameHeaderCss = "";
			int nbCurrentTree = 0, nbTotalTree = _nbMaxExcludeTree + _nbMaxIncludeTree;

			foreach (UniverseLevel level in _allowedLevels)
				listId = listId + "classificationList_" + level.ID + ",";
			if (listId != null && listId.Length > 0) listId = listId.Substring(0, listId.Length - 1);

			html.Append("<table with=100%><tr valign=\"bottom\">");
			foreach (string currentKey in Page.Request.Form.AllKeys) {
				if (currentKey.IndexOf("TreeLevelSelectedIds") > -1) {
					st = Page.Request.Form.GetValues(currentKey);
					//if (st != null && st.Length > 0 && st[0].Length > 0) {
						selectedIds = st[0].ToString();
						tempArr = currentKey.Split('_');
						
						accessType = (UniverseAccessType)Int64.Parse(tempArr[1].ToString());
						
						nodeCss = (accessType == UniverseAccessType.includes) ? _parentNodeChildIncludeCss : _parentNodeChildExcludeCss;
						childNodeCss = (accessType == UniverseAccessType.includes) ? _childNodeIncludeCss : _childNodeExcludeCss;

						treeViewId = int.Parse(tempArr[0].ToString().Replace("TreeLevelSelectedIds", ""));

						//Root node
						if (!oldTreeViewId.Equals(tempArr[0].ToString())) {
							if (!first) {
								if (levelIds != null && levelIds.Length > 0) levelIds = levelIds.Substring(0, levelIds.Length - 1);
								nodeCurrentText = "<div > <a href=# onclick=customDeleteAllChild('" + levelIds + "'); class=\"" + _trashNodeCss + "\"><img src=\"" + _trashImage + "\" heigth=14px width=14px border=0>" + GetWebWord(_deleteAllTreeNodeTextCode, _siteLanguage) + "</a></div>";
								oTree.Add("root", "root0", nodeCurrentText, false);
								

								//Debut Boite  arbe element selectionnés  
								html.Append("<td valign=\"top\"><div id=\"divTreeBox_" + oldIntTreeViewId + "\" ><table cellpadding=\"0\" cellspacing=\"0\">");

								//Debut boutons Inclusion/exclure et Intresection								
								html.Append("<tr><td valign=\"bottom\"><table><tr><td width=\"90%\" align=\"left\"><a href=\"javascript:AddSelectedItemsToTree('" + listId + "','" + oldIntTreeViewId + "'," + _defaultBranchId + "," + oldAccessType.GetHashCode() + ");\"><img src=\"" + ((oldAccessType == UniverseAccessType.includes) ? _includeImage : _excludeImage) + "\" alt=\"" + GetWebWord(code, _siteLanguage) + "\" border=\"0\"/> </a></td>");
								html.Append("</tr></table></td></tr>");
								//Fin boutons Inclusion/exclure et Intersection

								//Debut Tableau cadre arbre
								html.Append("<tr><td valign=\"top\">");
								html.Append("<table class=\"" + treeFrameCss + "\" cellpadding=\"0\" cellspacing=\"0\">");
								html.Append("<tr class=\"" + treeFrameHeaderCss + "\"><td>&nbsp;" + GetWebWord(headerCode, _siteLanguage) + "</td><tr>");
								html.Append(" <tr class=\"" + treeFrameBodyCss + "\"> <td>");
								html.Append("<div id=\"universeTreeViewZone_" + oldIntTreeViewId.ToString() + "\">" + oTree.HTML() + "</div>" + inputHiddenFields);
								html.Append("</td></tr>");
								html.Append("</table>");
								html.Append("</td></tr>");								
								nbCurrentTree++;
								//Fin Tableau cadre arbre

								//Fin Boite  arbe element selectionés 
								html.Append("</table></div></td>");

								//Nouvelle ligne pour les arbres suivants
								if (nbCurrentTree > 0 && (nbCurrentTree < nbTotalTree) && (nbTotalTree > _nbMaxTreeByLine)
										&& (nbCurrentTree % _nbMaxTreeByLine) == 0)
									html.Append("</tr><tr valign=\"bottom\">");

							}
							oldLevelsId = new List<long>();
							
							oTree = new obout_ASPTreeView_2_NET.Tree();
							oTree.id = "treeViewId" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString();
							oTree.FolderIcons = _treeViewIcons;
							oTree.FolderScript = _treeViewScripts;
							oTree.FolderStyle = _treeViewStyles;
							oTree.ShowIcons = false;
							oTree.Height = _treeViewHeight;
							oTree.Width = _treeViewWidth;
							oTree.ShowRootPlusMinus = false;														
							inputHiddenFields = "";
							levelIds = "";
						}


						if (accessType == UniverseAccessType.includes) {
							code = _includeTextCode;
							headerCode = _includeElementsTextCode;
							treeFrameCss = _treeIncludeFrameCss;
							treeFrameBodyCss = _treeIncludeFrameBodyCss;
							treeFrameHeaderCss = _treeIncludeFrameHeaderCss;
						}
						else {
							code = _excludeTextCode;
							headerCode = _excludeElementsTextCode;
							treeFrameCss = _treeExcludeFrameCss;
							treeFrameBodyCss = _treeExcludeFrameBodyCss;
							treeFrameHeaderCss = _treeExcludeFrameHeaderCss;
						}

						//Parent level node
						levelId = Int64.Parse(tempArr[2].ToString());
						currentLevel = UniverseLevels.Get(levelId);												
						
						if (!oldLevelsId.Contains(levelId)) {
							nodeCurrentText = "<div class=\"" + nodeCss + "\">" + GetWebWord(currentLevel.LabelId, _siteLanguage) + "</div>";
							oTree.Add("root", "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString(), nodeCurrentText, false, null, null);
							levelIds = levelIds + "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString() + ",";
						}
						
						//Child node
						hiddenFieldIds = "";
						if (selectedIds != null && selectedIds.Length > 0 && selectedIds.Split(',').Length<_nbMaxItemByLevel) {
                            ds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetOneLevelItems(currentLevel.TableName, selectedIds, _webSession, _dBSchema, _dimension);
							if (ds != null && ds.Tables[0].Rows.Count > 0) {								
								foreach (DataRow dr in ds.Tables[0].Rows) {
									childId = treeViewId + "_" + currentLevel.ID + "_" + dr[0].ToString();
									nodeCurrentText = "<div class=" + childNodeCss + " nowrap>" + dr[1].ToString().ToLower();
									nodeCurrentText += "</td><td nowrap>&nbsp;<a href=# onclick=\"removeItem('TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "' , '" + dr[0].ToString() + "');ob_t2_Remove('" + childId + "');\" ><img src=\""+_deleteImage+"\" border=0></a></div>";
									hiddenFieldIds += dr[0].ToString() + ",";
									oTree.Add("t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString(), childId, nodeCurrentText, false, null, null);
								}
								
							}
						}

						if (hiddenFieldIds.Length > 0) 
							hiddenFieldIds = hiddenFieldIds.Substring(0, hiddenFieldIds.Length - 1);
						
						inputHiddenFields += "<input type=\"hidden\" id=\"TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "\" name=\"TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "\" value=\"" + hiddenFieldIds + "\">";
						

						oldTreeViewId = tempArr[0].ToString();
						oldLevelsId.Add(levelId);
						oldAccessType = accessType;
						oldIntTreeViewId = treeViewId;
						first = false;
					//}
				}
			}

			if (!first) {
				//levelIds = levelIds + "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString() + ",";
				if (levelIds != null && levelIds.Length > 0) levelIds = levelIds.Substring(0, levelIds.Length - 1);
				nodeCurrentText = "<div > <a href=# onclick=customDeleteAllChild('" + levelIds + "'); class=\"" + _trashNodeCss + "\"><img src=\"" + _trashImage + "\" heigth=14px width=14px border=0>" + GetWebWord(_deleteAllTreeNodeTextCode, _siteLanguage) + "</a></div>";
				oTree.Add("root", "root0", nodeCurrentText, false);				

				if (accessType == UniverseAccessType.includes) {
					code = _includeTextCode;
					headerCode = _includeElementsTextCode;
					treeFrameCss = _treeIncludeFrameCss;
					treeFrameBodyCss = _treeIncludeFrameBodyCss;
					treeFrameHeaderCss = _treeIncludeFrameHeaderCss;
				}
				else {
					code = _excludeTextCode;
					headerCode = _excludeElementsTextCode;
					treeFrameCss = _treeExcludeFrameCss;
					treeFrameBodyCss = _treeExcludeFrameBodyCss;
					treeFrameHeaderCss = _treeExcludeFrameHeaderCss;
				}

				//Debut Boite  arbe element selectionnés  
				html.Append("<td valign=\"top\"><div id=\"divTreeBox_" + oldIntTreeViewId + "\" ><table cellpadding=\"0\" cellspacing=\"0\">");

				//Debut boutons Inclusion/exclure et Intresection
				if (listId != null && listId.Length > 0) listId = listId.Substring(0, listId.Length - 1);
				html.Append("<tr><td valign=\"bottom\"><table><tr><td width=\"90%\" align=\"left\"><a href=\"javascript:AddSelectedItemsToTree('" + listId + "','" + oldIntTreeViewId + "'," + _defaultBranchId + "," + accessType.GetHashCode() + ");\"><img src=\"" + ((oldAccessType == UniverseAccessType.includes) ? _includeImage : _excludeImage) + "\" alt=\"" + GetWebWord(code, _siteLanguage) + "\" border=\"0\"/> </a></td>");
				html.Append("</tr></table></td></tr>");
				//Fin boutons Inclusion/exclure et Intersection

				//Debut Tableau cadre arbre
				html.Append("<tr><td valign=\"top\">");
				html.Append("<table class=\"" + treeFrameCss + "\" cellpadding=\"0\" cellspacing=\"0\">");
				html.Append("<tr class=\"" + treeFrameHeaderCss + "\"><td>&nbsp;" + GetWebWord(headerCode, _siteLanguage) + "</td><tr>");
				html.Append(" <tr class=\"" + treeFrameBodyCss + "\"> <td>");
				html.Append("<div id=\"universeTreeViewZone_" + oldIntTreeViewId.ToString() + "\">" + oTree.HTML() + "</div>" + inputHiddenFields);
				html.Append("</td></tr>");
				html.Append("</table>");
				html.Append("</td></tr>");

				nbCurrentTree++;
				//Fin Tableau cadre arbre

				//Fin Boite  arbe element selectionés 
				html.Append("</table></div></td>");

				//Nouvelle ligne pour les arbres suivants
				if (nbCurrentTree > 0 && (nbCurrentTree < nbTotalTree) && (nbTotalTree > _nbMaxTreeByLine)
						&& (nbCurrentTree % _nbMaxTreeByLine) == 0)
					html.Append("</tr><tr valign=\"bottom\">");

			}
			html.Append("</tr></table>");

		}


		/// <summary>
		/// Build Obout Html treeviw
		/// </summary>
		/// <param name="treeViewId">Treeview Id</param>
		/// <param name="levels">Levels associated</param>
		/// <param name="accessType">access Type (include or exclude)</param>
		/// <param name="groups">groups of nomenclature items</param>
		/// <returns>html treeview</returns>
		protected virtual string BuildUniverseLevelTreeView(int treeViewId, List<UniverseLevel> levels, UniverseAccessType accessType, TNS.Classification.Universe.NomenclatureElementsGroup groups) {
			try {
				DataSet ds = null;
				obout_ASPTreeView_2_NET.Tree oTree = new obout_ASPTreeView_2_NET.Tree();
				oTree.id = "treeViewId" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString();
				string nodeCurrentText = "";
				string levelIds = "";
				string childId = "";
				string hiddenFieldIds = "", inputHiddenFields = "";
				string nodeCss = (accessType == UniverseAccessType.includes) ? _parentNodeChildIncludeCss : _parentNodeChildExcludeCss;
				string childNodeCss = (accessType == UniverseAccessType.includes) ? _childNodeIncludeCss : _childNodeExcludeCss;

				List<long> oldLevelsId = new List<long>();

				foreach (UniverseLevel currentLevel in levels) {
					if (!oldLevelsId.Contains(currentLevel.ID)) {
						nodeCurrentText = "<div class=\"" + nodeCss + "\">" + GetWebWord(currentLevel.LabelId, _siteLanguage) + "</div>";
						oTree.Add("root", "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString(), nodeCurrentText, false, null, null);

						//Add saved childs
						hiddenFieldIds = "";
						if (groups != null && groups.Contains(currentLevel.ID)) {
                            ds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetOneLevelItems(currentLevel.TableName, groups.GetAsString(currentLevel.ID), _webSession, _dBSchema, _dimension);
							if (ds != null && ds.Tables[0].Rows.Count > 0) {								
								foreach (DataRow dr in ds.Tables[0].Rows) {
									childId = treeViewId + "_" + currentLevel.ID + "_" + dr[0].ToString();
									nodeCurrentText = "<div class=" + childNodeCss + " nowrap>" + dr[1].ToString().ToLower();
									nodeCurrentText += "</td><td nowrap>&nbsp;<a href=# onclick=\"removeItem('TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "' , '" + dr[0].ToString() + "');ob_t2_Remove('" + childId + "');\" ><img src=\""+_deleteImage+"\" border=0></a></div>";
									hiddenFieldIds += dr[0].ToString() + ",";
									oTree.Add("t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString(), childId, nodeCurrentText, false, null, null);
								}							
							}
						}
						oldLevelsId.Add(currentLevel.ID);
						levelIds = levelIds + "t" + treeViewId.ToString() + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID.ToString() + ",";
					}
					if (hiddenFieldIds.Length > 0) {
						hiddenFieldIds = hiddenFieldIds.Substring(0, hiddenFieldIds.Length - 1);
					}
					inputHiddenFields += "<input type=\"hidden\" id=\"TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "\" name=\"TreeLevelSelectedIds" + treeViewId + "_" + accessType.GetHashCode().ToString() + "_" + currentLevel.ID + "\" value=\"" + hiddenFieldIds + "\">";
					
				}

				if (levelIds != null && levelIds.Length > 0) levelIds = levelIds.Substring(0, levelIds.Length - 1);

				nodeCurrentText = "<div > <a href=# onclick=customDeleteAllChild('" + levelIds + "'); class=\"" + _trashNodeCss + "\"><img src=\"" + _trashImage + "\" heigth=14px width=14px border=0>" + GetWebWord(_deleteAllTreeNodeTextCode, _siteLanguage) + "</a></div>";
				oTree.Add("root", "root0", nodeCurrentText, false);
				oTree.FolderIcons = _treeViewIcons;
				oTree.FolderScript = _treeViewScripts;
				oTree.FolderStyle = _treeViewStyles;
				oTree.ShowIcons = false;
				oTree.Height = _treeViewHeight;
				oTree.Width = _treeViewWidth;
				oTree.ShowRootPlusMinus = false;
				return "<div id=\"universeTreeViewZone_" + treeViewId.ToString() + "\">" + oTree.HTML() + "</div>" + inputHiddenFields;
			}
			catch (System.Exception err) {
				throw (new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible de construire le Treeview Obout", err));
			}

		}

		/// <summary>
		/// Build Universe level treeview 
		/// </summary>
		/// <param name="html">html</param>
		/// <param name="treeviewId">treeview Id</param>
		/// <param name="accessType"> access Type</param>
		/// <param name="groups">groups in universe</param>
		/// <param name="listId">list id</param>
		/// <param name="nbCurrentTree">nb Current Tree</param>
		/// <param name="nbTotalTree">nb Total Tree</param>
		protected virtual void BuildUniverseLevelTreeView(StringBuilder html, ref int treeviewId, UniverseAccessType accessType, TNS.Classification.Universe.NomenclatureElementsGroup groups,  string listId, ref int nbCurrentTree, int nbTotalTree) {
			
			int code = 0, headerCode = 0;			
			string treeFrameCss = "", treeFrameBodyCss = "", treeFrameHeaderCss = "";

			if (accessType == UniverseAccessType.includes) {
				code = _includeTextCode;
				headerCode = _includeElementsTextCode;
				treeFrameCss = _treeIncludeFrameCss;
				treeFrameBodyCss = _treeIncludeFrameBodyCss;
				treeFrameHeaderCss = _treeIncludeFrameHeaderCss;
			}
			else {
				code = _excludeTextCode;
				headerCode = _excludeElementsTextCode;
				treeFrameCss = _treeExcludeFrameCss;
				treeFrameBodyCss = _treeExcludeFrameBodyCss;
				treeFrameHeaderCss = _treeExcludeFrameHeaderCss;
			}

			//Debut Boite  arbe element selectionnés  
			html.Append("<td valign=\"top\"><div id=\"divTreeBox_" + treeviewId + "\" ><table cellpadding=\"0\" cellspacing=\"0\">");

			//Debut boutons Inclusion/exclure et Intresection
			html.Append("<tr><td valign=\"bottom\"><table><tr><td width=\"90%\" align=\"left\"><a href=\"javascript:AddSelectedItemsToTree('" + listId + "','" + treeviewId + "'," + _defaultBranchId + "," + accessType.GetHashCode() + ");\"><img src=\"" + ((accessType == UniverseAccessType.includes) ? _includeImage : _excludeImage) + "\" alt=\"" + GetWebWord(code, _siteLanguage) + "\" border=\"0\"/> </a></td>");
			html.Append("</tr></table></td></tr>");
			//Fin boutons Inclusion/exclure et Intersection

			//Debut Tableau cadre arbre
			html.Append("<tr><td valign=\"top\">");
			html.Append("<table class=\"" + treeFrameCss + "\" cellpadding=\"0\" cellspacing=\"0\">");
			html.Append("<tr class=\"" + treeFrameHeaderCss + "\"><td>&nbsp;" + GetWebWord(headerCode, _siteLanguage) + "</td><tr>");
			html.Append(" <tr class=\"" + treeFrameBodyCss + "\"> <td>");
			html.Append(BuildUniverseLevelTreeView(treeviewId, _allowedLevels, accessType, groups));
			html.Append("</td></tr>");
			html.Append("</table>");
			html.Append("</td></tr>");
			treeviewId++;
			nbCurrentTree = treeviewId;
			//Fin Tableau cadre arbre

			//Fin Boite  arbe element selectionés 
			html.Append("</table></div></td>");

			//Nouvelle ligne pour les arbres suivants
			if (nbCurrentTree > 0 && (nbCurrentTree < nbTotalTree) && (nbTotalTree > _nbMaxTreeByLine)
					&& (nbCurrentTree % _nbMaxTreeByLine) == 0)
				html.Append("</tr><tr valign=\"bottom\">");
		}

		
		#endregion

		#region SetHiddenFields
		/// <summary>
		/// Set hidden fields
		/// </summary>
		/// <remarks>For each level (exlude ou include) a hidden field store selected iitems ids</remarks>
		/// <returns>html hidden fields</returns>
		protected override string SetHiddenFields() {			
			StringBuilder html = new StringBuilder(1000);
			
				if((_eventTarget !=FrameWorkSelection.eventSelection.LOAD_EVENT || _eventTarget != FrameWorkSelection.eventSelection.SAVE_EVENT)
					&& (_errorCode != FrameWorkSelection.error.SECURITY_EXCEPTION
					|| _errorCode != FrameWorkSelection.error.MAX_ELEMENTS
					|| _errorCode != FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE)
					)
					html.Append(base.SetHiddenFields());											
			return html.ToString();
		}

		#endregion
		
		#endregion

		#region Implementation of abstract methodS

		#region  GetWebWord
		/// <summary>
		/// Translate word
		/// </summary>
		/// <param name="code">word code</param>
		/// <param name="siteLanguage">site language</param>
		/// <returns>translate word</returns>
		protected override string GetWebWord(int code, int siteLanguage) {
			return GestionWeb.GetWebWord(code, siteLanguage);
		}
		#endregion

		#region  GetData
		/// <summary>
		/// Get data of hierarchical level associated with  selected ids of another level
		/// </summary>
		/// <param name="universeLevelId">level target</param>
		/// <param name="selectedItemIds">identifiers of items selected into one level</param>
		/// <param name="universeLevelOfSelectedItem">universe Level OfSelectedI tem</param>
		/// <returns>datatable like [id_item,item]</returns>
		protected override DataTable GetData(int universeLevelId, string selectedItemIds, int universeLevelOfSelectedItem) {
			try {
				_webSession = (WebSession)WebSession.Load(_idSession);
				return TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetItems(UniverseLevels.Get(universeLevelId).TableName, selectedItemIds, UniverseLevels.Get(universeLevelOfSelectedItem).TableName, _webSession, _dBSchema, _dimension).Tables[0];
			}
			catch (Exception err) {
				throw new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible d'obtenir les données.", err);
			}
		}

		///<summary>
		/// Get data word like keyWord parameter
		/// </summary>
		/// <param name="universeLevelId">universe Level Id</param>
		/// <param name="wordToSearch">word to search</param>
		/// <returns></returns>
		protected override DataTable GetData(int universeLevelId, string wordToSearch) {
			try {
				_webSession = (WebSession)WebSession.Load(_idSession);
				return TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetItems(UniverseLevels.Get(universeLevelId).TableName, wordToSearch, _webSession, _dBSchema, _dimension).Tables[0];
			}
			catch (Exception err) {
				throw new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible d'obtenir les données.", err);
			}
		}
		#endregion

		#region OnAjaxMethodError
		/// <summary>
		/// Appelé sur erreur à l'exécution des méthodes Ajax
		/// </summary>
		/// <param name="errorException">Exception</param>
		/// <returns>Message d'erreur</returns>
		protected override string OnAjaxMethodError(Exception errorException) {
			WebSession customerSession = (WebSession)WebSession.Load(_idSession);
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
			try {
				BaseException err = (BaseException)errorException;
				cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
			}
			catch (System.Exception) {
				try {
					cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
				}
				catch (System.Exception es) {
					throw (es);
				}
			}
			cwe.SendMail();		
			return GetMessageError(customerSession, 1973);
		}
		#endregion


		#endregion


		
	}
}