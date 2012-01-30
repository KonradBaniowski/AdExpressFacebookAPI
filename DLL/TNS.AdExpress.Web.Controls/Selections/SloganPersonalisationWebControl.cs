#region Informations
// Auteur: D. Mussuma
// Date de création: 31/05/2006
// Date de modification:  
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;

using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Slogans;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using VhCstes=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TableName=TNS.AdExpress.Constantes.Classification.DB.Table.name;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes= TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Controls.Buttons;
using TNS.AdExpress.Domain.Classification;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Insertions;

namespace TNS.AdExpress.Web.Controls.Selections {
	/// <summary>
	/// Composant de sélection des versions en fonction du produit et du média
	/// </summary>
	public class SloganPersonalisationWebControl :  System.Web.UI.WebControls.CheckBoxList {
		
		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 
		/// <summary>
		/// Dataset contenant la liste des médias
		/// </summary>
		protected DataSet dsSloganList;
		/// <summary>
		/// Vrai si possède des slogans à traiter
		/// </summary>
		public bool hasSlogans=false;
		/// <summary>
		/// Valider la sélection
		/// </summary>
		public ImageButtonRollOverWebControl _validateButton;

		/// <summary>
		/// Zoom date
		/// </summary>
		protected string _zoomDate = "";

		/// <summary>
		/// Period type
		/// </summary>
		protected Constantes.Web.CustomerSessions.Period.Type _periodType;

        /// <summary>
        /// Data Access Layer
        /// </summary>
        protected IInsertionsDAL _dalLayer;
		#endregion
	
		#region Accesseurs
		/// <summary>
		/// Définit la session à utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}

		/// <summary>
		/// Zoom date
		/// </summary>
		public string ZoomDate {
			set { _zoomDate = value; }
		}

		/// <summary>
		/// Period type
		/// </summary>
		public Constantes.Web.CustomerSessions.Period.Type PeriodType {
			set { _periodType = value; }
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SloganPersonalisationWebControl():base() {
			this.EnableViewState=true;
		}
		#endregion

		#region Evènements
		
		#region Onload
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e) {	

			#region Obtention des données
//			if(!Page.IsPostBack){
				if( webSession != null) {
					string periodBeginning;
					string periodEnd;
                    DateTime begin;
                    DateTime end;

					if((webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.dayly && webSession.PeriodBeginningDate.Length<8)
						|| webSession.CurrentModule==Constantes.Web.Module.Name.BILAN_CAMPAGNE
						|| webSession.CurrentModule==Constantes.Web.Module.Name.JUSTIFICATIFS_PRESSE){
						periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
						periodEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
					}
					else{
						periodBeginning=webSession.PeriodBeginningDate;
						periodEnd=webSession.PeriodEndDate;
		
					}

					if (_zoomDate != null && _zoomDate.Length > 0) {
						if (_zoomDate.Length < 8) {

                            begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, _periodType);
                            end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, _periodType);

                            begin = WebFunctions.Dates.Max(begin,
                                        WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                            end = WebFunctions.Dates.Min(end,
                                        WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                            periodBeginning = begin.ToString("yyyyMMdd");
                            periodEnd = end.ToString("yyyyMMdd");

						}
						else periodBeginning = periodEnd = _zoomDate;						 
					}
					//Chargement des données
                    if (WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(webSession))
                    {                       
                        //Get data                       
                        object[] param = new object[2];
                        param[0] = webSession;
                        param[1] = webSession.CurrentModule;
                        CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertionsDAL];
                        if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
                        _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
                        dsSloganList = _dalLayer.GetVersions(periodBeginning, periodEnd);
                    }
				}
				else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas définie"));

			#endregion

			#region Script
			// Cochage/Decochage des checkbox pères, fils et concurrents
			if (!Page.ClientScript.IsClientScriptBlockRegistered("CheckAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CheckAllChilds",WebFunctions.Script.CheckAllChilds());
			}
			// Ouverture/fermeture des fenêtres pères
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",WebFunctions.Script.DivDisplayer());
			}
			// fermer/ouvrir tous les calques
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",WebFunctions.Script.ExpandColapseAllDivs());
			}	
			// Sélection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectAllChilds",WebFunctions.Script.SelectAllChilds());
			}	
			// Intégration automatique
			if (!Page.ClientScript.IsClientScriptBlockRegistered("GroupIntegration")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GroupIntegration",WebFunctions.Script.GroupIntegration());
			}
			// Sélection de tous les 
			if (!Page.ClientScript.IsClientScriptBlockRegistered("AllGroupIntegration")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllGroupIntegration",WebFunctions.Script.AllGroupIntegration());
			}	
			// Affichage de l'image taille réelle
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ViewAdZoom")){
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ViewAdZoom",WebFunctions.Script.ViewAdZoom());
			}
			//script d'ouverture d'une popUp de téléchargement
			if (!Page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openDownload",WebFunctions.Script.OpenDownload());
			}
			if (!Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")){
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openPressCreation",WebFunctions.Script.OpenPressCreation());
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInternetCreation", WebFunctions.Script.OpenInternetCreation());
			}
            if (!Page.ClientScript.IsClientScriptBlockRegistered("OpenWindow"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenWindow", WebFunctions.Script.OpenWindow());
            }
			#endregion
		}
		#endregion

		#region Prerender
		/// <summary>
		/// Contruction des éléments du cheboxlist
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnPreRender(EventArgs e){		
				
			#region Variables locales
			//Variables media
			Int64 IdVehicle = 0;
			Int64 oldIdVehicle = 0;
				
			//Variables produits
			Int64 IdProduct = 0;
			Int64 oldIdProduct = 0;	
			
			//Variables versions
			Int64 IdSlogan = -1;
			Int64 oldIdSlogan = -1;	
			
			//Variables annonceur
			Int64 IdAvertiser = 0;
			Int64 oldIdAdvertiser =-1;	

			bool isNewAdvertiser=false;
			bool isNewProduct=false;
			bool isNewVehicle=false;
			#endregion
				
			//Initialisation de la liste d'items
			this.Items.Clear();
			
			if(WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(webSession)){
				//Construction de la liste de checkbox	
				if(dsSloganList != null && dsSloganList.Tables.Count>0 && dsSloganList.Tables[0].Rows.Count>0){
					foreach(DataRow currentRow in dsSloganList.Tables[0].Rows) {
                        if (currentRow["id_slogan"] != null && currentRow["id_slogan"] != System.DBNull.Value && Int64.Parse(currentRow["id_slogan"].ToString()) != 0)
                        {
							if ((IdAvertiser = Int64.Parse(currentRow["id_advertiser"].ToString())) != oldIdAdvertiser){
								oldIdAdvertiser =IdAvertiser;
								isNewAdvertiser=true;
							}
							//Ajout d'un produit
							if ((IdProduct = Int64.Parse(currentRow["id_product"].ToString())) != oldIdProduct){							
								oldIdProduct = IdProduct;
								isNewProduct=true;
							}

							//Ajout d'un média (vehicle)
							if ((IdVehicle = Int64.Parse(currentRow["id_vehicle"].ToString())) != oldIdVehicle){							
								oldIdVehicle = IdVehicle;
								isNewVehicle =true;					
							}

							//Ajout d'une version

							if (Int64.Parse(currentRow["id_slogan"].ToString())!= 0 && (( IdSlogan = Int64.Parse(currentRow["id_slogan"].ToString())) != oldIdSlogan  )
								||(isNewVehicle) || (isNewProduct) || (isNewAdvertiser) ) {
								oldIdSlogan = IdSlogan ;
								this.Items.Add(new System.Web.UI.WebControls.ListItem(IdSlogan.ToString(),"slg_"+IdSlogan)); 
							}
							isNewVehicle=isNewProduct=isNewAdvertiser=false;
					
						}
					}
					if(this.Items.Count>0)hasSlogans=true;
				}
			}								
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);		
		
			string[] fileList = null;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

			#region Construction du code HTML
            if (WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(webSession) && dsSloganList != null && dsSloganList.Tables.Count>0 && dsSloganList.Tables[0].Rows.Count > 0 && hasSlogans)
            {
				
				#region variables locales
				//Hastable de versions
				Int64 i = 0;

				//variables du niveau  Annonceur
				Int64 idAdvertiserOld=-1;
				Int64 idAdvertiser;										
				int startAdvertiser=-1;		
				string advertiserIds = "";	
				bool isNewAdvertiser=false;
			
				//variables du niveau produit
				Int64 idProductOld=-1;
				Int64 idProduct;										
				int startProduct=-1;	
				bool isNewProduct=false;

				//variables du niveau media (vehicle)
				Int64 idVehicleOld=-1;
				Int64 idVehicle;										
				int startVehicle=-1;
				bool isNewVehicle=false;
				
				//variables du niveau slogans
				int numColumn = 0;
				string checkBox="";																	
				Int64 idSloganOld=-1;
				Int64 idSlogan=-1;

                IInsertionsResult _rulesLayer = null;
				#endregion

				#region Debut Tableau global 
                output.Write("<tr vAlign=\"top\" height=\"100%\" align=\"center\"><td class=\"backGroundWhite\"><table  vAlign=\"top\">");		
				
				output.Write("<a href=\"javascript: SelectAllChilds('selectAllSlogans");
				output.Write("')\" class=\"roll04\" >"+GestionWeb.GetWebWord(816,webSession.SiteLanguage)+"</a>");								
				output.Write("<tr><td vAlign=\"top\">");
				output.Write("<DIV id=selectAllSlogans>");//Ouverture calque permettant de sélectionner tous les éléménts
				#endregion

                CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertions];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
                object[] param = new object[2];
                param[0] = webSession;
                param[1] = webSession.CurrentModule;
                _rulesLayer = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);				

				#region Foreach  Dataset des versions
				foreach(DataRow currentRow in dsSloganList.Tables[0].Rows) {

					#region Pour chaque slogan
                    if (currentRow["id_slogan"] != null && currentRow["id_slogan"] != System.DBNull.Value && Int64.Parse(currentRow["id_slogan"].ToString()) != 0)
                    {
						//Initialisation des identifiants parents
						idAdvertiser=Int64.Parse(currentRow["id_advertiser"].ToString());
						idProduct=Int64.Parse(currentRow["id_product"].ToString());
						idVehicle=Int64.Parse(currentRow["id_vehicle"].ToString());
						idSlogan = Int64.Parse(currentRow["id_slogan"].ToString());
				
						#region Fermeture media
						if ((idVehicle!= idVehicleOld && startVehicle==0) || (idProduct!= idProductOld && startProduct==0) 
							|| (idAdvertiser!= idAdvertiserOld && startAdvertiser==0) ) {								
							if (numColumn==1) {
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								output.Write("</tr>");
							}else if(numColumn==2 ) {
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								output.Write("</tr>");
							}
							output.Write("</table><tr height=\"5\"><td></td></tr></div></td></tr></table></td></tr>");
						}
						#endregion

						#region Fermeture produit
						if ((idProduct!= idProductOld && startProduct==0) 
							|| (idAdvertiser!= idAdvertiserOld && startAdvertiser==0)
							){
							startVehicle=-1;
							output.Write("</table></div></td></tr></table></td></tr>");
						}
						#endregion 
					
						#region Fermeture Annonceur
						if (idAdvertiser!= idAdvertiserOld && startAdvertiser==0) {
							startProduct=-1;						
							output.Write("</table></div></td></tr></table>");
						}
						#endregion

						#region Nouvel annonceur
						if (idAdvertiser!= idAdvertiserOld) {
							//bordure du haut de tableau
							if (idAdvertiserOld == -1)output.Write("<table class=\"violetBorder txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\"><tr onClick=\"javascript : DivDisplayer('ad_"+idAdvertiser+"');\" style=\"cursor : pointer\">");
                            else output.Write("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\"><tr onClick=\"javascript : DivDisplayer('ad_" + idAdvertiser + "');\" style=\"cursor : pointer\">");
                            //idAdvertiserOld=idAdvertiser;
							startAdvertiser=0;
							output.Write("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">&nbsp;&nbsp;&nbsp;"+currentRow["advertiser"].ToString());
							output.Write("</td>");
                            output.Write("<td align=\"right\" class=\"arrowBackGround\"></td>");
                            output.Write("</tr><tr><td colspan=\"2\"><div style=\"MARGIN-LEFT: 0px; display ='none';\" class=\"violetBackGroundV3\" id=\"ad_" + idAdvertiser + "\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//lien tous selectionner
							output.Write("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('ad_"+idAdvertiser+"')\" title=\""+GestionWeb.GetWebWord(817,webSession.SiteLanguage)+"\" class=\"roll04\">"+GestionWeb.GetWebWord(817,webSession.SiteLanguage)+"</a></td></tr>");
							advertiserIds+="ad_"+idAdvertiser+",";
							isNewAdvertiser = true;
						}
						#endregion

						#region Nouveau produit
						if ((idProduct!= idProductOld) 
							|| (idAdvertiser!= idAdvertiserOld && startAdvertiser==0)
							){
							//bordure du haut de tableau
							if (startProduct == -1)output.Write("<tr><td ><table class=\"violetBackGroundV3 txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('ad"+idAdvertiser+"pr_"+idProduct+"');\" style=\"cursor : pointer\">");
                            else output.Write("<tr><td ><table class=\"violetBackGroundV3 violetBorderTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('ad" + idAdvertiser + "pr_" + idProduct + "');\" style=\"cursor : pointer\">");
							idProductOld=idProduct;
							startProduct=0;
							output.Write("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
							output.Write("&nbsp;&nbsp;"+currentRow["product"].ToString()+"</td>");
                            output.Write("<td align=\"right\" class=\"arrowBackGround\"></td>");
                            output.Write("</tr><tr><td colspan=\"2\"><DIV style=\"MARGIN-LEFT: 5px\" class=\"mediumPurple1\" id=\"ad" + idAdvertiser + "pr_" + idProduct + "\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							//lien tous selectionner
							output.Write("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('ad"+idAdvertiser+"pr_"+idProduct+"')\" title=\""+GestionWeb.GetWebWord(944,webSession.SiteLanguage)+"\" class=\"roll04\">"+GestionWeb.GetWebWord(944,webSession.SiteLanguage)+"</a></td></tr>");
											
							isNewProduct=true;
						}
						#endregion

						#region Nouveau media (vehicle)
						if ((idVehicle!= idVehicleOld) || (isNewProduct) 
							|| (idAdvertiser!= idAdvertiserOld && startAdvertiser==0) ) {
							numColumn = 0;
							//bordure du haut de tableau#
                            output.Write("<tr><td ><table class=\"mediumPurple1 whiteTopBorder txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"643\"><tr width=100%>");
							idVehicleOld=idVehicle;
							startVehicle=0;
							output.Write("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
							output.Write("&nbsp;");			
							output.Write(currentRow["vehicle"].ToString()+"</td>");
							output.Write("<td align=\"right\" width=100% onClick=\"javascript : DivDisplayer('ad"+idAdvertiser+"pr"+idProduct+"vh_"+idVehicle+"');\" style=\"cursor : pointer\" class=\"arrowBackGround\"></td>");
							output.Write("</tr><tr><td colspan=\"2\"><DIV style=\"MARGIN-LEFT: 0px\" id=\"ad"+idAdvertiser+"pr"+idProduct+"vh_"+idVehicle+"\"><table cellpadding=0 cellspacing=0 border=\"0\" class=\"greyBackGround\" width=\"100%\"><tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('ad"+idAdvertiser+"pr"+idProduct+"vh_"+idVehicle+"')\" class=\"roll04\"  >"+GestionWeb.GetWebWord(1932,webSession.SiteLanguage)+"</a></td></tr>");						
							isNewVehicle=true;
						}
						#endregion

                        //correction bug set id old advertiser
                        if (idAdvertiser != idAdvertiserOld) idAdvertiserOld = idAdvertiser;

						if (( idSlogan!=idSloganOld ) ||(isNewVehicle) || (isNewProduct) 
							|| (isNewAdvertiser) && idSlogan!=0) {

							#region versions (fils de media)
							string vignettes="";
							string sloganDetail="";
                            //string imagesList="";
                            //bool first=true;
							string pathWeb = string.Empty;

                            //Get creative links
                            sloganDetail = _rulesLayer.GetCreativeLinks(idVehicle, currentRow);

							if(numColumn==0) {								
								output.Write("<tr>");
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">");								
								output.Write("<input type=\"checkbox\" "+checkBox+" name=\"SloganPersonalisationWebControl1$"+i+"\" id=\"SloganPersonalisationWebControl1_"+i+"\" value=slg_"+currentRow["id_slogan"]+">"+sloganDetail);
								output.Write("</td>");								
								numColumn++;
								i++;
								
							}
							else if(numColumn==1 ) {
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">");								
								output.Write("<input type=\"checkbox\" "+checkBox+" name=\"SloganPersonalisationWebControl1$"+i+"\" id=\"SloganPersonalisationWebControl1_"+i+"\" value=slg_"+currentRow["id_slogan"]+">"+sloganDetail);
								output.Write("</td>");								
								numColumn++;
								i++;
								
							}
							else {
								output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">");								
								output.Write("<input type=\"checkbox\" "+checkBox+" name=\"SloganPersonalisationWebControl1$"+i+"\" id=\"SloganPersonalisationWebControl1_"+i+"\" value=slg_"+currentRow["id_slogan"]+">"+sloganDetail);
								output.Write("</td></tr>");
								numColumn=0;
								i++;
								
							}
							idSloganOld=idSlogan;
							#endregion
						}
						isNewProduct=false;
						isNewAdvertiser=false;
						isNewVehicle=false;
					}
					#endregion
				}
				#endregion
				
				#region Fermeture Tableau global
				if (numColumn==1) {
					output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					output.Write("</tr>");
				}else if(numColumn==2 ) {
					output.Write("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
					output.Write("</tr>");
				}
				output.Write("</table></div></td></tr></table></td></tr>      </table></div></td></tr></table></td></tr>         </table></div></td></tr></table></td></tr></table></td>");			
				output.Write(" </DIV>");//Fermeture calque permettant de sélectionner tous les éléménts
				output.Write("</tr>");
				if(advertiserIds!=null && advertiserIds.Length>0){
					advertiserIds = advertiserIds.Remove(advertiserIds.Length-1, 1);
				}
				#endregion
			
			}
			else {
                output.Write("<tr><td class=\"backGroundWhite txtGris11Bold\"><div align=\"center\" class=\"txtGris11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
					+"</div><br> </td>  </tr>");
			}
			#endregion

			output.Write(t.ToString());
		}
		#endregion

		#endregion


	}
}
