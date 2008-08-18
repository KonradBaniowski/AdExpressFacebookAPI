#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 20/09/2004
// Date de modification: 20/09/2004
//	30/12/2004  D. Mussuma Int�gration de WebPage
//	01/08/2006 Modification FindNextUrl
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
using Oracle.DataAccess.Client;
using System.Globalization;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Constantes.Customer;
using AdExpressWebControles=TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException=AdExpress.Exceptions;
using TNS.AdExpress.Web.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de s�lection de dates par mois ou par ann�es
	/// Cette Page est utilis�e pour le choix des p�riodes dans l'analyse sectorielle
	/// </summary>
	public partial class SectorDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{		
		
		#region Variables MMI
		/// <summary>
		/// Entete
		/// </summary>
		/// <summary>
		/// Titre et description de la page
		/// </summary>
		/// <summary>
		/// Titre s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Texte �tude comparative p�riode glissante
		/// </summary>
		/// <summary>
		/// choix �tude comparative p�riode glissante
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Texte titre 
		/// </summary>
		/// <summary>
		/// Texte commentaire
		/// </summary>
		/// <summary>
		/// Liste des mois
		/// </summary>
		/// <summary>
		/// Texte du mois s�lectionnable
		/// </summary>
		/// <summary>
		/// Texte de la derni�re p�riode s�lectionn�e
		/// </summary>
		/// <summary>
		/// ann�e courante (N)
		/// </summary>
		/// <summary>
		/// Texte ann�e courante
		/// </summary>
		/// <summary>
		/// choix ann�e pr�c�dente (N-1)
		/// </summary>
		/// <summary>
		/// Texte ann�e pr�c�dente (N-1)
		/// </summary>
		/// <summary>
		/// choix ann�e avant ann�e pr�c�dente (N-2)
		/// </summary>
		/// <summary>
		/// Texte ann�e avant ann�e pr�c�dente (N-2)
		/// </summary>
		/// <summary>
		/// Texte �tude comparative
		/// </summary>
		/// <summary>
		/// Choix �tude comparative
		/// </summary>
		/// <summary>
		/// Bouton valider 2
		/// </summary>
		/// <summary>
		/// Calendrier de s�lection de date de d�but
		/// </summary>
		/// <summary>
		/// Calendrier de s�lection de date de fin
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Index s�lectionn�
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Etude de comparative ?
		/// </summary>
		int selectedComparativeStudy=-1;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Ann�e de chargement des donn�es
		/// </summary>
		public int downloadDate=0;
		#endregion
		
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public SectorDateSelection():base(){			
		}
		#endregion	

		#region Ev�nements
		
		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Options pour la p�riode s�lectionn�e
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);
				#endregion

				#region Rappel des diff�rentes s�lections
//				ArrayList linkToShow=new ArrayList();			
//				if(_webSession.isSelectionProductSelected())linkToShow.Add(2);
//				if(_webSession.isMediaSelected())linkToShow.Add(4);
//				recallWebControl.LinkToShow=linkToShow;
//				if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isSelectionProductSelected() && _webSession.isDatesSelected())recallWebControl.CanGoToResult=true;
				#endregion

				//ATTaquer une table
				downloadDate=_webSession.DownLoadDate;				

				#region Next URL
//				_nextUrl=this.recallWebControl.NextUrl;
//				if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//				else {
//					_nextUrlOk=true;
//				}
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress		
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				//validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				//validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";						
				// Gestion des Calendrier
				this.monthCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthCalendarEndWebControl.Language=_webSession.SiteLanguage;
				#endregion

				#region D�finition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"SectorDateSelectionHelp.aspx";
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Pr�Rendu de la page

		/// <summary>
		/// Ev�nement de Pr�Rendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_PreRender(object sender, System.EventArgs e) 
		{
			if (this.IsPostBack)
			{
				#region Url Suivante
				
				if(_nextUrlOk)
				{
					if(selectedIndex==6 ||selectedIndex==7 ||selectedIndex==8) validateButton1_Click(this, null);
					else validateButton2_Click(this, null);
				}
				#endregion
			}
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

		#region Validation des calendriers
		/// <summary>
		/// Ev�nement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void validateButton1_Click(object sender, System.EventArgs e) 
		{			
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);	
			try
			{
				calendarValidation();
				if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
				else _webSession.ComparativeStudy=false;
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(NoDataException) {
				_webSession.ComparativeStudy=false;
				Response.Write("<script language=\"JavaScript\">alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");</script>");
			}
			catch(System.Exception ex)
			{	_webSession.ComparativeStudy=false;
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}			
		}
		#endregion

		#region Validation des dates avec sauvegarde
		/// <summary>
		/// Ev�nement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void validateButton2_Click(object sender, System.EventArgs e)
		{
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);	
			try
			{
				DateTime downloadDate=new DateTime(_webSession.DownLoadDate,12,31);				
				string absolutEndPeriod ="";
				switch(selectedIndex)
				{
						//Choix N derniers mois
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0)
						{
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);						

							// Cas o� l'ann�e de chargement des donn�es est inf�rieur � l'ann�e actuelle
							if(DateTime.Now.Year>_webSession.DownLoadDate){
								_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
								_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
							}
							else{
								_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
								_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
							}
							
							 absolutEndPeriod = Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
							if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4,2).Equals("00")))
								throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));

							if(int.Parse(absolutEndPeriod)<int.Parse(_webSession.PeriodEndDate))
								_webSession.PeriodEndDate=absolutEndPeriod;

							_webSession.DetailPeriod = CstPeriodDetail.dayly;
							//Activation de l'option etude comparative si selectionn�
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();
						}
						else
						{
							throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
						//Choix ann�e courante
					case 2:
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;						
						// Cas o� l'ann�e de chargement est inf�rieur � l'ann�e en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.ToString("yyyy01");						
							_webSession.PeriodEndDate=downloadDate.ToString("yyyyMM");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.ToString("yyyy01");						
							_webSession.PeriodEndDate=DateTime.Now.ToString("yyyyMM");
						}

						//D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
						//du dernier mois dispo en BDD
						//traitement de la notion de fr�quence
						absolutEndPeriod = Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
						if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4,2).Equals("00")) )
							throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));
												
						_webSession.PeriodEndDate=absolutEndPeriod;

						_webSession.DetailPeriod = CstPeriodDetail.monthly;	
						//Activation de l'option etude comparative si selectionn�
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						break;

						//Choix ann�e pr�cedente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;					
						
						// Cas o� l'ann�e de chargement est inf�rieur � l'ann�e en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.AddYears(-1).ToString("yyyy01");
							_webSession.PeriodEndDate=downloadDate.AddYears(-1).ToString("yyyy12");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-1).ToString("yyyy01");
							_webSession.PeriodEndDate=DateTime.Now.AddYears(-1).ToString("yyyy12");
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						//Activation de l'option etude comparative si selectionn�
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						
						break;

						//Choix ann�e N-2
					case 4:
						if(isComparativeStudy(selectedComparativeStudy))throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						_webSession.PeriodType=CstPeriodType.nextToLastYear;
						_webSession.PeriodLength=1;						
						// Cas o� l'ann�e de chargement est inf�rieur � l'ann�e en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.AddYears(-2).ToString("yyyy01");
							_webSession.PeriodEndDate=downloadDate.AddYears(-2).ToString("yyyy12");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-2).ToString("yyyy01");
							_webSession.PeriodEndDate=DateTime.Now.AddYears(-2).ToString("yyyy12");
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						_webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
					default :
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(NoDataException) {
				Response.Write("<script language=\"JavaScript\">alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");</script>");
			}
			catch(System.Exception ex)
			{
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}	
				
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
            

		}
		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			monthDateList.WebSession=_webSession;
			monthCalendarBeginWebControl.WebSession=_webSession;
			monthCalendarEndWebControl.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			
			return tmp;
		}
		#endregion

		#region M�thodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation()
		{
			// On v�rifie que 2 dates ont �t� s�lectionn�es
			if( monthCalendarBeginWebControl.isDateSelected() && monthCalendarEndWebControl.isDateSelected())
			{		
				// On teste que la date de d�but est inf�rieur � la date de fin
				if(monthCalendarBeginWebControl.SelectedDate> monthCalendarEndWebControl.SelectedDate)
				throw(new AdExpressException.SectorDateSelectionException("La date de d�but doit �tre inf�rieure � la date de fin"));
				
				if(monthCalendarBeginWebControl.SelectedYear != monthCalendarEndWebControl.SelectedYear)
				throw(new AdExpressException.SectorDateSelectionException("Il est n�cessaire de s�lectionner deux ann�es identiques pour valider."));
				
				if(DateTime.Now.Year>_webSession.DownLoadDate){
					if(isComparativeStudy(selectedComparativeStudy) && (monthCalendarBeginWebControl.SelectedYear==DateTime.Now.Year-3 || monthCalendarEndWebControl.SelectedYear==DateTime.Now.Year-3) )
						throw(new AdExpressException.SectorDateSelectionException("Il est n�cessaire de s�lectionner une p�riode sup�rieure � N-2 pour r�aliser une �tude comparative."));
				
				}else{
					if(isComparativeStudy(selectedComparativeStudy) && (monthCalendarBeginWebControl.SelectedYear==DateTime.Now.Year-2 || monthCalendarEndWebControl.SelectedYear==DateTime.Now.Year-2) )
						throw(new AdExpressException.SectorDateSelectionException("Il est n�cessaire de s�lectionner une p�riode sup�rieure � N-2 pour r�aliser une �tude comparative."));
				}


				_webSession.PeriodType =monthCalendarBeginWebControl.SelectedDateType;
				try
				{					
					_webSession.DetailPeriod = CstPeriodDetail.monthly;
				}
				catch(Exception e){
					_webSession.ComparativeStudy=false;
					Response.Write("<script language=\"JavaScript\">alert(\""+e.Message+"\");</script>");
				}
				

				_webSession.PeriodBeginningDate = monthCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = monthCalendarEndWebControl.SelectedDate.ToString();
	
				// On sauvegarde les donn�es
				//D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
				//du dernier mois dispo en BDD
				//traitement de la notion de fr�quence	
				if(int.Parse(monthCalendarEndWebControl.SelectedDate.ToString().Substring(0,4))==DateTime.Now.Year && int.Parse(monthCalendarBeginWebControl.SelectedDate.ToString().Substring(0,4))==DateTime.Now.Year){
					string absolutEndPeriod = Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
					if (int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate))
						throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));

					if(int.Parse(absolutEndPeriod)<int.Parse(_webSession.PeriodEndDate))
						_webSession.PeriodEndDate=absolutEndPeriod;
				}
			}
			else
			{	_webSession.ComparativeStudy=false;
				throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}		
		}
		#endregion
		
		#region selection etude comparative
		/// <summary>
		/// Verifie si c'est une �tude comparative
		/// </summary>
		/// <param name="idStudy">Etude � traiter</param>
		/// <returns>True s'il c'en est une, false sinon</returns>
		private bool isComparativeStudy(Int64 idStudy)
		{			
			switch(idStudy)
			{
				case 5:
				case 8:				
					return(true);
				default:
					return(false);
			}
		}
		#endregion

		#endregion

		#region Impl�mentation m�thodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion
	}
}
