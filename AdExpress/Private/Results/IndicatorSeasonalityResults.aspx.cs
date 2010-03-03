#region Information
/*
 * auteur : D. V. MUSSUMA
 * créé le : 30/09/2004
 * modifié le : 
 *	30/12/2004  D. Mussuma Intégration de WebPage
 *      12/08/2008 - G Ragneau - Layers Analyse Secto
 * */
#endregion

#region Namespace
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
using Oracle.DataAccess.Client;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;


using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Translation.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Core.Sessions;


#endregion

namespace AdExpress.Private.Results {
	
	/// <summary>
	///Résultas des différentes planches indicateurs :
	///- Saisonnalité : répartition des investissements par média et par annonceurs ou références.
	///- totalChoice : le top 10 des annonceurs et références ayant le plus investi sur la période sélectionnée
	///en fonction du média.
	///- Nouveautés : nouveaux annonceurs ou références sur la période sélectionnée.
	///- Evolution : annonceurs et références dont l'investissement a le plus augmenté entre la période N-1 et N.
	///- Stratégie Média : répartion des investissements par média et par annonceurs et références.
	/// </summary>
    public partial class IndicatorSeasonalityResults : TNS.AdExpress.Web.UI.ResultWebPage {

		#region variables
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Choix du type de total (bas sélection, famille, marché)
		/// </summary>
        public bool totalChoice = true;
		/// <summary>
		/// Taille de l'image dans un plus gd format
		/// </summary>
		public bool bigFormat=false;
		/// <summary>
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";

		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public IndicatorSeasonalityResults():base(){
			// Chargement de la Session			
			_webSession.CurrentModule = CstWeb.Module.Name.INDICATEUR;
		}
		#endregion		

		#region Evènements

		#region chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {

            try
            {

                #region Gestion du flash d'attente
                if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
                {
                    string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                    if (nomInput != MenuWebControl2.ID)
                    {
                        Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                        Page.Response.Flush();
                    }
                }
                else
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
                #endregion

                #region Url Suivante
                if (_nextUrl.Length != 0)
                {
                    _webSession.Source.Close();
                    Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
                }
                #endregion

                #region Textes et Langage du site
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
                ResultsOptionsWebControl1.ChartTitle = GestionWeb.GetWebWord(1191, _webSession.SiteLanguage);
                ResultsOptionsWebControl1.TableTitle = GestionWeb.GetWebWord(1192, _webSession.SiteLanguage);
                zoomTitle = GestionWeb.GetWebWord(1235, _webSession.SiteLanguage);
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

				VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

				if (_webSession.CurrentTab == CstResult.MediaStrategy.MEDIA_STRATEGY) {
					if (vehicleInfo != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null
						&& !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)
						&& !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)
						&& vehicleInfo.Id != TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia) {
						_webSession.Graphics = false;
						ResultsOptionsWebControl1.GraphRadioButton.Checked = false;
						ResultsOptionsWebControl1.GraphRadioButton.Visible = false;
						ResultsOptionsWebControl1.TableRadioButton.Visible = false;
						_webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicle;
						_webSession.Save();
					}					
				}
				else {
					ResultsOptionsWebControl1.GraphRadioButton.Visible = true;
					ResultsOptionsWebControl1.TableRadioButton.Visible = true;
				}

                if (!IsPostBack)
                {

                    #region Création de totalRadioButtonList
                    totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188, _webSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
                    totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1189, _webSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()));
                    totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1190, _webSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()));
                    totalRadioButtonList.Items[0].Selected = true;
                    totalRadioButtonList.CssClass = "txtNoir11";
                    #endregion

                    ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
                    ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
                }
                else
                {
                    if (_webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS
                        && (!ResultsOptionsWebControl1.GraphRadioButton.Checked && !ResultsOptionsWebControl1.TableRadioButton.Checked)
                        )
                    {
                        ResultsOptionsWebControl1.GraphRadioButton.Checked = _webSession.Graphics;
                        ResultsOptionsWebControl1.TableRadioButton.Checked = !_webSession.Graphics;
                    }
                    else
                    {
                        _webSession.Graphics = ResultsOptionsWebControl1.GraphRadioButton.Checked;
                    }
                }

                if (_webSession.CurrentTab == CstResult.SynthesisRecap.PALMARES && _webSession.Graphics)
                {
                    for (int itemIndex = 0; itemIndex < totalRadioButtonList.Items.Count; itemIndex++)
                        if (totalRadioButtonList.Items[itemIndex] != null) totalRadioButtonList.Items[itemIndex].Enabled = false;
                }
                else
                {
                    for (int itemIndex = 0; itemIndex < totalRadioButtonList.Items.Count; itemIndex++)
                        if (totalRadioButtonList.Items[itemIndex] != null) totalRadioButtonList.Items[itemIndex].Enabled = true;
                }

                #region Suppression du total univers
                if ((_webSession.CurrentTab == CstResult.PalmaresRecap.SEASONALITY 
                    || _webSession.CurrentTab == CstResult.PalmaresRecap.MEDIA_STRATEGY)
                    && _webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS
                    )
                {
                    totalRadioButtonList.Items.Remove(totalRadioButtonList.Items.FindByValue("2"));
                }
                #endregion

                #region Ajout du total univers
                if ((_webSession.CurrentTab == CstResult.PalmaresRecap.PALMARES 
                    || _webSession.CurrentTab == CstResult.PalmaresRecap.NOVELTY)
                    && _webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS)
                {
                    try
                    {
                        if (totalRadioButtonList.Items.FindByValue("2").Value != null) { }
                    }
                    catch (Exception)
                    {
                        ListItem listitem = new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188, _webSession.SiteLanguage), CstComparisonCriterion.universTotal.GetHashCode().ToString());
                        if (_webSession.CurrentTab == CstResult.SynthesisRecap.PALMARES && _webSession.Graphics) listitem.Enabled = false;
                        else listitem.Enabled = true;
                        totalRadioButtonList.Items.Insert(0, listitem);
                    }
                }
                #endregion

                #region radioButton sélectionné
                if (_webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS)
                {
                    try
                    {
                        totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Selected = true;
                    }
                    catch (Exception)
                    {
                        if (_webSession.CurrentTab == CstResult.PalmaresRecap.PALMARES || _webSession.CurrentTab == CstResult.PalmaresRecap.NOVELTY)
                        {
                            totalRadioButtonList.Items.FindByValue(CstComparisonCriterion.universTotal.GetHashCode().ToString()).Selected = true;
                        }
                        else if (_webSession.CurrentTab == CstResult.PalmaresRecap.SEASONALITY || _webSession.CurrentTab == CstResult.PalmaresRecap.MEDIA_STRATEGY)
                        {
                            totalRadioButtonList.Items.FindByValue(CstComparisonCriterion.sectorTotal.GetHashCode().ToString()).Selected = true;
                        }
                    }
                }
                #endregion

                #region ComparaisonCriterion
                if (_webSession.CurrentTab != CstResult.PalmaresRecap.EVOLUTION && _webSession.CurrentTab != CstResult.SynthesisRecap.SYNTHESIS)
                {
                    _webSession.ComparaisonCriterion = (CstComparisonCriterion)Convert.ToInt32(totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Value);
                }
                #endregion

                #region Résultat

                ResultsOptionsWebControl1.ResultFormat = true;
                InitializeProductWebControl1.Visible = true;
                ResultsOptionsWebControl1.MediaDetailOption = false;
                bigFormat = false;


                try
                {
                    //Summary
                    if (_webSession.CurrentTab == CstResult.SynthesisRecap.SYNTHESIS)
                    {
                        totalChoice = false;
                        ResultsOptionsWebControl1.ResultFormat = false;
                        InitializeProductWebControl1.Visible = false;
                    }
                    //Seasonality and palmares
                    else if (_webSession.CurrentTab == CstResult.Seasonality.SEASONALITY || _webSession.CurrentTab == CstResult.PalmaresRecap.PALMARES)
                    {
                        totalChoice = true;
                        if (_webSession.CurrentTab == CstResult.Seasonality.SEASONALITY && _webSession.Graphics)
                        {
                            bigFormat = true;
                            ProductClassContainerWebControl1.BigSize = this.bigFormatCheckBox.Checked;
                        }
                    }
                    //Novelty
                    else if (_webSession.CurrentTab == CstResult.Novelty.NOVELTY)
                    {
                        totalChoice = false;
                    }

                    //EVOL
                    else if (_webSession.CurrentTab == CstResult.PalmaresRecap.EVOLUTION)
                    {
                        totalChoice = false;

                        if (_webSession.Graphics)
                        {
                            //Cas année N-2
                            DateTime PeriodBeginningDate = FctUtilities.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                            if ((PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - 2) && DateTime.Now.Year <= _webSession.DownLoadDate)
                                || (PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - 3) && DateTime.Now.Year > _webSession.DownLoadDate)
                                )
                            {
                                _webSession.Graphics = false;
                            }
                        }
                    }

                    //Media Strategy
                    else if (_webSession.CurrentTab == CstResult.MediaStrategy.MEDIA_STRATEGY)
                    {
                        ResultsOptionsWebControl1.MediaDetailOption = true;
                        totalChoice = true;
                        if (_webSession.Graphics)
                        {
							if (vehicleInfo != null ) {
								// si WebSession est au niveau media on se met au niveau categorie/media
								if (_webSession.PreformatedMediaDetail == CstPreformatedDetail.PreformatedMediaDetails.vehicle && vehicleInfo.Id != CstDBClassif.Vehicles.names.plurimedia) {
									_webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory;
								}
							}
                        }
                    }

                }
                catch (System.Exception)
                {
                    result = noResult("");
                }
                #endregion

                #region MAJ _webSession
                _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                _webSession.Save();
                #endregion

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ModuleBridgeWebControl1.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;		
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
            ProductClassContainerWebControl1.Session = _webSession;
			
			return tmp;
		}
		#endregion

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
		private void InitializeComponent(){
           
		}
		#endregion

		#region Aucun Résultat
		/// <summary>
		/// Affichage d'un message d'erreur
		/// </summary>
		/// <returns></returns>
		private string noResult(string message){
			System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
			t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
			t.Append(GestionWeb.GetWebWord(177,_webSession.SiteLanguage)+" "+message);
			t.Append("</td></tr></table>");
			return t.ToString();
		} 
		#endregion

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            // Suppime les items inutiles de la list "niveau media" pour le graphe de strategie Media
            if (_webSession.CurrentTab == CstResult.PalmaresRecap.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked)
            {
				VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
				if (vehicleInfo != null) {
					switch (vehicleInfo.Id) {
						case CstDBClassif.Vehicles.names.tv:
						case CstDBClassif.Vehicles.names.radio:
						case CstDBClassif.Vehicles.names.outdoor:
						case CstDBClassif.Vehicles.names.mediasTactics:
						case CstDBClassif.Vehicles.names.mobileTelephony:
						case CstDBClassif.Vehicles.names.emailing:
							ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
							break;
						case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.newspaper:
                        case CstDBClassif.Vehicles.names.magazine:
						case CstDBClassif.Vehicles.names.internet:
							ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
							break;
						case CstDBClassif.Vehicles.names.cinema:
							result = noResult("");
							break;
						case CstDBClassif.Vehicles.names.plurimedia:
							ResultsOptionsWebControl1.mediaDetail.Enabled = true;
							break;
						default:
							ResultsOptionsWebControl1.mediaDetail.Enabled = false;
							break;
					}
				}
            }
            base.Render(output);
        }
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Get next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return this.MenuWebControl2.NextUrl;
		}
		#endregion

	}
}
