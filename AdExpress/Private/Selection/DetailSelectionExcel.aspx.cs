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
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.UI;
using TNS.FrameWork;
using TNS.FrameWork.Date;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Description résumée de DetailSelectionExcel.
	/// </summary>
	public partial class DetailSelectionExcel : TNS.AdExpress.Web.UI.PrivateWebPage{
	
		#region Variables MMI
		/// <summary>
		/// Choix de l'étude
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
		/// <summary>
		/// Période
		/// </summary>
		/// <summary>
		/// Média
		/// </summary>
		/// <summary>
		/// Unité
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
		public string productAdExpressText;

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
		/// 		/// Si vrai affiche les produits dans la page aspx
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
		/// Indique si on affiche type de pourcentage
		/// </summary>
		public bool displayPercentageAlignment = false;
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
		public string referenceProductAdExpressText="";
		/// <summary>
		/// Texte
		/// </summary>
		public string productText="";
		/// <summary>
		/// Choix de l'étude
		/// </summary>
		public string typeEtude="";
		/// <summary>
		/// Unité
		/// </summary>
		public string unitTitle="";
		/// <summary>
		/// Type de pourcentage
		/// </summary>
		public string percentageAlignmentTitle ="";
		/// <summary>
		/// Libellé de la section genres d'émissions
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
		/// Affiche les éléments dans reference media
		/// </summary>
		public bool displayReferenceDetailMedia=false;
		/// <summary>
		/// Affiche le code pour les éléments contenue dans reference media
		/// </summary>
		public string referenceMediaDetailText;
		/// <summary>
		/// Texte Annonceur
		/// </summary>
		public string advertiserAdexpresstext="";
		/// <summary>
		/// Texte : détail de votre sélection
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
		/// Libellé type de pourcentage
		/// </summary>

		/// <summary>
		/// Libellé niveaux de détil génériques
		/// </summary>

		
		/// <summary>
		/// Indique si on affiche nioveaux de détail génériques
		/// </summary>
		public bool displayGenericlevelDetailLabel = false;
        /// <summary>
        /// Indique si on affiche les niveaux de détail colonne génériques
        /// </summary>
        public bool displayGenericlevelDetailColumnLabel = false;

		/// <summary>
		/// Texte titre Noveaux de détail
		/// </summary>
		public string genericlevelDetailLabelTitle = "";

        /// <summary>
        /// Texte titre Niveaux de détail colonne
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
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
                Response.ContentType = "application/vnd.ms-excel";
			
				#region Variables
				//string periodText;
				int i=1;
				int idMedia=1;
				int idAdvertiser=1;						
				#endregion
				
				// logo
                logo = TNS.AdExpress.Web.UI.ExcelWebPage.GetLogo(_webSession);

				// Intitulé de l'étude
				typeEtude=Convertion.ToHtmlString(GestionWeb.GetWebWord(842,_webSession.SiteLanguage));

                // Zoom Date
                _zoomDate = Page.Request.QueryString.Get("zoomDate");

                if (_zoomDate == null) _zoomDate = "";

				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);

				#region Affichage des paramètres sélectionnés
				
                detailSelection=Convertion.ToHtmlString(GestionWeb.GetWebWord(1539,_webSession.SiteLanguage))+" "+DateTime.Now.ToString("dd/MM/yyyy")+"";
				// Module
				moduleLabel.Text=Convertion.ToHtmlString(GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule),_webSession.SiteLanguage));

				// Unité
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

				#region Media de références
				// Détail référence média			
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

				#region Détail média
				// Détail Média
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
	

				#region Périodes
				// Période
                if (_zoomDate == null) _zoomDate = "";
                if (_zoomDate.Length > 0) {
                    displayPeriod = true;
                    infoDateLabel.Text = HtmlFunctions.GetZoomPeriodDetail(_webSession, _zoomDate);
                }
                else if (_webSession.isDatesSelected()) {
					displayPeriod=true;
					infoDateLabel.Text = HtmlFunctions.GetPeriodDetail(_webSession);
				}
				#endregion

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                    ) {

                    #region Période de l'étude
                    // Période de l'étude
                    if (_webSession.isStudyPeriodSelected()) {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(_webSession);
                    }
                    #endregion

                    #region Période comparative
                    // Période comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodComparative()) {
                            displayComparativePeriod = true;
                            comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type Sélection comparative
                    // Type Sélection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isComparativePeriodTypeSelected()) {
                            displayComparativePeriodType = true;
                            ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type disponibilité des données
                    // Type disponibilité des données
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
				
				//Niveaux de détail générique	
				MediaDetailLevel.GetGenericLevelDetail(_webSession,ref displayGenericlevelDetailLabel,genericlevelDetailLabel,true);
				if(displayGenericlevelDetailLabel)genericlevelDetailLabelTitle =  Convertion.ToHtmlString(GestionWeb.GetWebWord(1886,_webSession.SiteLanguage));

                //Niveaux de détail colonne générique	
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

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {    
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

		#region Méthodes internes
		/// <summary>
		/// Vérifie si produits sélectionnés
		/// </summary>
		/// <param name="_webSession">session du client</param>
		/// <returns>vrai si produis sélectionnés</returns>
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
