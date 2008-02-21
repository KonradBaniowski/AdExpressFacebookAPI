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
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.UI;
using TNS.FrameWork;
using TNS.FrameWork.Date;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Description r�sum�e de DetailSelectionExcel.
	/// </summary>
	public partial class DetailSelectionExcel : TNS.AdExpress.Web.UI.WebPage{
	
		#region Variables MMI
		/// <summary>
		/// Choix de l'�tude
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
		/// <summary>
		/// P�riode
		/// </summary>
		/// <summary>
		/// M�dia
		/// </summary>
		/// <summary>
		/// Unit�
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText5;
		/// <summary>
		/// Texte
		/// </summary>	
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText advertiserCompetitorText;
		/// <summary>
		/// Bouton Fermer
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl closeImageButtonRollOverWebControl;
		/// <summary>
		///  Choix de l'�tude
		/// </summary>
		/// <summary>
		/// Date
		/// </summary>
		/// <summary>
		/// Unit�
		/// </summary>
		/// <summary>
		/// D�tail de votre s�lection
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
		public string productAdExpressText;

		#endregion

		#region Variables
		/// <summary>
		/// System.Windows.Forms.TreeNode des media
		/// </summary>
		protected string mediaText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des genres d'�missions
		/// </summary>
		protected string programTypeText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des formes de parrainage
		/// </summary>
		protected string sponsorshipFormText;
		/// <summary>
		/// D�tail des m�dias
		/// </summary>
		protected string mediaDetailText;
		/// <summary>
		/// System.Windows.Forms.TreeNode des annonceurs/r�f�rences
		/// </summary>
		protected string advertiserText;
		/// <summary>
		/// Affiche les media dans page aspx
		/// </summary>
		public bool displayMedia=false;
		/// <summary>
		/// Affiche les genres d'�missions dans page aspx
		/// </summary>
		public bool displayProgramType=false;
		/// <summary>
		/// Affiche les formes de parrainage dans page aspx
		/// </summary>
		public bool displaySponsorshipForm=false;
		/// <summary>
		/// Affiche les d�tails d'un m�dia
		/// </summary>	
		public bool displayDetailMedia=false;
		/// <summary>
		/// Affiche les annonceurs dans page aspx
		/// </summary>
		public bool displayAdvertiser=false;
		/// <summary>
		/// 		/// Si vrai affiche les produits dans la page aspx
		/// </summary>
		public bool displayProduct=false;
		/// <summary>
		/// Affiche la p�riode dans page aspx
		/// </summary>
		public bool displayPeriod=false;
        /// <summary>
        /// Affiche la p�riode de l'�tude dans la page aspx
        /// </summary>
        public bool displayStudyPeriod = false;
        /// <summary>
        /// Affiche la p�riode comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriod = false;
        /// <summary>
        /// Affiche le type de la p�riode comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriodType = false;
        /// <summary>
        /// Affiche le type de la disponibilit� des donn�es dans la page aspx
        /// </summary>
        public bool displayPeriodDisponibilityType = false;
		/// <summary>
		/// Bool�en pour afficher les annonceurs concurrents
		/// </summary>
		public bool displayCompetitorAdvertiser=false;
		/// <summary>
		/// Affiche les annonceurs de r�f�rences
		/// </summary>
		public bool displayReferenceAdvertiser=false;
		/// <summary>
		/// Affiche les vagues s�lectionn�es
		/// </summary>
		public bool displayWave=false;
		/// <summary>
		///Affiche les cibles s�lectionn�es 
		/// </summary>
		public bool displayTargets=false;
		/// <summary>
		/// Indique si on affiche type de pourcentage
		/// </summary>
		public bool displayPercentageAlignment = false;
		/// <summary>
		/// Code html pour afficher les annonceurs de r�f�rences s�l�ctionn�e
		/// </summary>
		public string referenceAdvertiserText="";		
		/// <summary>
		/// Texte
		/// </summary>
		public string competitorAdvertiserText;		
		/// <summary>
		/// Texte
		/// </summary>
		public string referenceProductAdExpressText="";
		/// <summary>
		/// Texte
		/// </summary>
		public string productText="";
		/// <summary>
		/// Choix de l'�tude
		/// </summary>
		public string typeEtude="";
		/// <summary>
		/// Unit�
		/// </summary>
		public string unitTitle="";
		/// <summary>
		/// Type de pourcentage
		/// </summary>
		public string percentageAlignmentTitle ="";
		/// <summary>
		/// Libell� de la section genres d'�missions
		/// </summary>
		public string programTypeLabel="";
		/// <summary>
		/// Etude comparative
		/// </summary>
		public bool comparativeStudy=false;
		/// <summary>
		/// Texte etude comparative
		/// </summary>
		public string comparativeStudyText="";
		/// <summary>
		/// Affiche les �l�ments dans reference media
		/// </summary>
		public bool displayReferenceDetailMedia=false;
		/// <summary>
		/// Affiche le code pour les �l�ments contenue dans reference media
		/// </summary>
		public string referenceMediaDetailText;
		/// <summary>
		/// Texte Annonceur
		/// </summary>
		public string advertiserAdexpresstext="";
		/// <summary>
		/// Texte : d�tail de votre s�lection
		/// </summary>
		public string detailSelection="";
		/// <summary>
		/// Affiche les vagues
		/// </summary>
		public string waveText="";
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext9;
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
		///Affiche les cibles 
		/// </summary>
		public string targetsText="";
		/// <summary>
		/// Libell� type de pourcentage
		/// </summary>

		/// <summary>
		/// Libell� niveaux de d�til g�n�riques
		/// </summary>

		
		/// <summary>
		/// Indique si on affiche nioveaux de d�tail g�n�riques
		/// </summary>
		public bool displayGenericlevelDetailLabel = false;
        /// <summary>
        /// Indique si on affiche les niveaux de d�tail colonne g�n�riques
        /// </summary>
        public bool displayGenericlevelDetailColumnLabel = false;

		/// <summary>
		/// Texte titre Noveaux de d�tail
		/// </summary>
		public string genericlevelDetailLabelTitle = "";

        /// <summary>
        /// Texte titre Niveaux de d�tail colonne
        /// </summary>
        public string genericlevelDetailColumnLabelTitle = "";

		/// <summary>
		/// logo
		/// </summary>
		public string logo = "";
		/// <summary>
		/// copryright
		/// </summary>
		public string copyRight = "";
        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _zoomDate = "";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DetailSelectionExcel():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
			
				#region Variables
				//string periodText;
				int i=1;
				int idMedia=1;
				int idAdvertiser=1;						
				#endregion
				
				// logo
				logo = TNS.AdExpress.Web.UI.ExcelWebPage.GetLogo();

				// Intitul� de l'�tude
				typeEtude=Convertion.ToHtmlString(GestionWeb.GetWebWord(842,_webSession.SiteLanguage));

                // Zoom Date
                _zoomDate = Page.Request.QueryString.Get("zoomDate");

                if (_zoomDate == null) _zoomDate = "";

				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);

				#region Affichage des param�tres s�lectionn�s
				
                detailSelection=Convertion.ToHtmlString(GestionWeb.GetWebWord(1539,_webSession.SiteLanguage))+" "+DateTime.Now.ToString("dd/MM/yyyy")+"";
				// Module
				moduleLabel.Text=Convertion.ToHtmlString(GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule),_webSession.SiteLanguage));

				// Unit�
				unitTitle=Convertion.ToHtmlString(GestionWeb.GetWebWord(849,_webSession.SiteLanguage));
				unitLabel.Text=Convertion.ToHtmlString(GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[_webSession.Unit],_webSession.SiteLanguage));	

				// Media
				if (_webSession.isMediaSelected()){
					displayMedia=true;
					mediaText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.SelectionUniversMedia,_webSession.SiteLanguage,false);
				}

				#region Media concurrents
				if(_webSession.isCompetitorMediaSelected()){
					displayDetailMedia=true;
					System.Text.StringBuilder mediaSB=new System.Text.StringBuilder(1000);
				
					mediaSB.Append("<TR>");
					mediaSB.Append("<td width=\"5\"></td>");
					mediaSB.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					mediaSB.Append("<label>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1087,_webSession.SiteLanguage))+"</label></TD>");
					mediaSB.Append("</TR>");

					while((System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia]!=null){
					
						System.Windows.Forms.TreeNode tree=(System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia];				
						mediaSB.Append("<TR height=\"20\">");
						mediaSB.Append("<td width=\"5\"></td>");
						mediaSB.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((System.Windows.Forms.TreeNode)_webSession.CompetitorUniversMedia[idMedia],_webSession.SiteLanguage,true)+"</TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR>");
						mediaSB.Append("<td width=\"5\"></td>");
						mediaSB.Append("<TD>&nbsp;</TD>");
						mediaSB.Append("</TR>");						
						i++;
						idMedia++;
					}
					mediaDetailText=mediaSB.ToString();
				}
				#endregion

				#region Media de r�f�rences
				// D�tail r�f�rence m�dia			
				if(_webSession.isReferenceMediaSelected()){
			
					displayReferenceDetailMedia=true;
					System.Text.StringBuilder referenceDetailMedia=new System.Text.StringBuilder(1000);
				
					referenceDetailMedia.Append("<TR>");
					referenceDetailMedia.Append("<td width=\"5\"></td>");
					referenceDetailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					referenceDetailMedia.Append("<label>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+"</label></TD>");
					referenceDetailMedia.Append("</TR>");									
					referenceDetailMedia.Append("<TR height=\"20\">");
					referenceDetailMedia.Append("<td width=\"5\"></td>");
					referenceDetailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((System.Windows.Forms.TreeNode)_webSession.ReferenceUniversMedia,_webSession.SiteLanguage,true)+"</TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"5\">");
					referenceDetailMedia.Append("<td width=\"5\"></td>");
					referenceDetailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"7\">");
					referenceDetailMedia.Append("<td width=\"5\"></td>");
					referenceDetailMedia.Append("<TD></TD>");
					referenceDetailMedia.Append("</TR>");									
					referenceMediaDetailText=referenceDetailMedia.ToString();			
				}
				#endregion

				#region D�tail m�dia
				// D�tail M�dia
				if(_webSession.SelectionUniversMedia.FirstNode!=null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count>0){
			
					displayDetailMedia=true;
					System.Text.StringBuilder detailMedia=new System.Text.StringBuilder(1000);
				
					detailMedia.Append("<TR>");
					detailMedia.Append("<td width=\"5\"></td>");
					detailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					detailMedia.Append("<label>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+"</label></TD>");
					detailMedia.Append("</TR>");				
					
									
					detailMedia.Append("<TR height=\"20\">");
					detailMedia.Append("<td width=\"5\"></td>");
					detailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((System.Windows.Forms.TreeNode)_webSession.SelectionUniversMedia.FirstNode,_webSession.SiteLanguage,true)+"</TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"5\">");
					detailMedia.Append("<td width=\"5\"></td>");
					detailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"7\">");
					detailMedia.Append("<td width=\"5\"></td>");
					detailMedia.Append("<TD></TD>");
					detailMedia.Append("</TR>");
					mediaDetailText=detailMedia.ToString();			
				}
				#endregion
			
				// Etude comparative
				if(_webSession.ComparativeStudy){
					comparativeStudy=true;
					comparativeStudyText=GestionWeb.GetWebWord(1118,_webSession.SiteLanguage);
				}
				// Vague
				if (_webSession.IsWaveSelected()){
					displayWave=true;
					waveText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.SelectionUniversAEPMWave,_webSession.SiteLanguage,false);
				}
				//Cibles				
				if (_webSession.IsTargetSelected()){
					displayTargets=true;
					targetsText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.SelectionUniversAEPMTarget,_webSession.SiteLanguage,false);
				}

				#region Ancienne version
				#region Annonceurs
				//// Annonceur/R�f�rence
				//if (_webSession.isAdvertisersSelected() && !_webSession.isCompetitorAdvertiserSelected()){
				//    displayAdvertiser=true;
				//    // Affichage type d'advertiser
				//    if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(814,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(813,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
				//    {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(1585,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(815,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(965,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(966,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        advertiserAdexpresstext=Convertion.ToHtmlString(GestionWeb.GetWebWord(964,_webSession.SiteLanguage));
				//    }
				//    else if (((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess
				//   || ((LevelInformation)_webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException) {
				//        advertiserAdexpresstext = Convertion.ToHtmlString(GestionWeb.GetWebWord(2242, _webSession.SiteLanguage));
				//    }
			

				//    // Affichage du System.Windows.Forms.TreeNode
				//    advertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.CurrentUniversAdvertiser,_webSession.SiteLanguage,true);
					
				//}
				#endregion
               
				#region Produits
				//// Produit
				//if (_webSession.isSelectionProductSelected()){
				//    displayProduct=true;
			
				//    if(((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        productAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(815,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        productAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(965,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        productAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(966,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        productAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(964,_webSession.SiteLanguage));
				//    }
				//    else if (((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess
				//   || ((LevelInformation)_webSession.SelectionUniversProduct.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException) {
				//        productAdExpressText = Convertion.ToHtmlString(GestionWeb.GetWebWord(2242, _webSession.SiteLanguage));
				//    }	

				//    // Affichage du System.Windows.Forms.TreeNode
				//    productText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.SelectionUniversProduct,_webSession.SiteLanguage,true);
				//    i++;
				//    if(_webSession.CurrentModule==195)
				//    {
				//        productText+=TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetExcelSectorsSelected(_webSession);
				//    }
				//}else if(IsSelectionProductSelected(_webSession)){
				//    displayProduct=true;
				//    if(((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException) {
				//        productAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(965,_webSession.SiteLanguage));
				//    }					
				//    // Affichage du System.Windows.Forms.TreeNode
				//    productText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.CurrentUniversProduct,_webSession.SiteLanguage,true);
				//}
				#endregion

				#region Annonceurs de r�f�rences
				//// R�f�rence advertiser
				//if (_webSession.isReferenceAdvertisersSelected()){
				//    displayReferenceAdvertiser=true;
			
				//    if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(814,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(813,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
				//    {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(1585,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(815,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(965,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(966,_webSession.SiteLanguage));
				//    }
				//    else if(((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(964,_webSession.SiteLanguage));
				//    }
				//    else if (((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess
				//   || ((LevelInformation)_webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.segmentException) {
				//        referenceProductAdExpressText = Convertion.ToHtmlString(GestionWeb.GetWebWord(2242, _webSession.SiteLanguage));
				//    }
				//    if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//        || _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//        ){
				//        referenceProductAdExpressText=Convertion.ToHtmlString(GestionWeb.GetWebWord(1195,_webSession.SiteLanguage));
				//    }
				//    // Affichage du System.Windows.Forms.TreeNode
				//    referenceAdvertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.ReferenceUniversAdvertiser,_webSession.SiteLanguage,true);
					
				//}
				#endregion
				#endregion

				//Selection univers produit
				//productAdExpressText = Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage));
				productText = Convertion.ToHtmlString(TNS.AdExpress.Web.UI.ExcelWebPage.GetProductSelected(_webSession));
				if (productText != null && productText.Length > 0) {
					productText = "<table>" + productText + "</table>";
					displayProduct = true;
				}
				if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) {
					productText += TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetExcelSectorsSelected(_webSession);
				}
				if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
					|| _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
					){
					idAdvertiser=0;
				}
			
				#region Annonceurs Concurrents (ancienne version)
				//if(_webSession.isCompetitorAdvertiserSelected()){
				//    displayCompetitorAdvertiser=true;
				//    displayAdvertiser=false;
				//    System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
				//    string nameProduct="";
						
				//    while(_webSession.CompetitorUniversAdvertiser[idAdvertiser]!=null){
					
				//        System.Windows.Forms.TreeNode tree=null;
				//        if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//            || _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//            ){
				//            tree=(System.Windows.Forms.TreeNode)_webSession.CompetitorUniversAdvertiser[idAdvertiser];
			
				//        }else{
				//            tree=((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser;
				//        }
				//        if(tree.FirstNode!=null){
					
				//            if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
				//                ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
				//                nameProduct=GestionWeb.GetWebWord(814,_webSession.SiteLanguage);
				//            }
				//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
				//                ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
				//                nameProduct=GestionWeb.GetWebWord(813,_webSession.SiteLanguage);
				//            }
				//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
				//                ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
				//            {
				//                nameProduct=GestionWeb.GetWebWord(1585,_webSession.SiteLanguage);
				//            }
				//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//                ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//                nameProduct=GestionWeb.GetWebWord(815,_webSession.SiteLanguage);
				//            }
				//            if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//                || _webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//                ){
				//                nameProduct=GestionWeb.GetWebWord(1196,_webSession.SiteLanguage);
				//            }

				//            t.Append("<TR>");
				//            t.Append("<td width=\"5\"></td>");
				//            t.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
				//            t.Append("<label>"+Convertion.ToHtmlString(nameProduct)+"</label></TD>");
				//            t.Append("</TR>");
					
				//            if(_webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//                && _webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//                ){
				//                t.Append("<TR>");
				//                t.Append("<td width=\"5\"></td>");
				//                t.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
				//                t.Append("<Label>"+(string)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[idAdvertiser]).NameCompetitorAdvertiser)+"</Label>");
				//                t.Append("</TD></TR>");
				//            }
				//            t.Append("<TR height=\"20\">");		
				//            t.Append("<td width=\"5\"></td>");
				//            if(_webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//                && _webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//                ){
				//                t.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((System.Windows.Forms.TreeNode)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser),_webSession.SiteLanguage,true)+"</TD>");
				//            }
				//            else{
				//                t.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(tree,_webSession.SiteLanguage,true)+"</TD>");
				//            }
				//            t.Append("</TR>");
				//            t.Append("<TR height=\"5\">");	
				//            t.Append("<td width=\"5\"></td>");
				//            t.Append("<TD bgColor=\"#ffffff\"></TD>");
				//            t.Append("</TR>");
				//            t.Append("<TR height=\"7\">");
				//            t.Append("<TD></TD>");
				//            t.Append("</TR>");
				//            idAdvertiser++;
				//        }else{
				//            idAdvertiser++;
				//        }
				//    }
				//    competitorAdvertiserText=t.ToString();
				//}
				#endregion

				#region P�riodes
				// P�riode
				if (_webSession.isDatesSelected()){
					displayPeriod=true;
					infoDateLabel.Text = HtmlFunctions.GetPeriodDetail(_webSession);
				}
				#endregion

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {

                    #region P�riode de l'�tude
                    // P�riode de l'�tude
                    if (_zoomDate.Length > 0) {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetZoomPeriodDetail(_webSession, _zoomDate);
                    }
                    else if (_webSession.isStudyPeriodSelected()) {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(_webSession);
                    }
                    #endregion

                    #region P�riode comparative
                    // P�riode comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodComparative()) {
                            displayComparativePeriod = true;
                            comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type S�lection comparative
                    // Type S�lection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isComparativePeriodTypeSelected()) {
                            displayComparativePeriodType = true;
                            ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type disponibilit� des donn�es
                    // Type disponibilit� des donn�es
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodDisponibilityTypeSelected()) {
                            displayPeriodDisponibilityType = true;
                            PeriodDisponibilityType.Text = HtmlFunctions.GetPeriodDisponibilityTypeDetail(_webSession);
                        }
                    }
                    #endregion

                }

                //Program Type
				if (_webSession.IsCurrentUniversProgramTypeSelected()){
					displayProgramType=true;
					programTypeLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(2066,_webSession.SiteLanguage));
					programTypeText = TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.CurrentUniversProgramType,_webSession.SiteLanguage,false);
				}
				//Sponsorship Form
				if (_webSession.IsCurrentUniversSponsorshipFormSelected()){
					displaySponsorshipForm=true;
					sponsorshipFormText = TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(_webSession.CurrentUniversSponsorshipForm,_webSession.SiteLanguage,false);
				}

				//Type de pourcentage
				switch(_webSession.PercentageAlignment){
					case CstWeb.Percentage.Alignment.vertical :
						percentageAlignmentTitle = Convertion.ToHtmlString(GestionWeb.GetWebWord(2153,_webSession.SiteLanguage));
						displayPercentageAlignment = true;
						percentageAlignmentLabel.Text = Convertion.ToHtmlString(GestionWeb.GetWebWord(2065,_webSession.SiteLanguage));	
						break;
					case CstWeb.Percentage.Alignment.horizontal :
						percentageAlignmentTitle = Convertion.ToHtmlString(GestionWeb.GetWebWord(2153,_webSession.SiteLanguage));
						displayPercentageAlignment = true;
						percentageAlignmentLabel.Text = Convertion.ToHtmlString(GestionWeb.GetWebWord(2064,_webSession.SiteLanguage));						
						break;
					default : break;						
				}		
				
				//Niveaux de d�tail g�n�rique	
				MediaDetailLevel.GetGenericLevelDetail(_webSession,ref displayGenericlevelDetailLabel,genericlevelDetailLabel,true);
				if(displayGenericlevelDetailLabel)genericlevelDetailLabelTitle =  Convertion.ToHtmlString(GestionWeb.GetWebWord(1886,_webSession.SiteLanguage));

                //Niveaux de d�tail colonne g�n�rique	
                MediaDetailLevel.GetGenericLevelDetailColumn(_webSession, ref displayGenericlevelDetailColumnLabel, genericlevelDetailColumnLabel, true);
                if (displayGenericlevelDetailColumnLabel) genericlevelDetailColumnLabelTitle = Convertion.ToHtmlString(GestionWeb.GetWebWord(2300, _webSession.SiteLanguage));
				#endregion

				// copyright
				copyRight = TNS.AdExpress.Web.UI.ExcelWebPage.GetFooter(_webSession);
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent() {    
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region M�thodes internes
		/// <summary>
		/// V�rifie si produits s�lectionn�s
		/// </summary>
		/// <param name="_webSession">session du client</param>
		/// <returns>vrai si produis s�lectionn�s</returns>
		private static bool IsSelectionProductSelected(WebSession _webSession){
			switch(_webSession.CurrentModule){
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PRESSE :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_RADIO :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_TELEVISION:
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PAN_EURO :
					if(_webSession.CurrentUniversProduct!=null && _webSession.CurrentUniversProduct.Nodes.Count>0)return true;
					else return false;
				default : return false;
			}
		}
		#endregion

	}
}
