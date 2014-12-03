

#region Informations
// Auteur: D. Mussuma 
// Date de création: 03/12/2008 
// Date de modification: 
#endregion

using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using RightConstantes = TNS.AdExpress.Constantes.Customer.Right;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
namespace TNS.AdExpress.Web.Controls.Selections {
	/// <summary>
	/// Show list of  Parent/media of one vehicle to select with corresponding rights.
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:GenericDetailVehicleSelectionWebControl runat=server></{0}:GenericDetailVehicleSelectionWebControl>")]
	public class GenericDetailVehicleSelectionWebControl : System.Web.UI.WebControls.CheckBoxList {

		#region Variables
		/// <summary>
		/// Session of client
		/// </summary>
		protected WebSession _webSession = null;
		/// <summary>
		/// Data set 
		/// </summary>
		protected DataSet _dsListMedia = null;
		/// <summary>
		/// Event 
		/// </summary>
		protected int _eventButton;
		/// <summary>
		/// Key word
		/// </summary>
		protected string _keyWord = "";
		/// <summary>
		/// Define the validity of number of items selected
		/// <example>
		/// -> 2 : None items selected
		/// -> 4 : Too much items selected 
		/// </example>
		/// </summary>
		protected int _nbItemsValidity = -1;
		/// <summary>
		/// List of media already selected
		/// </summary>
		protected string _listAccessMedia = "";
		/// <summary>
		/// Is listeMedia empty
		/// </summary>
		protected static bool _isEmptyList = false;
		/// <summary>
		/// Verify is it possible to load a saved universe
		/// </summary>
		protected static bool _canLoadUnivese = true;
		/// <summary>
		/// selected media
		/// </summary>
		protected  List<Int64> _currentSelectedMediaList = null;

		/// <summary>
		///  media alreay selected
		/// </summary>
		protected List<Int64> _oldSelectedMediaList = null;

	
		#endregion

		#region Constantes
		const int MAX_SELECTABLE_ELEMENTS = 200;
		#endregion

		#region Accessors
		/// <summary>
		/// Set Session of client
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set { _webSession = value; }
		}
		/// <summary>
		/// Set current event
		/// </summary>
		public virtual int EventButton {
			set { _eventButton = value; }
		}		
		/// <summary>
		/// Get / Set Key word
		/// </summary>
		public string KeyWord {
			get { return _keyWord; }
			set { _keyWord = value; }
		}
		
		/// <summary> 
		/// Get if listeMedia is empty
		/// </summary>
		[Bindable(true),
		Description("bool iste vide")]
		public static bool IsEmptyList {
			get { return _isEmptyList; }
			set { _isEmptyList = value; }
		}

		/// <summary>
		/// Get /Set the validity of number of items selected
		/// <example>
		/// -> 2 : None items selected
		/// -> 4 : Too much items selected 
		/// </example>
		/// </summary>
		public int NbItemsValidity {
			get { return _nbItemsValidity; }
			set { _nbItemsValidity = value; }
		}
		/// <summary> 
		/// Get if it's possible to load a saved universe
		/// </summary>
		[Bindable(true),
	    Description("Get if it's possible to load a saved universe")]
		public static bool CanLoadUnivese {
			get { return _canLoadUnivese; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public GenericDetailVehicleSelectionWebControl(): base() {
			this.EnableViewState=true;
		}
		#endregion

		#region Events

		#region OnLoad
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">Event Arguments</param>
		protected override void OnLoad(EventArgs e) {

			_currentSelectedMediaList = null; _oldSelectedMediaList = null;
			bool canSaveMedias = true;
			int nbAllSelectedItems = 0;

			#region Get current selected medias
			bool sel = false;
			foreach (System.Web.UI.WebControls.ListItem currentItem in this.Items) {
					if (currentItem.Selected) {
						if(!sel)_currentSelectedMediaList = new List<long>();
						if (!_currentSelectedMediaList.Contains(Int64.Parse(currentItem.Value))) 
							_currentSelectedMediaList.Add(Int64.Parse(currentItem.Value));
						sel = true;
					}
			}
			#endregion

			#region Get old selected medias
			int j=1;
			_oldSelectedMediaList = new List<long>();
			while(j<=_webSession.CompetitorUniversMedia.Count){
                System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[j];
                foreach (System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes)
                {
					if(!_oldSelectedMediaList.Contains(((LevelInformation)item2.Tag).ID))
					_oldSelectedMediaList.Add(((LevelInformation)item2.Tag).ID);
				}			
				j++;
			}
			#endregion

			#region Can Save Medias 
			//Cheks if medias have been already selected
			if (_currentSelectedMediaList != null && _currentSelectedMediaList.Count > 0) {
				foreach (Int64 idMedia in _currentSelectedMediaList) {
					if (_oldSelectedMediaList != null && _oldSelectedMediaList.Count > 0) {
						if (_oldSelectedMediaList.Contains(idMedia)) {
							canSaveMedias = false; break;
						}						
					}
					if (!canSaveMedias) break;
				}
			}
			if (!canSaveMedias) {
				_nbItemsValidity = constEvent.error.MEDIA_SELECTED_ALREADY;
			}				
			#endregion

			// Trim keyWord
			_keyWord = _keyWord.Trim();

			#region limit max number of items to selected
			//limit max number of items to selected 
			nbAllSelectedItems = ((_currentSelectedMediaList != null && _currentSelectedMediaList.Count > 0) ? _currentSelectedMediaList.Count : 0)
				+ ((_oldSelectedMediaList != null && _oldSelectedMediaList.Count > 0) ? _oldSelectedMediaList.Count : 0);

			if (nbAllSelectedItems >= MAX_SELECTABLE_ELEMENTS
				&& (_eventButton == constEvent.eventSelection.OK_EVENT
				|| _eventButton == constEvent.eventSelection.VALID_EVENT
				|| _eventButton == constEvent.eventSelection.NEXT_EVENT)) {
				_nbItemsValidity = constEvent.error.MAX_ELEMENTS;
				canSaveMedias = false;
			}
			#endregion			

			#region Verify if can load data from save universe (Pour Bouton CHARGEMENT UNIVERS)
			//Verify if can load data form save universe
			_canLoadUnivese = VerifyDataFromUniverse(_eventButton, _keyWord);
			if (_eventButton == constEvent.eventSelection.LOAD_EVENT) {
				_canLoadUnivese = VerifyDataFromUniverse();
				if (!_canLoadUnivese) {
					_nbItemsValidity = constEvent.error.LOAD_NOT_POSSIBLE;
				}
			}
			#endregion

			#region Load items from data saved in web session (Pour Bouton CHARGEMENT UNIVERS)
			if (_eventButton == constEvent.eventSelection.LOAD_EVENT) {
				if (_currentSelectedMediaList == null) _currentSelectedMediaList = new List<long>();
				else _currentSelectedMediaList.Clear();
				foreach (System.Windows.Forms.TreeNode currentNode in _webSession.CurrentUniversMedia.Nodes) {
					
					if (!_currentSelectedMediaList.Contains(((LevelInformation)currentNode.Tag).ID)) {
						_currentSelectedMediaList.Add(((LevelInformation)currentNode.Tag).ID);
						_isEmptyList = false;
					}
				}
				if (_currentSelectedMediaList != null) {
					_listAccessMedia = "";
					for (int i = 0; i < _currentSelectedMediaList.Count; i++) {
						_listAccessMedia += _currentSelectedMediaList[i].ToString() + ",";
					}
					if (_listAccessMedia.Length > 0) _listAccessMedia = _listAccessMedia.Substring(0, _listAccessMedia.Length - 1);
				}
			}

			#endregion

			#region  Keep current items selected during search
			if ( _eventButton == constEvent.eventSelection.OK_EVENT) {								
				if (_currentSelectedMediaList != null) {
					_listAccessMedia = "";
					for (int i = 0; i < _currentSelectedMediaList.Count; i++) {
						_listAccessMedia += _currentSelectedMediaList[i].ToString() + ",";
					}
				}
				List<Int64> alreadySelectedMedia = GetAlreadySelectedMedia();
				if (alreadySelectedMedia != null) {
					for (int i = 0; i < alreadySelectedMedia.Count; i++) {
						_listAccessMedia += alreadySelectedMedia[i].ToString() + ",";
					}
				}
				if (_listAccessMedia!=null && _listAccessMedia.Length > 0) _listAccessMedia = _listAccessMedia.Substring(0, _listAccessMedia.Length - 1);
			}
			#endregion

			//Load data from DB
			if (_eventButton != constEvent.eventSelection.SAVE_EVENT) 
			_dsListMedia = GetData(_eventButton, _keyWord, _listAccessMedia);
		
			#region Save selected items in WebSession
			if (canSaveMedias && CanCreateTreeNode()) {
				_webSession.CurrentUniversMedia = CreateTreeNode();
				_webSession.Save();
			}
			#endregion

			#region Add items from data saved in web session (Pour Bouton SAUVEGARDE UNIVERS)
			if (_eventButton == constEvent.eventSelection.SAVE_EVENT) {
				_listAccessMedia = "";
				if (_currentSelectedMediaList == null) _currentSelectedMediaList = new List<long>();
				else _currentSelectedMediaList.Clear();
				foreach (System.Windows.Forms.TreeNode currentNode in _webSession.CurrentUniversMedia.Nodes) {					
					if (!_currentSelectedMediaList.Contains(((LevelInformation)currentNode.Tag).ID)) {					
						_currentSelectedMediaList.Add(((LevelInformation)currentNode.Tag).ID);
						_isEmptyList = false;						
						_listAccessMedia += ((LevelInformation)currentNode.Tag).ID.ToString() + ",";
					}					
				}
				if (_listAccessMedia.Length > 0) _listAccessMedia = _listAccessMedia.Substring(0, _listAccessMedia.Length - 1);
				_dsListMedia = GetData(_eventButton, _keyWord, _listAccessMedia);

			}
			#endregion

			#region Scripts
			// Open/Close all Div
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ExpandColapseAllDivs", TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
			}
			// Open/Close parent Div
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}			
			//Sélection de tous les fils (SelectAllNotDisabledChilds)
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllNotDisabledChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SelectAllNotDisabledChilds", TNS.AdExpress.Web.Functions.Script.SelectAllNotDisabledChilds());
			}
			#endregion			

			#region Add Items
			int start = 0;
			if (CanAddItemsFromDataBase()) {
				this.Items.Clear();		
				DetailLevelItemInformation detailLevelItemInformation = null;
				TNS.AdExpress.Domain.Level.GenericDetailLevel genericDetailLevel = _webSession.GenericMediaSelectionDetailLevel;
				if (genericDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.media)) {
					int indexL = genericDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.media);
					if (indexL > 0) detailLevelItemInformation = genericDetailLevel[indexL];
				}
				if (_dsListMedia != null && detailLevelItemInformation != null) {
					foreach (DataRow currentRow in _dsListMedia.Tables[0].Rows) {
						System.Web.UI.WebControls.ListItem checkBox1 = new System.Web.UI.WebControls.ListItem();
						checkBox1.Text = currentRow[detailLevelItemInformation.GetSqlFieldWithoutTablePrefix()].ToString();
						checkBox1.Value =  currentRow[detailLevelItemInformation.GetSqlFieldIdWithoutTablePrefix()].ToString();
						if (_currentSelectedMediaList != null && _currentSelectedMediaList.Count > 0 && _currentSelectedMediaList.Contains(Int64.Parse(currentRow[detailLevelItemInformation.GetSqlFieldIdWithoutTablePrefix()].ToString())))
							checkBox1.Selected = true;
						this.Items.Add(checkBox1);
					}
				}
			}
			#endregion

			#region Initialisation of items list
			// Initialisation of items list
			if (_eventButton == constEvent.eventSelection.INITIALIZE_EVENT 
				|| _eventButton == constEvent.eventSelection.ALL_INITIALIZE_EVENT) {
				_currentSelectedMediaList = null;
				if (_eventButton == constEvent.eventSelection.ALL_INITIALIZE_EVENT) {
					_webSession.CompetitorUniversMedia.Clear();
					_webSession.Save();
					_canLoadUnivese = true;
				}
			}
			#endregion

			#region Next Label
			if ((_dsListMedia != null && _dsListMedia.Tables[0].Rows.Count > 0) || (_webSession.CompetitorUniversMedia != null && _webSession.CompetitorUniversMedia.Count > 0)) {
				_isEmptyList = false;
			}
			else _isEmptyList = true;
			#endregion
		}
		#endregion

		#endregion

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			
			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(10000);
			string textOpenclose = "", checkBox = "", allDivIds = "", disabled = "", displayDiv = "none", cssTextItem = "txtViolet10",  cssL1="violetBackGroundV3"; 
			int insertIndex = 0, nbLevels = 0, numColumn = 0, displayIndex = 0, counter = 0;
			int startL1 = -1, startL2 = -1, startL3 = -1;	
			Int64 i = 0, idL1 = -2, oldIdL1 = -1,idL1Div = -2, idL2 = -2, oldIdL2 = -1, idL3 = -2, oldIdL3 = -1;
			string allIdL1 = "", allIdL2 = "", allIdL3 = "";
			string labelL1 = "", labelL2 = "", labelL3 = "";
			bool isNewL1 = false, isNewL2 = false, isNewL3 = false;
			TNS.AdExpress.Domain.Level.GenericDetailLevel genericDetailLevel = null;
			DetailLevelItemInformation currentLevel = null;
			#endregion

			#region Show Messages
			t.Append(GetMessage());			
			#endregion

			#region  Render with items loaded from DataBase
			if (CanShowData()) {

				if(!_isEmptyList)textOpenclose = GestionWeb.GetWebWord(2461, _webSession.SiteLanguage);
				
				if (_webSession.GenericMediaSelectionDetailLevel != null) {
					genericDetailLevel = _webSession.GenericMediaSelectionDetailLevel;
					nbLevels = genericDetailLevel.GetNbLevels;
				}

				//Start Global Table
				//t.Append(" <table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
				#region Debut Tableau global
				t.Append("<tr ><td  vAlign=\"top\"><table vAlign=\"top\" width=\"100%\">");
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
				insertIndex = t.Length;
				t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;" + textOpenclose + "</a>");
				t.Append("<tr><td vAlign=\"top\">");
				//t.Append("<DIV id=selectAllSlogans>");//Ouverture calque permettant de sélectionner tous les éléménts
				#endregion

				if (nbLevels > 0) {
					
					//Start render of All lines contents
					List<Int64> alreadySelectedMedia = GetAlreadySelectedMedia();

					foreach (DataRow currentRow in _dsListMedia.Tables[0].Rows) {

						counter += 1;
						//render html for each Level
						for (int lev = 1; lev <= nbLevels; lev++) {

							//Init ids parents
							currentLevel = genericDetailLevel[lev];
							if (lev == 1) {
								idL1 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL1 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
							if (lev == 2) {
								idL2 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL2 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
							if (lev == 3) {
								idL3 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL3 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
						}

						if (counter == 1) {
							idL1Div = idL1;
						}
						//to maintain the state of the list 
						if (idL1Div != idL1) {
							t.Insert(displayIndex, displayDiv);
							displayDiv = "none";
							idL1Div = idL1;
						}

						#region Closing Level 2
						if (nbLevels > 2 && ((idL2 != oldIdL2 && startL2 == 0) || (oldIdL1 != idL1 && startL1 == 0))) {
							if (numColumn == 1) {
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								t.Append("</tr>");
							}
							else if (numColumn == 2) {
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								t.Append("</tr>");
							}
							numColumn = 0;
							t.Append("</table></div></td></tr></table></td></tr>");
						}
						#endregion

						#region Closing Level 1
						if ((oldIdL1 != idL1 && startL1 == 0)) {
							if (nbLevels == 2) {
								if (numColumn == 1) {
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
									t.Append("</tr>");
								}
								else if (numColumn == 2) {
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
									t.Append("</tr>");
								}
								numColumn = 0;
							}
							startL1 = -1;
							oldIdL2 = -1;
							t.Append("</table></div></td></tr></table>");
						}
						#endregion
					
						#region New Level 1
						if (idL1 > -1 && idL1 != oldIdL1) {
							//border top table
							if (oldIdL1 == -1) t.Append("<table class=\"violetBorder txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "');\" style=\"cursor : pointer\">");
							else t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "');\" style=\"cursor : pointer\">");
							oldIdL1 = idL1;
							startL1 = 0;
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">&nbsp;&nbsp;&nbsp;" + labelL1);
							t.Append("</td>");
							t.Append("<td align=\"right\" class=\"arrowBackGround\"></td></tr>");
							t.Append("<tr><td colspan=\"2\"><div style=\"MARGIN-LEFT: 0px; DISPLAY:");
							displayIndex = t.Length;
							if (nbLevels == 2) cssL1 = "mediumPurple1";
							t.Append(";\" class=\""+cssL1+"\" id=\"1_" + idL1 + "\">");

							t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//link for select all items
							t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllNotDisabledChilds('1_" + idL1 + "')\" title=\"" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[2]), _webSession.SiteLanguage) + "\" class=\"roll04\">" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[2]), _webSession.SiteLanguage) + "</a></td></tr>");

							allDivIds = allDivIds + "1_" + idL1 + ",";
							//isNewL1 = false;
							numColumn = 0;
						}
						#endregion

						#region New Level 2 ( if nbLevels > 2)
						if (nbLevels > 2 && (idL2 > -1 && idL2 != oldIdL2)) {
							if (startL2 == -1) t.Append("<tr><td ><table class=\"violetBackGroundV3 txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "2_" + idL2 + "');\" style=\"cursor : pointer\">");
							else t.Append("<tr><td ><table class=\"violetBackGroundV3 violetBorderTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "2_" + idL2 + "');\" style=\"cursor : pointer\">");
							oldIdL2 = idL2;
							startL2 = 0;
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
							t.Append("&nbsp;&nbsp;" + labelL2 + "</td>");
							t.Append("<td align=\"right\" class=\"arrowBackGround\"></td></tr>");

							t.Append("<tr><td colspan=\"2\"><DIV style=\"MARGIN-LEFT: 5px\" class=\"mediumPurple1\" id=\"1_" + idL1 + "2_" + idL2 + "\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//link for select all items
							t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllNotDisabledChilds('1_" + idL1 + "2_" + idL2 + "')\" title=\"" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[3]), _webSession.SiteLanguage) + "\" class=\"roll04\">" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[3]), _webSession.SiteLanguage) + "</a></td></tr>");
							//isNewL2 = false;
							numColumn = 0;
						}
						#endregion

						#region New Level 3
						if (nbLevels == 2) {
							labelL3 = labelL2;
							idL3 = idL2;
						}

						checkBox = "";
						disabled = "";
						if (_currentSelectedMediaList != null && _currentSelectedMediaList.Count > 0) {
							foreach (long item1 in _currentSelectedMediaList) {
								if (item1 == idL3) {
									checkBox = "checked";
									break;
								}
							}
						}					
						
						if (alreadySelectedMedia.Contains(idL3)) {
							disabled = "disabled";
							checkBox = "checked";
							displayDiv = "''";
						}

						if (numColumn == 0) {
							cssTextItem = int.Parse(currentRow["activation"].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";
							t.Append("<tr>");
							t.Append("<td class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " name=\"GenericDetailVehicleSelectionWebControl1$" + i + "\" id=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + "><label for=\"GenericDetailVehicleSelectionWebControl1_	" + i + "\">" + labelL3 + "<br></label>");
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else if (numColumn == 1) {
							cssTextItem = int.Parse(currentRow["activation"].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";
							t.Append("<td  class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " name=\"GenericDetailVehicleSelectionWebControl1$" + i + "\" id=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + "><label for=\"GenericDetailVehicleSelectionWebControl1_	" + i + "\">" + labelL3 + "<br></label>");
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else {
							cssTextItem = int.Parse(currentRow["activation"].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";
							t.Append("<td class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " name=\"GenericDetailVehicleSelectionWebControl1$" + i + "\" id=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + "><label for=\"GenericDetailVehicleSelectionWebControl1_	" + i + "\">" + labelL3 + "<br></label>");
							t.Append("</td></tr>");
							numColumn = 0;
							i++;
						}
						oldIdL3 = idL3;

						//To maintain the state of the list when its the last div as it wont be handeled by the first check
						if (idL1Div == idL1 && _dsListMedia!=null && _dsListMedia.Tables[0].Rows.Count == counter) {
							t.Insert(displayIndex, displayDiv);
							displayDiv = "none";
							idL1Div = idL1;
						}
						#endregion

						//isNewL1 = false;
						//isNewL2 = false;
					}
					if (allDivIds.Length > 0) {
						allDivIds = allDivIds.Remove(allDivIds.Length - 1, 1);
						t.Insert(insertIndex, allDivIds);
					}

					//End render of All lines contents
				}

				//End Global Table
				#region Fermeture Tableau global

				if (numColumn == 1) {
					t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					t.Append("</tr>");
				}
				else if (numColumn == 2) {
					t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					t.Append("</tr>");
				}

				if (nbLevels > 2) t.Append("</table></div></td></tr></table></td></tr>");
				t.Append("</table></div></td></tr></table>");//Close Level 1
				t.Append("   </td></tr></table></td>");
				//output.Write(" </DIV>");//Fermeture calque permettant de sélectionner tous les éléménts
				t.Append("</tr>");

				#endregion
			}
			#endregion

			output.Write(t.ToString());
		}
		#endregion

		#region Private methods

		#region GetData
		/// <summary>
		/// Get list of media to show
		/// </summary>
		/// <param name="eventButton">event</param>
		/// <param name="keyWord">Key Word</param>
		/// <param name="listAccessMedia">media access list</param>
		/// <returns>List of media to show</returns>
		private DataSet GetData(int eventButton, string keyWord, string listAccessMedia) {
			IClassificationDAL classficationDAL = null;
			CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
			if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));

			object[] param = null;
			if (_eventButton == constEvent.eventSelection.OK_EVENT && _nbItemsValidity != constEvent.error.MAX_ELEMENTS) {
				param = new object[3];
				param[0] = _webSession;
				param[1] = _webSession.GenericMediaSelectionDetailLevel;
				param[2] = listAccessMedia;				
				classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
				return classficationDAL.GetDetailMedia(keyWord);

			}
			else if (_eventButton == constEvent.eventSelection.LOAD_EVENT || _eventButton == constEvent.eventSelection.SAVE_EVENT) {
				param = new object[3];
				param[0] = _webSession;
				param[1] = _webSession.GenericMediaSelectionDetailLevel;
				param[2] = listAccessMedia;
			}
			else {
				param = new object[3];
				param[0] = _webSession;
				param[1] = _webSession.GenericMediaSelectionDetailLevel;
				param[2] = "";
			}
			classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
			return classficationDAL.GetDetailMedia();

			//if (_eventButton == constEvent.eventSelection.OK_EVENT && _nbItemsValidity != constEvent.error.MAX_ELEMENTS)
			//    return Core.DataAccess.DetailMediaDataAccess.keyWordDetailMediaListDataAccess(_webSession, keyWord, listAccessMedia, _webSession.GenericMediaSelectionDetailLevel);
			//else if (_eventButton == constEvent.eventSelection.LOAD_EVENT || _eventButton == constEvent.eventSelection.SAVE_EVENT)
			//    return Core.DataAccess.DetailMediaDataAccess.DetailMediaListDataAccess(_webSession, _webSession.GenericMediaSelectionDetailLevel,listAccessMedia);
			//else
			//    return Core.DataAccess.DetailMediaDataAccess.DetailMediaListDataAccess(_webSession, _webSession.GenericMediaSelectionDetailLevel);
		}
		#endregion

		#region Events conditions
		/// <summary>
		/// Get if items must be loaded from data base
		/// </summary>
		/// <returns>True if items should be loaded from data base</returns>
		private bool LoadItemsFromDataBase() {
			if (_eventButton == constEvent.eventSelection.OK_EVENT
				|| (!Page.IsPostBack && _eventButton != constEvent.eventSelection.OK_POP_UP_EVENT)
				|| _eventButton == constEvent.eventSelection.NEXT_EVENT
				|| _eventButton == constEvent.eventSelection.OK_OPTION_MEDIA_EVENT
				|| _eventButton == constEvent.eventSelection.INITIALIZE_EVENT
				|| _eventButton == constEvent.eventSelection.ALL_INITIALIZE_EVENT) {
				return true;
			}
			//return false;
			return true; //tests à enlever
			
		
		}
		/// <summary>
		/// Get if items can be shown
		/// </summary>
		/// <returns>if items can be shown</returns>
		private bool CanShowData() {
			if (_isEmptyList) return false;
			if (!IsMediaFound(_keyWord, _eventButton, _isEmptyList)) return false; 
			switch (_eventButton) {
				case constEvent.eventSelection.LOAD_EVENT :
					return _canLoadUnivese;
				case constEvent.eventSelection.SAVE_EVENT :
					return true;
				default :  return LoadItemsFromDataBase();
			}
		}
			/// <summary>
		/// Get if items must be added from data base
		/// </summary>
		/// <returns>True if items should be loaded from data base</returns>
		private bool CanAddItemsFromDataBase() {
			if (_eventButton == constEvent.eventSelection.OK_EVENT
				|| !Page.IsPostBack
				|| _eventButton == constEvent.eventSelection.INITIALIZE_EVENT
				|| _eventButton == constEvent.eventSelection.ALL_INITIALIZE_EVENT
				|| _eventButton == constEvent.eventSelection.NEXT_EVENT
				|| _eventButton == constEvent.eventSelection.OK_OPTION_MEDIA_EVENT
				|| _eventButton == constEvent.eventSelection.LOAD_EVENT
				|| _eventButton == constEvent.eventSelection.SAVE_EVENT)//ADDED the 11/12/09
				return true;
				return false;
		}
		/// <summary>
		/// Get if items treenode can be created
		/// </summary>
		/// <returns>True if items items treenode can be created</returns>
		private bool CanCreateTreeNode() {
			if (_eventButton == constEvent.eventSelection.NEXT_EVENT
				|| _eventButton == constEvent.eventSelection.VALID_EVENT
				|| _eventButton == constEvent.eventSelection.SAVE_EVENT)
				return true;
			return false;
		}
		#endregion

		#region IsMediaFound
		/// <summary>
		/// Get if media have been found 
		/// </summary>
		/// <param name="keyWord">key Word</param>
		/// <param name="eventButton">Event</param>
		/// <param name="isEmptyList">True if is not empty</param>
		/// <returns>True if media have been found</returns>
		private bool IsMediaFound(string keyWord, int eventButton, bool isEmptyList) {
			if ((keyWord.Length < 2 && keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT)
			|| (!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord) && keyWord.Length > 0 && eventButton == constEvent.eventSelection.OK_EVENT && isEmptyList)
			|| (_dsListMedia == null || (_dsListMedia != null && _dsListMedia.Tables[0].Rows.Count == 0) || isEmptyList)
				)
				return false;

			return true;
		}
		#endregion

		#region GetWebWordCode
		/// <summary>
		/// Get web word code
		/// </summary>
		/// <param name="currentLevel">DetailLevelI tem Information</param>
		/// <returns>web word code</returns>
		private int GetWebWordCode(DetailLevelItemInformation currentLevel) {
			int code = 0;
			switch (currentLevel.Id) {
				case DetailLevelItemInformation.Levels.media:
					code = 1066;
					break;
				case DetailLevelItemInformation.Levels.category:
					code = 1151;
					break;
				case DetailLevelItemInformation.Levels.interestCenter:
					code = 2547;
					break;
				case DetailLevelItemInformation.Levels.mediaSeller:
					code = 2548;
					break;
				case DetailLevelItemInformation.Levels.title:
					code = 2546;
					break;
				case DetailLevelItemInformation.Levels.basicMedia:
					code = 2549;
					break;
				case DetailLevelItemInformation.Levels.country:
					code = 2550;
					break;
			}
			return code;
		}
		#endregion

		#region CreateTreeNode
		/// <summary>
		/// Creation of treenode with media items selected
		/// </summary>
		/// <returns></returns>
		protected System.Windows.Forms.TreeNode CreateTreeNode() {
			System.Windows.Forms.TreeNode mediaTree = new System.Windows.Forms.TreeNode();
			System.Windows.Forms.TreeNode tmpNode = null;
			// Nbre de cases cochés
			int compteurChild = 0;

			foreach (System.Web.UI.WebControls.ListItem currentItem in this.Items) {
				#region foreach
				//string[] tabParent = currentItem.Text.Split('_');
				//if (tabParent[0] == "Children" && currentItem.Selected) {
				if (currentItem.Selected) {
					tmpNode = new System.Windows.Forms.TreeNode(currentItem.Text);
					tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess, Int64.Parse(currentItem.Value), currentItem.Text);
					tmpNode.Checked = true;
					mediaTree.Nodes.Add(tmpNode);
					compteurChild++;
				}
				#endregion
			}

			// Fournit une valeur à nbElement pour vérifier la validité du nombre 
			// d'éléments sélectionné
			if (compteurChild == 0) {
				_nbItemsValidity = constEvent.error.CHECKBOX_NULL;
			}
			else if (compteurChild >= MAX_SELECTABLE_ELEMENTS) {
				_nbItemsValidity = constEvent.error.MAX_ELEMENTS;
			}

			return mediaTree;

		}
		#endregion

		#region Can Load Universe
		/// <summary>
		/// Verify if data can be loaded from saved universe
		/// </summary>
		/// <returns>True if data can be loaded form saved universe</returns>
		protected bool VerifyDataFromUniverse() {
			int j = 1;
			bool loadOk = true;
			List<Int64>  oldSelectedMediaList = new List<long>();

			while (j <= _webSession.CompetitorUniversMedia.Count) {

				//Get all items already saved in websession
				System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[j];
				foreach (System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes) {
					if (!oldSelectedMediaList.Contains(((LevelInformation)item2.Tag).ID)) oldSelectedMediaList.Add(((LevelInformation)item2.Tag).ID);
				}
				j++;

				//Verify in one of current selected items are already save in web session
				foreach (System.Windows.Forms.TreeNode item2 in _webSession.CurrentUniversMedia.Nodes) {
					if (oldSelectedMediaList.Contains(((LevelInformation)item2.Tag).ID)) {
						loadOk = false; break;
					}					
				}
			}
			return loadOk;
		}
		/// <summary>
		/// Verify if data can be loaded form saved universe
		/// </summary>
		/// <param name="eventButton">Event button</param>
		/// <param name="keyWord">keyWord</param>
		/// <returns>True if data can be loaded form saved universe</returns>
		protected bool VerifyDataFromUniverse(int eventButton, string keyWord) {
			bool loadData = false;
			if (Page.IsPostBack && eventButton != constEvent.eventSelection.INITIALIZE_EVENT 
				&& eventButton != constEvent.eventSelection.ALL_INITIALIZE_EVENT) {
				if (eventButton == constEvent.eventSelection.OK_OPTION_MEDIA_EVENT
					|| (eventButton == constEvent.eventSelection.OK_OPTION_MEDIA_EVENT 
					&& (keyWord.Length >= 2 || keyWord.Length == 0 || TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord)))
					)
					loadData = true;
			}
			return loadData;
		}
		#endregion

		#region Message
		/// <summary>
		/// Get Message to show
		/// <example>Error message</example>
		/// </summary>
		/// <returns>Message</returns>
		protected string GetMessage() {
			System.Text.StringBuilder t = new System.Text.StringBuilder(3000);
			#region  Message d'erreurs
			// Message d'erreur : veuillez saisir 2 caractères minimums
			if (_keyWord.Length < 2 && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT) {
				t.Append("<tr class=\"whiteBackGround\" ><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(1473, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
				t.Append("</td>");
				t.Append("</tr>");				
			}
			// Message d'erreur : mot incorrect
			else if (!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(_keyWord) && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT && _isEmptyList) {
				t.Append("<tr><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(1088, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
				t.Append("</td>");
				t.Append("</tr>");				
			}
			// Message d'erreur : aucun résultat avec le mot clé
			else if (_dsListMedia != null) {
				if (_dsListMedia.Tables[0].Rows.Count == 0 || _isEmptyList) {
					t.Append("<tr><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" " + GestionWeb.GetWebWord(819, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
					t.Append("</td>");
					t.Append("</tr>");					
				}
			}
			#endregion

			// Message d'erreur : Chargement de l'univers impossible
			if (!_canLoadUnivese && _eventButton == constEvent.eventSelection.LOAD_EVENT) {
				t.Append("<tr class=\"whiteBackGround\" ><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(1086, _webSession.SiteLanguage) + "</p> ");
				t.Append(" </td> ");
				t.Append(" </tr> ");				
			}

			return t.ToString();
		}
		#endregion

		#region Get already selected media
		/// <summary>
		/// Get already selected media
		/// </summary>
		/// <returns>Media list</returns>
		private List<Int64> GetAlreadySelectedMedia() {
			List<Int64> mediaList = new List<long>();
			int j = 1;
			while (j <= _webSession.CompetitorUniversMedia.Count) {
				System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[j];
				foreach (System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes) {
					mediaList.Add(((LevelInformation)item2.Tag).ID);					
				}
				j++;
			}
			return mediaList;
		}
		#endregion

		#endregion
	}
}
