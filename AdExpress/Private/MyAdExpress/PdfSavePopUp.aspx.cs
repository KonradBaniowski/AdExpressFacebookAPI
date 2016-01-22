#region Informations
// Auteur: D. Mussuma
// Date de création: 02/02/2007
#endregion

using System;
using System.Web.UI.WebControls;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Constantes.Web;
using System.Collections.Generic;

namespace AdExpress.Private.MyAdExpress
{
    /// <summary>
    /// Page de demande d'export de résultat au format PDF
    /// </summary>
    public partial class PdfSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage
    {
        #region Variables

        #region variables fiche justificatives
        /// <summary>
        /// Identifiant du media
        /// </summary>
        private string _idMedia = null;
        /// <summary>
        /// Identifiant du produit
        /// </summary>
        private string _idProduct = null;
        /// <summary>
        /// Date faciale
        /// </summary>
        private string _dateCover = null;
        /// <summary>
        /// Date parution
        /// </summary>
        private string _dateParution = null;
        /// <summary>
        /// Page
        /// </summary>
        private string _pageNumber = null;
        #endregion

        #region Variables Plan média
        /// <summary>
        /// Zoom parameters in Media Plan
        /// </summary>
        string zoomDate = string.Empty;
        #endregion

        private string _idDataPromotion = null;
        private string _resultType = null;
        protected bool _canGroupCreativesExport = false;
        protected string _levelsValue = string.Empty;
        protected string _dateBegin = string.Empty;
        protected string _dateEnd = string.Empty;
        private string _idUnit = string.Empty;
        private string _idModule = string.Empty;
        #endregion

        #region Varaibles MMI

        /// <summary>
        /// Libellé du nom de fichier
        /// </summary>
        /// <summary>
        /// Libellé du mail
        /// </summary>
        /// <summary>
        /// Nom du fichier
        /// </summary>
        /// <summary>
        /// Mail
        /// </summary>
        /// <summary>
        /// Bouton valider
        /// </summary>
        /// <summary>
        /// Titre de la sauvegarde
        /// </summary>

        /// <summary>
        /// Case à cocher mémoriser adresse email 
        /// </summary>
        /// <summary>
        /// Bouton fermer
        /// </summary>


        #endregion

        public bool CanGroupCreativesExport
        {
            get { return _canGroupCreativesExport; }

        }

        #region Constructeur
        /// <summary>
        /// Constructeur : chargement de la session
        /// </summary>
        public PdfSavePopUp()
            : base()
        {


        }
        #endregion

        #region Evènements


        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>		
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                #region Paramètres pour les fiches justificatives
                //Récupération des paramètres de l'url
                if (_webSession.CurrentModule == Module.Name.JUSTIFICATIFS_PRESSE)
                {
                    _idMedia = Page.Request.QueryString.Get("idmedia");
                    _idProduct = Page.Request.QueryString.Get("idproduct");
                    _dateCover = Page.Request.QueryString.Get("dateCover");
                    _dateParution = Page.Request.QueryString.Get("dateParution");
                    _pageNumber = Page.Request.QueryString.Get("page");
                }
                #endregion

                #region Paramètres Plans média
                zoomDate = Page.Request.QueryString.Get("zoomDate");
                #endregion
                _idDataPromotion = Page.Request.QueryString.Get("idDataPromotion");

                _resultType = Page.Request.QueryString.Get("resultType");

                _levelsValue = Page.Request.QueryString.Get("levelsValue");
                _dateBegin = Page.Request.QueryString.Get("datebegin");
                _dateEnd = Page.Request.QueryString.Get("dateend");
                _idUnit = Page.Request.QueryString.Get("u");
                _idModule = Page.Request.QueryString.Get("m");
               

                askremoteexportwebControl1.ResultType = _resultType;

                if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
                {
                    saveTitle.Code = 2932;
                   
                }else if (_webSession.CurrentModule==Module.Name.ANALYSE_DES_DISPOSITIFS)
                {
                    saveTitle.Code = 2942;
                }
                else saveTitle.Code = 1747;

                #region Gestion des cookies

                #region Cookies enregistrement des préférences

               

                #endregion

                #endregion

                // Boutton valider
                if (Request.Form.Get("__EVENTTARGET") == "validateRollOverWebControl")
                {
                    validateRollOverWebControl_Click(this, null);
                }


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
        /// Evaluation de l'évènement PostBack:
        ///		base.DeterminePostBackMode();
        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            try
            {
                askremoteexportwebControl1.CustomerWebSession = _webSession;
                askremoteexportwebControl1.EnableViewState = true;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            return tmp;
        }
        #endregion


        /// <summary>
        /// Femeture de la fenêtre
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        protected void closeRollOverWebControl_Click(object sender, EventArgs e)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeScript", WebFunctions.Script.CloseScript());


        }

        /// <summary>
        /// Lancer une génération
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        protected void validateRollOverWebControl_Click(object sender, EventArgs e)
        {

           
            #region Validation
            string fileName = askremoteexportwebControl1.TbxFileName.Text; 
            string mail = askremoteexportwebControl1.TbxMail.Text; 
            List<int> sel;
            Int64 idStaticNavSession = 0;
            int maxChar = 80;
            try
            {
                if (!string.IsNullOrEmpty(_idModule))
                    _webSession.CurrentModule = long.Parse(_idModule);

                if (fileName == null || mail == null || fileName.Length == 0 || mail.Length == 0)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", WebFunctions.Script.Alert(GestionWeb.GetWebWord(1748, _siteLanguage)));
                }
                else if (_webSession.CurrentModule== Module.Name.ANALYSE_DES_DISPOSITIFS && !string.IsNullOrEmpty(fileName) && fileName.Length > maxChar)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", WebFunctions.Script.Alert(string.Format(GestionWeb.GetWebWord(2946, _siteLanguage),maxChar.ToString())));
                }
                else if (!WebFunctions.CheckedText.CheckedMailText(mail))
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", WebFunctions.Script.Alert(GestionWeb.GetWebWord(2041, _siteLanguage)));
                }               
                else
                {

                    #region Gestion des cookies

                    #region Cookies enregistrement des préférences

                    //Vérifie si le navigateur accepte les cookies
                    if (Request.Browser.Cookies)
                    {
                        WebFunctions.Cookies.SaveEmailForRemotingExport(Page, mail, askremoteexportwebControl1.CbxRegisterMail);//cbxRegisterMail
                    }
                    #endregion

                    #endregion

                    _webSession.ExportedPDFFileName = WebFunctions.CheckedText.CheckedAccentText(fileName);
                    string[] mails = new string[1];
                    mails[0] = mail;
                    _webSession.EmailRecipient = mails;
                    
                   

                    switch (_webSession.CurrentModule)
                    {

                        case Module.Name.BILAN_CAMPAGNE:
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.mnevis);
                            break;
                        //Plan média
                        case Module.Name.ANALYSE_CONCURENTIELLE:
                        case Module.Name.ANALYSE_DYNAMIQUE:
                        case Module.Name.ANALYSE_PLAN_MEDIA:
                        case Module.Name.ANALYSE_PORTEFEUILLE:
                        case Module.Name.ANALYSE_DES_PROGRAMMES:
                        case Module.Name.ALERTE_PORTEFEUILLE:
                        case Module.Name.CELEBRITIES:
                    

                            if (Module.Name.ANALYSE_CONCURENTIELLE == _webSession.CurrentModule && !string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
                            {

                                 sel = new List<int>();
                                CheckBoxList groupAdByCheckBoxList = askremoteexportwebControl1.GroupAdByCheckBoxList;
                                foreach (ListItem it in groupAdByCheckBoxList.Items)
                                {
                                    if (it.Selected) sel.Add(int.Parse(it.Value));
                                }
                                if (sel.Count > 0)
                                    _webSession.CreativesExportOptions = sel;
                                idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.dedoum);

                            }
                            else
                            {
                                #region Classification Filter Init
                                string id = "";
                                string Level = "";
                                if (Page.Request.QueryString.Get("id") != null) id = Page.Request.QueryString.Get("id").ToString();
                                if (Page.Request.QueryString.Get("Level") != null) Level = Page.Request.QueryString.Get("Level").ToString();

                                if (id.Length > 0 && Level.Length > 0)
                                {
                                    SetProduct(int.Parse(id), int.Parse(Level));
                                }
                                #endregion

                                #region Period Detail
                                DateTime begin;
                                DateTime end;
                                if (zoomDate != null && zoomDate != string.Empty)
                                {
                                    if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.weekly)
                                    {
                                        begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                        end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                    }
                                    else
                                    {
                                        begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                        end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                    }
                                    begin = WebFunctions.Dates.Max(begin,
                                        WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType));
                                    end = WebFunctions.Dates.Min(end,
                                        WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType));

                                    _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                                }
                                else
                                {
                                    begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                                    end = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                                    if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                                    {
                                        _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                                    }
                                }
                                _webSession.PeriodBeginningDate = begin.ToString("yyyyMMdd");
                                _webSession.PeriodEndDate = end.ToString("yyyyMMdd");
                                switch (_webSession.PeriodType)
                                {
                                    case ConstantesPeriod.Type.currentYear:
                                    case ConstantesPeriod.Type.dateToDateMonth:
                                    case ConstantesPeriod.Type.dateToDateWeek:
                                    case ConstantesPeriod.Type.LastLoadedMonth:
                                    case ConstantesPeriod.Type.LastLoadedWeek:
                                    case ConstantesPeriod.Type.nextToLastYear:
                                    case ConstantesPeriod.Type.nLastMonth:
                                    case ConstantesPeriod.Type.nLastWeek:
                                    case ConstantesPeriod.Type.nLastYear:
                                    case ConstantesPeriod.Type.previousWeek:
                                    case ConstantesPeriod.Type.previousYear:
                                        _webSession.PeriodType = ConstantesPeriod.Type.dateToDate;
                                        break;
                                }
                                _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                                #endregion

                                if (!string.IsNullOrEmpty(_idUnit))
                                    _webSession.Unit = (CustomerSessions.Unit)int.Parse(_idUnit);

                                idStaticNavSession = (_webSession.CurrentModule== Module.Name.CELEBRITIES) ? TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.apis):
                                    TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.miysis);
                            }
                            break;

                        case Module.Name.ANALYSE_DES_DISPOSITIFS:
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.pachet);                                                          
                            break;
                        case Module.Name.INDICATEUR:
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.hotep);
                            break;
                        case Module.Name.JUSTIFICATIFS_PRESSE:
                            var pDetail = new ProofDetail(_webSession, _idMedia, _idProduct, _dateCover, _dateParution, _pageNumber);
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(pDetail, TNS.AdExpress.Anubis.Constantes.Result.type.shou, _webSession.ExportedPDFFileName);
                            break;
                        case Module.Name.DONNEES_DE_CADRAGE:
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.aton);
                            break;
                        case Module.Name.VP:
                            if (!string.IsNullOrEmpty(_resultType))
                            {
                                if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode())
                                {
                                    if (!string.IsNullOrEmpty(_idDataPromotion)) _webSession.IdPromotion = long.Parse(_idDataPromotion);
                                    idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.selket);
                                }
                                else if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.thoueris.GetHashCode())
                                {
                                    idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.thoueris);
                                }
                            }
                            break;
                        case Module.Name.ROLEX:
                            if (!string.IsNullOrEmpty(_resultType))
                            {
                                if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.ptah.GetHashCode())
                                {
                                    if (!string.IsNullOrEmpty(_levelsValue)) _webSession.SelectedLevelsValue = new List<string>(_levelsValue.Split(',')).ConvertAll(Convert.ToInt64);
                                    _webSession.DetailPeriodBeginningDate = _dateBegin;
                                    _webSession.DetailPeriodEndDate = _dateEnd;
                                    TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.ptah);
                                }
                                else if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.amon.GetHashCode())
                                {
                                     TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.amon);
                                }
                            }
                            break;
                        //default :
                        //throw new AdExpress. Exceptions.PdfSavePopUpException(" Impossssile d'identifier le module.");

                    }


                    closeRollOverWebControl_Click(this, null);

                }
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            #endregion

        }
        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguements</param>
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

        }
        #endregion

        #region Identifiant des éléments de la nomenclature produit
        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        private void SetProduct(int id, int level)
        {
            WebFunctions.ProductDetailLevel.SetProductLevel(_webSession, id, level);
        }
        #endregion



        

       


    }
}

