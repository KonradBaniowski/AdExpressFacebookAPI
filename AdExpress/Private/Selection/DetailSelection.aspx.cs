#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
// Date de modification: 
//		22/04/2005 K. Shehzad Intégration de famille pour le module Indicateurs
/*		23/08/2005 G. RAGNEAU Period detail displaying modification
 * */
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using DBCst = TNS.AdExpress.Constantes.DB;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Rappel des sélections
	/// </summary>
	public partial class DetailSelection : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables MMI
		/// <summary>
		/// Choix de l'étude
		/// </summary>
		/// <summary>
		/// Période
		/// </summary>
		/// <summary>
		/// Média
		/// </summary>
		/// <summary>
		/// Unité
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>	
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText advertiserCompetitorText;
		/// <summary>
		/// Bouton Fermer
		/// </summary>
		/// <summary>
		///  Choix de l'étude
		/// </summary>
		/// <summary>
		/// Date
		/// </summary>
		/// <summary>
		/// Unité
		/// </summary>
		/// <summary>
		/// Détail de votre sélection
		/// </summary>
		/// <summary>
		/// Label 
		/// </summary>
		protected System.Web.UI.WebControls.Label univers1Text;
		/// <summary>
		/// Texte
		/// </summary>
		protected System.Web.UI.WebControls.Label univers2Text;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>

		/// <summary>
		/// Libellé type de pourcentage
		/// </summary>
		/// <summary>
		/// Libellé niveaux de détil génériques
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// System.Windows.Forms.TreeNode des media
		/// </summary>
		protected string mediaText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des genres d'émissions
		/// </summary>
		protected string programTypeText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des formes de parrainage
		/// </summary>
		protected string sponsorshipFormText;
		/// <summary>
		/// Détail des médias
		/// </summary>
		protected string mediaDetailText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des annonceurs/références
		/// </summary>
		protected string advertiserText;
		/// <summary>
		/// Script javascript
		/// </summary>
		protected string script;
		/// <summary>
		/// Affiche les media dans page aspx
		/// </summary>
		public bool displayMedia=false;
		/// <summary>
		/// Affiche les genres d'émissions dans page aspx
		/// </summary>
		public bool displayProgramType=false;
		/// <summary>
		/// Affiche les formes de parrainage dans page aspx
		/// </summary>
		public bool displaySponsorshipForm=false;
		/// <summary>
		/// Affiche les détails d'un média
		/// </summary>	
		public bool displayDetailMedia=false;
		/// <summary>
		/// Affiche les annonceurs dans page aspx
		/// </summary>
		public bool displayAdvertiser=false;
		/// <summary>
		/// Si vrai affiche les produits dans la page aspx
		/// </summary>
		public bool displayProduct=false;
		/// <summary>
		/// Affiche la période dans page aspx
		/// </summary>
		public bool displayPeriod=false;
        /// <summary>
        /// Affiche la période de l'étude dans la page aspx
        /// </summary>
        public bool displayStudyPeriod = false;
        /// <summary>
        /// Affiche la période comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriod = false;
        /// <summary>
        /// Affiche le type de la période comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriodType = false;
        /// <summary>
        /// Affiche le type de la disponibilité des données dans la page aspx
        /// </summary>
        public bool displayPeriodDisponibilityType = false;
		/// <summary>
		/// Affiche les informations sur les agences médias
		/// </summary>
		public bool displayMediaAgency=false;
		/// <summary>
		/// Booléen pour afficher les annonceurs concurrents
		/// </summary>
		public bool displayCompetitorAdvertiser=false;
		/// <summary>
		/// Affiche les annonceurs de références
		/// </summary>
		public bool displayReferenceAdvertiser=false;
		/// <summary>
		/// Affiche les vagues sélectionnées
		/// </summary>
		public bool displayWave=false;
		/// <summary>
		///Affiche les cibles sélectionnées 
		/// </summary>
		public bool displayTargets=false;
		/// <summary>
		/// Code html pour afficher les annonceurs de références séléctionnée
		/// </summary>
		public string referenceAdvertiserText="";		
		/// <summary>
		/// Texte
		/// </summary>
		public string competitorAdvertiserText;		
		/// <summary>
		/// Texte
		/// </summary>
		public string productText="";
		/// <summary>
		/// Etude comparative
		/// </summary>
		public bool comparativeStudy=false;
		/// <summary>
		/// Texte etude comparative
		/// </summary>
		public string comparativeStudyText="";
		/// <summary>
		/// Affiche les éléments dans reference media
		/// </summary>
		public bool displayReferenceDetailMedia=false;
		/// <summary>
		/// Affiche le code pour les éléments contenue dans reference media
		/// </summary>
		public string referenceMediaDetailText;
		/// <summary>
		/// Affiche les vagues
		/// </summary>
		public string waveText="";
		/// <summary>
		///Affiche les cibles 
		/// </summary>
		public string targetsText="";
		/// <summary>
		/// Lien pour l'export Excel
		/// </summary>
		public string excelUrl="";
		/// <summary>
		/// texte
		/// </summary>
		/// <summary>
		/// Agence
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Commentaire Excel
		/// </summary>
		public string excelComment="";
		/// <summary>
		/// Indique si on affiche type de pourcentage
		/// </summary>
		public bool displayPercentageAlignment = false;
		
		/// <summary>
		/// Indique si on affiche les niveaux de détail génériques
		/// </summary>
		public bool displayGenericlevelDetailLabel = false;
        /// <summary>
        /// Indique si on affiche les niveaux de détail colonne génériques
        /// </summary>
        public bool displayGenericlevelDetailColumnLabel = false;
        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _zoomDate = "";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DetailSelection():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
		
			try{
			
				#region Variables
//				string periodText;
				int i=1;
				int idMedia=1;
				int idAdvertiser=1;	
//				string dateBegin;
//				string dateEnd;	
				
				#endregion
			
//				excelUrl="/Private/Selection/DetailSelectionExcel.aspx?idSession="+_webSession.IdSession;
//				excelComment=GestionWeb.GetWebWord(791,_webSession.SiteLanguage);

                // Zoom Date
                _zoomDate = Page.Request.QueryString.Get("zoomDate");

                if (_zoomDate == null) _zoomDate = "";

				// Rollover
				//closeImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_up.gif";
				//closeImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_down.gif";

				// Chargement du javascript des System.Windows.Forms.TreeNode
				//script= TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript();

				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_webSession.SiteLanguage);
				
				MenuWebControl2.ForcePrint = "/Private/Selection/DetailSelectionExcel.aspx?idSession="
					+ this._webSession.IdSession + ((_zoomDate.Length > 0)?"&zoomDate="+_zoomDate:"");
				MenuWebControl2.ForcePrintTraductionCode = 791;

                #region Script
                // script
                if(!Page.ClientScript.IsClientScriptBlockRegistered("script")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "script", TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript());
                }
                #endregion


                #region Affichage des paramètres sélectionnés
                // Module
				moduleLabel.Text=GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule),_webSession.SiteLanguage);

				// Unité
				unitLabel.Text=GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[_webSession.Unit],_webSession.SiteLanguage);	

				// Media
				if (_webSession.isMediaSelected()){
					displayMedia=true;
					mediaText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversMedia,false,false,false,"100",false,false,_webSession.SiteLanguage,2,1,true);
				}

				if(_webSession.isCompetitorMediaSelected()){
					displayDetailMedia=true;
					System.Text.StringBuilder mediaSB=new System.Text.StringBuilder(1000);
				
					mediaSB.Append("<TR><TD></TD>");
					mediaSB.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					mediaSB.Append("<label>"+GestionWeb.GetWebWord(1087,_webSession.SiteLanguage)+"</label></TD>");
					mediaSB.Append("</TR>");

					while((System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia]!=null){
					
						System.Windows.Forms.TreeNode tree=(System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia];				
						mediaSB.Append("<TR height=\"20\">");
						mediaSB.Append("<TD>&nbsp;</TD>");
						mediaSB.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia],false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true)+"</TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"5\">");
						mediaSB.Append("<TD></TD>");
						mediaSB.Append("<TD bgColor=\"#ffffff\"></TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"7\">");
						mediaSB.Append("<TD colSpan=\"2\"></TD>");
						mediaSB.Append("</TR>");
						if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
							Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
						}
						i++;
						idMedia++;
					}
					mediaDetailText=mediaSB.ToString();
				}
			
				// Détail référence média
			
				if(_webSession.isReferenceMediaSelected()){
			
					displayReferenceDetailMedia=true;
					System.Text.StringBuilder referenceDetailMedia=new System.Text.StringBuilder(1000);
				
					referenceDetailMedia.Append("<TR><TD></TD>");
					referenceDetailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					referenceDetailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,_webSession.SiteLanguage)+"</label></TD>");
					referenceDetailMedia.Append("</TR>");				
					
									
					referenceDetailMedia.Append("<TR height=\"20\">");
					referenceDetailMedia.Append("<TD>&nbsp;</TD>");
					referenceDetailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_webSession.ReferenceUniversMedia,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true)+"</TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"5\">");
					referenceDetailMedia.Append("<TD></TD>");
					referenceDetailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"7\">");
					referenceDetailMedia.Append("<TD colSpan=\"2\"></TD>");
					referenceDetailMedia.Append("</TR>");
					if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
					}
					i++;				
					referenceMediaDetailText=referenceDetailMedia.ToString();			
			
				}

				// Détail Média
				if(_webSession.SelectionUniversMedia.FirstNode!=null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count>0){
			
					displayDetailMedia=true;
					System.Text.StringBuilder detailMedia=new System.Text.StringBuilder(1000);
				
					detailMedia.Append("<TR><TD></TD>");
					detailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					detailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,_webSession.SiteLanguage)+"</label></TD>");
					detailMedia.Append("</TR>");				
					
									
					detailMedia.Append("<TR height=\"20\">");
					detailMedia.Append("<TD>&nbsp;</TD>");
					detailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_webSession.SelectionUniversMedia.FirstNode,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true)+"</TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"5\">");
					detailMedia.Append("<TD></TD>");
					detailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"7\">");
					detailMedia.Append("<TD colSpan=\"2\"></TD>");
					detailMedia.Append("</TR>");
					if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
					}
					i++;				
					mediaDetailText=detailMedia.ToString();			
				}
			
				// Etude comparative

				if(_webSession.ComparativeStudy){
					comparativeStudy=true;
					comparativeStudyText=GestionWeb.GetWebWord(1118,_webSession.SiteLanguage);

				}

				// Agence média
				if(_webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyProduct
					|| _webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser
					|| _webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.group_agencyAgency
					){
					displayMediaAgency=true;
					MediaAgency.Text=_webSession.MediaAgencyFileYear.Substring(_webSession.MediaAgencyFileYear.Length-4,4);;
				
				}
				
				// Vague
				if (_webSession.IsWaveSelected()){
					displayWave=true;
					waveText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversAEPMWave,false,false,false,"100",false,false,_webSession.SiteLanguage,2,1,true);
				}

				//Cibles				
				if (_webSession.IsTargetSelected()){
					displayTargets=true;
					targetsText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversAEPMTarget,false,false,false,"100",false,false,_webSession.SiteLanguage,2,1,true);
				}

				#region ancienne version
				//// Annonceur/Référence
				//if (_webSession.isAdvertisersSelected() && !_webSession.isCompetitorAdvertiserSelected()){
				//    displayAdvertiser=true;
				//    // Affichage type d'advertiser
				//    if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
				//        advertiserAdexpresstext.Code=814;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
				//        advertiserAdexpresstext.Code=813;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
				//    {
				//        advertiserAdexpresstext.Code=1585;
				//    }

				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        advertiserAdexpresstext.Code=815;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        advertiserAdexpresstext.Code=965;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        advertiserAdexpresstext.Code=966;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        advertiserAdexpresstext.Code=964;
				//    }
				//    else if (((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess
				//   || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException) {
				//        advertiserAdexpresstext.Code = 2242;
				//    }	

				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//    }

				//    // Affichage du System.Windows.Forms.TreeNode
				//    advertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.CurrentUniversAdvertiser,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true);
				//    i++;
				//}





				//// Produit
				//if (_webSession.isSelectionProductSelected()){
				//    displayProduct=true;
			
				//    if(((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        productAdExpressText.Code=815;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException) {
				//        productAdExpressText.Code=965;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        productAdExpressText.Code=966;
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        productAdExpressText.Code=964;
				//    }
				//    else if (((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException
				//   || ((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess) {
				//        productAdExpressText.Code = 2242;
				//    }	

				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//    }

				//    // Affichage du System.Windows.Forms.TreeNode					
				//    productText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversProduct,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true);
				//    i++;
				//    //changes for sector display
				//    if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR ) {
				//        productText+=TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetSectorsSelected(_webSession);
				//    }


				//}else if(IsSelectionProductSelected(_webSession)){
				//    displayProduct=true;
				//    if(((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException) {
				//        productAdExpressText.Code=965;
				//    }					
				//    productText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.CurrentUniversProduct,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true);
				//}




				//// Référence advertiser

				//if (_webSession.isReferenceAdvertisersSelected()){
				//    displayReferenceAdvertiser=true;
			
				//    if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
				//        referenceProductAdExpressText.Code=814;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
				//        referenceProductAdExpressText.Code=813;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
				//    {
				//        referenceProductAdExpressText.Code=1585;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        referenceProductAdExpressText.Code=815;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        referenceProductAdExpressText.Code=965;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        referenceProductAdExpressText.Code=966;
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        referenceProductAdExpressText.Code=964;
				//    }
				//    else if (((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess
				//   || ((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException) {
				//        referenceProductAdExpressText.Code = 2242;
				//    }	
				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//    }

				//    if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//        || _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//        ){
				//        referenceProductAdExpressText.Code=1195;
				//    }

				//    // Affichage du System.Windows.Forms.TreeNode
				//    referenceAdvertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.ReferenceUniversAdvertiser,false,true,true,"100",true,false,_webSession.SiteLanguage,2,i,true);
				//    i++;
				//}

				#endregion
				Oracle.DataAccess.Client.OracleConnection oConnection = null;
				//Univers produit principal sélectionné
				if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0) {

					
					//if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
					//        || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
					//)
					//oConnection = new Oracle.DataAccess.Client.OracleConnection(DBCst.Connection.RECAP_CONNECTION_STRING);
					//else
						//oConnection = _webSession.CustomerLogin.Connection;

					System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
					string nameProduct = "";

					if (_webSession.PrincipalProductUniverses.Count > 1) {
						displayCompetitorAdvertiser = true;						
						productAdExpressText.Code = 2302;
					}
					else {
						displayProduct = true;
						productAdExpressText.Code = 1759;
					}					

					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/Styles/TreeView/Css";
					selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
					selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
					selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
					selectItemsInClassificationWebControl.DBSchema = TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA;
					for(int k =0; k<_webSession.PrincipalProductUniverses.Count;k++){
						if (_webSession.PrincipalProductUniverses.Count > 1) {
							if (_webSession.PrincipalProductUniverses.ContainsKey(k)) {
								if (k > 0) {
									nameProduct = GestionWeb.GetWebWord(2301, _webSession.SiteLanguage);
								}
								else {
									nameProduct = GestionWeb.GetWebWord(2302, _webSession.SiteLanguage);
								}

								t.Append("<TR><TD></TD>");
								t.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
								t.Append("<label>" + nameProduct + "</label></TD>");
								t.Append("</TR>");

								//Universe Label
								if (_webSession.PrincipalProductUniverses[k].Label != null && _webSession.PrincipalProductUniverses[k].Label.Length > 0) {
									t.Append("<TR>");
									t.Append("<TD></TD>");
									t.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
									t.Append("<Label>" + _webSession.PrincipalProductUniverses[k].Label + "</Label>");
									t.Append("</TD></TR>");
								}

								//Render universe html code
								t.Append("<TR height=\"20\">");
								t.Append("<TD>&nbsp;</TD>");
								t.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">" + selectItemsInClassificationWebControl.ShowUniverse(_webSession.PrincipalProductUniverses[k], _webSession.SiteLanguage, _webSession.Source) + "</TD>");								
								t.Append("</TR>");
								t.Append("<TR height=\"5\">");
								t.Append("<TD></TD>");
								t.Append("<TD bgColor=\"#ffffff\"></TD>");
								t.Append("</TR>");
								t.Append("<TR height=\"7\">");
								t.Append("<TD colSpan=\"2\"></TD>");
								t.Append("</TR>");

							}
						}
						else {
							if (_webSession.PrincipalProductUniverses.ContainsKey(k)) {
								productText += selectItemsInClassificationWebControl.ShowUniverse(_webSession.PrincipalProductUniverses[k], _webSession.SiteLanguage, _webSession.Source);
							}
						}						
					}
					if (_webSession.PrincipalProductUniverses.Count > 1) 
					competitorAdvertiserText=t.ToString();
				}

				//Univers produit secondaire selectionné
				if (_webSession.SecondaryProductUniverses != null && _webSession.SecondaryProductUniverses.Count > 0) {

					//if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
					//        || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
					//)
					//    oConnection = new Oracle.DataAccess.Client.OracleConnection(DBCst.Connection.RECAP_CONNECTION_STRING);
					//else 
					//oConnection = _webSession.CustomerLogin.Connection;

					if (WebFunctions.Modules.IsDashBoardModule(_webSession)) {
						displayProduct = true;
						productAdExpressText.Code = 1759;
					}
					else {

						if (_webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
							&& _webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
							) {
							referenceProductAdExpressText.Code = 2302;
							displayReferenceAdvertiser = true;
						}
					}
					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/Styles/TreeView/Css";
					selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
					selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
					selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
					selectItemsInClassificationWebControl.DBSchema = TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA;
					
					if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
							|| _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE){

						if(_webSession.SecondaryProductUniverses.ContainsKey(0)){
							//Liste des annonceurs de référence personnalisés
							referenceAdvertiserText = selectItemsInClassificationWebControl.ShowUniverse(_webSession.SecondaryProductUniverses[0], _webSession.SiteLanguage, _webSession.Source);
							referenceProductAdExpressText.Code = 1195;
							displayReferenceAdvertiser = true;
						}
						if (_webSession.SecondaryProductUniverses.ContainsKey(1)) {
							//Liste des annonceurs de référence personnalisés
							 advertiserAdexpresstext.Code = 1196;
							 displayAdvertiser = true;
                             advertiserText += selectItemsInClassificationWebControl.ShowUniverse(_webSession.SecondaryProductUniverses[1],_webSession.SiteLanguage,_webSession.Source);
						}

					}
					else {
						for (int k = 0; k < _webSession.SecondaryProductUniverses.Count; k++) {
							if (_webSession.SecondaryProductUniverses.ContainsKey(k)) {
								if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0
											&& _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE && _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis) {
									productText = competitorAdvertiserText = "";
									displayProduct = false;
								}

								if (k > 0) {
									if (k == 1) {
										advertiserAdexpresstext.Code = 2301;
										displayAdvertiser = true;
									}
                                    advertiserText += selectItemsInClassificationWebControl.ShowUniverse(_webSession.SecondaryProductUniverses[k],_webSession.SiteLanguage,_webSession.Source);
								}
								else {
									if (WebFunctions.Modules.IsDashBoardModule(_webSession)) {
                                        productText = selectItemsInClassificationWebControl.ShowUniverse(_webSession.SecondaryProductUniverses[k],_webSession.SiteLanguage,_webSession.Source);
									}
									else {
                                        referenceAdvertiserText = selectItemsInClassificationWebControl.ShowUniverse(_webSession.SecondaryProductUniverses[k],_webSession.SiteLanguage,_webSession.Source);										
									}
								}
							}
						}
					}
				}
				if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
					|| _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
					){
					idAdvertiser=0;
			
				}
				//changes for sector display
				if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) {
					productText += TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetSectorsSelected(_webSession);
				}
				

				// Période
                if (_zoomDate == null) _zoomDate = "";
                if (_zoomDate.Length > 0) {
                    displayPeriod = true;
                    infoDateLabel.Text = HtmlFunctions.GetZoomPeriodDetail(_webSession, _zoomDate);
                }
                else if (_webSession.isDatesSelected()){
					displayPeriod=true;
					infoDateLabel.Text = HtmlFunctions.GetPeriodDetail(_webSession);
				}

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                    ) {

                    // Période de l'étude
                    if (_webSession.isStudyPeriodSelected()) {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(_webSession);
                    }

                    // Période comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodComparative()) {
                            displayComparativePeriod = true;
                            comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(_webSession);
                        }
                    }

                    // Type Sélection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isComparativePeriodTypeSelected()) {
                            displayComparativePeriodType = true;
                            ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(_webSession);
                        }
                    }

                    // Type disponibilité des données
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodDisponibilityTypeSelected()) {
                            displayPeriodDisponibilityType = true;
                            PeriodDisponibilityType.Text = HtmlFunctions.GetPeriodDisponibilityTypeDetail(_webSession);
                        }
                    }
                }

				//Program Type
				if (_webSession.IsCurrentUniversProgramTypeSelected()){
					displayProgramType=true;
					programTypeText = TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.CurrentUniversProgramType,false,true,true,"100",true,false,_webSession.SiteLanguage,2,1,true);
				}
				//Sponsorship Form
				if (_webSession.IsCurrentUniversSponsorshipFormSelected()){
					displaySponsorshipForm=true;
					sponsorshipFormText = TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.CurrentUniversSponsorshipForm,false,true,true,"100",true,false,_webSession.SiteLanguage,2,1,true);
				}

				//Type de pourcentage
				switch(_webSession.PercentageAlignment){
					case CstWeb.Percentage.Alignment.vertical :
						displayPercentageAlignment = true;
						percentageAlignmentLabel.Text = GestionWeb.GetWebWord(2065,_webSession.SiteLanguage);	
						break;
					case CstWeb.Percentage.Alignment.horizontal :
						displayPercentageAlignment = true;
						percentageAlignmentLabel.Text = GestionWeb.GetWebWord(2064,_webSession.SiteLanguage);						
						break;
					default : break;						
				}	
					
				//Niveaux de détail générique	
				WebFunctions.MediaDetailLevel.GetGenericLevelDetail(_webSession,ref displayGenericlevelDetailLabel,genericlevelDetailLabel,false);

                //Niveaux de détail colonne générique	
                WebFunctions.MediaDetailLevel.GetGenericLevelDetailColumn(_webSession, ref displayGenericlevelDetailColumnLabel, genericlevelDetailColumnLabel, false);
				#endregion
				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}

		}
		#endregion

		#region Determine PostBack
		/// <summary>
		/// Determine Post Back
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection ret = base.DeterminePostBackMode ();

			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;

			return ret ;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

        //#region Bouton Fermer
        ///// <summary>
        ///// Gestion du bouton fermer
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void closeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
        //    Response.Write("<script language=javascript>");
        //    Response.Write("	window.close();");
        //    Response.Write("</script>");
        //}
        //#endregion

		#region Méthodes internes
		/// <summary>
		/// Vérifie si produits sélectionnés
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si produis sélectionnés</returns>
		private static bool IsSelectionProductSelected(WebSession webSession){
			switch(webSession.CurrentModule){
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PRESSE :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_RADIO :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_TELEVISION :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PAN_EURO :
					if(webSession.CurrentUniversProduct!=null && webSession.CurrentUniversProduct.Nodes.Count>0)return true;
					else return false;
				default : return false;
			}
		}

		#region Niveau de détail support générique
		/// <summary>
		/// Niveau de détail support ou média(Niveaux détaillés par  :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private void GetGenericLevelDetail(WebSession webSession){
			ArrayList detailSelections = null;

			//_webSession.CustomerLogin.ModuleList();
			Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
			try{
				detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
			}
			catch(System.Exception){
				if(currentModule.Id==CstWeb.Module.Name.ALERTE_PORTEFEUILLE)
					detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
			}
			foreach(int currentType in detailSelections){
				switch((CstWeb.DetailSelection.Type)currentType){
					case CstWeb.DetailSelection.Type.genericMediaLevelDetail :
						genericlevelDetailLabel.Text  = _webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);
						if(genericlevelDetailLabel.Text!=null && genericlevelDetailLabel.Text.Length>0)
							displayGenericlevelDetailLabel = true;
						break;
					case CstWeb.DetailSelection.Type.genericProductLevelDetail :
						genericlevelDetailLabel.Text = _webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage);
						if(genericlevelDetailLabel.Text!=null && genericlevelDetailLabel.Text.Length>0)
							displayGenericlevelDetailLabel = true;
						break;
					default:
						break;
				}
			}
		}
		#endregion



	
		#endregion

	}
}
