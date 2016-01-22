#region Informations
// Auteur: D. Mussuma 
// Date de création: 12/12/2008
// Date de modification: 

#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Classification.DAL;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;

namespace TNS.AdExpress.Web.Controls.Selections.Russia {
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:PortofolioGenericDetailVehicleSelectionWebControl runat=server></{0}:PortofolioGenericDetailVehicleSelectionWebControl>")]
	public class PortofolioGenericDetailVehicleSelectionWebControl : WebControl {
		
		#region Variables
		/// <summary>
		/// Customer session
		/// </summary>
		protected WebSession _webSession = null;
		/// <summary>
		/// Media list dataSet
		/// </summary>
		private DataSet _dsListMedia = null;
		/// <summary>
		/// Key word
		/// </summary>
		protected string _keyWord = "";
		/// <summary>
		/// boolean si listeMedia est vide
		/// </summary>
		protected static bool _isEmptyList = false;
		/// <summary>
		/// Event 
		/// </summary>
		protected int _eventButton = -1;
		/// <summary>
		/// Verify is it possible to load a saved universe
		/// </summary>
		protected static bool _canLoadUnivese = true;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get / Set the customer session
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set { _webSession = value; }
		}

		/// <summary>
		/// Get / Set The Key word
		/// </summary>
		public string KeyWord {
			get { return _keyWord; }
			set { _keyWord = value; }
		}
		/// <summary>
		/// Obtient ListeMedia est vide
		/// </summary>
		[Bindable(true),
		Description("bool de pour la liste vide")]
		public static bool IsEmptyList {
			get { return _isEmptyList; }
			set { _isEmptyList = value; }
		}
		#endregion

		#region Events

		#region OnLoad
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">Events</param>
		protected override void OnLoad(EventArgs e) {

			VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            IClassificationDAL classficationDAL = null;
            CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = null;
            param = new object[3];
            param[0] = _webSession;
            param[1] = _webSession.GenericMediaSelectionDetailLevel;
            param[2] = "";
            classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            _dsListMedia = classficationDAL.GetDetailMedia(_keyWord);
			//_dsListMedia = TNS.AdExpress.Web.Core.DataAccess.DetailMediaDataAccess.keyWordDetailMediaListDataAccess(_webSession, _keyWord, "", _webSession.GenericMediaSelectionDetailLevel);
			if (_dsListMedia != null && _dsListMedia.Tables[0].Rows.Count > 0) {
				_isEmptyList = false;
			}
			else
				_isEmptyList = true;

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
			string textOpenclose = "", checkBox = "", allDivIds = "", disabled = "", displayDiv = "None", cssTextItem = "txtViolet10", cssL1 = "violetBackGroundV3"; ;
			int insertIndex = 0, nbLevels = 0, numColumn = 0, displayIndex = 0, counter = 0;
			int startL1 = -1, startL2 = -1, startL3 = -1;
            Int64 i = 0, idL1 = -2, oldIdL1 = long.MinValue, idL1Div = -2, idL2 = -2, oldIdL2 = long.MinValue, idL3 = -2, oldIdL3 = long.MinValue;
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

				if (!_isEmptyList) textOpenclose = GestionWeb.GetWebWord(2461, _webSession.SiteLanguage);

				if (_webSession.GenericMediaSelectionDetailLevel != null) {
					genericDetailLevel = _webSession.GenericMediaSelectionDetailLevel;
					nbLevels = genericDetailLevel.GetNbLevels;
				}

				//Start Global Table
				#region Debut Tableau global
				t.Append("<tr class=\"backGroundWhite\"><td  vAlign=\"top\"><table  vAlign=\"top\">");
                if (nbLevels > 1)
                {
                    t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
                    insertIndex = t.Length;
                    t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;" + textOpenclose + "</a>");
                }
				t.Append("<tr><td vAlign=\"top\">");
				#endregion

				if (nbLevels > 0) {

                    if (nbLevels == 1) t.Append("<table class=\"violetBorder mediumPurple1\"  cellpadding=0 cellspacing=0 width=\"800\">");

					foreach (DataRow currentRow in _dsListMedia.Tables[0].Rows) {
                        
                        counter += 1;
						//render html for each Level
						for (int lev = 1; lev <= nbLevels; lev++) {

							//Init ids parents
							currentLevel = genericDetailLevel[lev];
                            if (nbLevels > 1 && lev == 1)
                            {
								idL1 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL1 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
                            if (nbLevels > 1 && lev == 2)
                            {
								idL2 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL2 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
                            if (lev == 3 || nbLevels == 1)
                            {
								idL3 = Int64.Parse(currentRow[currentLevel.GetSqlFieldIdWithoutTablePrefix()].ToString());
								labelL3 = currentRow[currentLevel.GetSqlFieldWithoutTablePrefix()].ToString();
							}
						}

                        if (nbLevels > 1 && counter == 1)
                        {
                            idL1Div = idL1;
                        }
                        //to maintain the state of the list 
                        if (nbLevels > 1 && idL1Div != idL1)
                        {
                            t.Insert(displayIndex, displayDiv);
                            displayDiv = "none";
                            idL1Div = idL1;
                        }
                        if (nbLevels == 1) displayDiv = "";

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
                        if (nbLevels > 1 && idL1 > long.MinValue && idL1 != oldIdL1)
                        {
							//border top table
                            if (oldIdL1 == long.MinValue) t.Append("<table class=\"violetBorder txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"750\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "');\" style=\"cursor : pointer\">");
							else t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"750\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "');\" style=\"cursor : pointer\">");
							oldIdL1 = idL1;
							startL1 = 0;
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">&nbsp;&nbsp;&nbsp;" + labelL1);
							t.Append("</td>");
							t.Append("<td align=\"right\" class=\"arrowBackGround\"></td></tr>");
							t.Append("<tr><td colspan=\"2\"><div style=\"MARGIN-LEFT: 0px; DISPLAY:none;\"");
                            displayIndex = t.Length;
							//if (nbLevels == 2) 
                                cssL1 = "mediumPurple1";
							t.Append(" class=\"" + cssL1 + "\"  id=\"1_" + idL1 + "\">");
							t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//link for select all items
							//t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllNotDisabledChilds('idL1_" + idL1 + "')\" title=\"" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[2]), _webSession.SiteLanguage) + "\" class=\"roll04\">" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[2]), _webSession.SiteLanguage) + "</a></td></tr>");
							
							allDivIds = allDivIds + "1_" + idL1 + ",";
							//isNewL1 = false;
							numColumn = 0;
						}
						#endregion

						#region New Level 2 ( if nbLevels > 2)
                        if (nbLevels > 2 && (idL2 > long.MinValue && idL2 != oldIdL2))
                        {
							if (startL2 == -1) t.Append("<tr><td ><table class=\"violetBackGroundV3 txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "2_" + idL2 + "');\" style=\"cursor : pointer\">");
							else t.Append("<tr><td ><table class=\"violetBackGroundV3 violetBorderTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('1_" + idL1 + "2_" + idL2 + "');\" style=\"cursor : pointer\">");
							oldIdL2 = idL2;
							startL2 = 0;
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
							t.Append("&nbsp;&nbsp;" + labelL2 + "</td>");
							t.Append("<td align=\"right\" class=\"arrowBackGround\"></td></tr>");

							t.Append("<tr><td colspan=\"2\"><DIV style=\"MARGIN-LEFT: 5px\" class=\"mediumPurple1\" id=\"1_" + idL1 + "2_" + idL2 + "\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//link for select all items
							//t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllNotDisabledChilds('idL1_" + idL1 + "idL2_" + idL2 + "')\" title=\"" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[3]), _webSession.SiteLanguage) + "\" class=\"roll04\">" + GestionWeb.GetWebWord(GetWebWordCode(genericDetailLevel[3]), _webSession.SiteLanguage) + "</a></td></tr>");
							//isNewL2 = false;
							numColumn = 0;
						}
						#endregion

						#region New Level 3
						if (nbLevels == 2) {
							labelL3 = labelL2;
							idL3 = idL2;
						}

                        if(currentRow.Table.Columns.Contains("activation"))
                            cssTextItem = int.Parse(currentRow["activation"].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";
                        else
                            cssTextItem = "txtViolet10";

						if (numColumn == 0) {
							t.Append("<tr>");
							t.Append("<td class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"radio\" name=\"GenericDetailVehicleSelectionWebControl1\" ID=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + " onClick=\"insertValueInHidden('" + idL3 + "');\">" + labelL3 + "<br>");
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else if (numColumn == 1) {
							t.Append("<td  class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"radio\"  name=\"GenericDetailVehicleSelectionWebControl1\" ID=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + " onClick=\"insertValueInHidden('" + idL3 + "');\">" + labelL3 + "<br>");
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else {
							t.Append("<td class=\"" + cssTextItem + "\" width=\"33%\">");
							t.Append("<input type=\"radio\" name=\"GenericDetailVehicleSelectionWebControl1\" ID=\"GenericDetailVehicleSelectionWebControl1_" + i + "\" value=" + idL3 + " onClick=\"insertValueInHidden('" + idL3 + "');\">" + labelL3 + "<br>");
							t.Append("</td></tr>");
							numColumn = 0;
							i++;
						}
						oldIdL3 = idL3;

						#endregion

						//isNewL1 = false;
						//isNewL2 = false;
					}
                    if (nbLevels > 1 && allDivIds.Length > 0)
                    {
						allDivIds = allDivIds.Remove(allDivIds.Length - 1, 1);
						t.Insert(insertIndex, allDivIds);
					}

					
				}

				
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
                if (nbLevels > 1) t.Append("</table></div></td></tr></table>");//Close Level 1
                if (nbLevels == 1) t.Append(" </table>"); 
				//t.Append("</table></div></td></tr></table>");//Close Level 1
				t.Append("   </td></tr></table></td>");
				t.Append("</tr>");

				#endregion
			}
			#endregion

			output.Write(t.ToString());
		}
		#endregion

		#region Private Methods


		/// <summary>
		/// Get if items can be shown
		/// </summary>
		/// <returns>if items can be shown</returns>
		private bool CanShowData() {
			if (_keyWord.Length < 3 && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT) return false;
			if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(_keyWord) && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT) return false;
			if (_dsListMedia != null && _dsListMedia.Tables[0].Rows.Count == 0) return false;
			return (!_isEmptyList);			
		}

		#region Message

		/// <summary>
		/// Get Message to show
		/// <example>Error message</example>
		/// </summary>
		/// <returns>Message</returns>
		protected string GetMessage() {
			System.Text.StringBuilder t = new System.Text.StringBuilder(3000);
			#region  Message d'erreurs
			// Message d'erreur : veuillez saisir 3 caractères minimums
			if (_keyWord.Length < 3 && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT) {
				t.Append("<tr class=\"backGroundWhite\" ><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(352, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
				t.Append("</td>");
				t.Append("</tr>");
			}
			// Message d'erreur : mot incorrect
			else if (!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(_keyWord) && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT) {
				t.Append("<tr><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(1088, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
				t.Append("</td>");
				t.Append("</tr>");
			}
			// Message d'erreur : aucun résultat avec le mot clé
			else if (_dsListMedia != null) {
				if (_dsListMedia.Tables[0].Rows.Count == 0) {
					t.Append("<tr><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" " + GestionWeb.GetWebWord(819, _webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
					t.Append("</td>");
					t.Append("</tr>");
				}
			}
			#endregion

			// Message d'erreur : Chargement de l'univers impossible
			if (!_canLoadUnivese) {
				t.Append("<tr class=\"backGroundWhite\" ><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" " + GestionWeb.GetWebWord(1086, _webSession.SiteLanguage) + "</p> ");
				t.Append(" </td> ");
				t.Append(" </tr> ");
			}

			return t.ToString();
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

		#endregion
	}
}
