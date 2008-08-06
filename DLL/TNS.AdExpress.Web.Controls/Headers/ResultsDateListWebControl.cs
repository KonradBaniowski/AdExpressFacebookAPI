#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 10/02/20045
// Date de modification: 10/02/2005
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Contr�le qui contruit une liste de dates hebdomadaires,mensuelles 
	/// </summary>
	[DefaultProperty("Text"),ToolboxData("<{0}:ResultsDateListWebControl runat=server></{0}:ResultsDateListWebControl>")]
	public class ResultsDateListWebControl : System.Web.UI.WebControls.DropDownList{
	
		#region Variables
		/// <summary>
		/// Niveau de d�tail p�riode (ann�e,mois ou semaine)
		/// </summary>
		protected CustomerSessions.Period.DisplayLevel  detailPeriod;				
		/// <summary>
		/// Session client
		/// </summary>
		protected WebSession webSession;
		#endregion

		#region Accesseurs
		
		/// <summary>
		/// Obtient et d�finit le type du module
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(CustomerSessions.Period.DisplayLevel.yearly)] 
		public CustomerSessions.Period.DisplayLevel DetailPeriod{
			get{return detailPeriod;}
			set{detailPeriod = value;}
		}
				
		/// <summary>
		/// Obtient ou d�finit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		#endregion

		#region Ev�nements

		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {	
	
			
			#region initialisation de la liste des dates
			this.Items.Clear();	
			
			switch(detailPeriod){
				case CustomerSessions.Period.DisplayLevel.monthly :				
					//Dates de l'ann�e N et N-1 mensuelles
					FillMonthlyDate(DateTime.Now,1);					
					this.Attributes["onchange"]="javascript:SelectPeriod('ResultsDateListWebControl1');";
					break;
				case CustomerSessions.Period.DisplayLevel.weekly :					
					//Dates de l'ann�e N et N-1 hebdomadaires
					this.Items.Clear();
					AtomicPeriodWeek startPeriod = new AtomicPeriodWeek(DateTime.Now);			
//					startPeriod.SubWeek(2);	
					startPeriod.SubWeek(1);	
					FillWeeklyDate(startPeriod);
					if(webSession.DetailPeriod==CustomerSessions.Period.DisplayLevel.weekly){
						try{
							this.Items.FindByValue(webSession.PeriodBeginningDate).Selected = true;
						}
						catch(System.Exception){
							this.Items.FindByValue("0").Selected = true;				
						}
					}
					this.Attributes["onchange"]="javascript:SelectPeriod('ResultsDateListWebControl2');";	
					break;
				default :
					throw(new WebControlInitializationException("Il est impossible de d�finir une liste de dates � afficher."));
			}			

			#endregion

			base.OnInit(e);
		}

		/// <summary>
		/// Even�ment de pr�rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
		}
		#endregion

		#region M�thodes internes	

		/// <summary>
		///Cr�e une liste de dates hebdomadaires	 
		/// </summary>
		///<param name="startPeriod">debut de la p�riode</param>	
		private void FillWeeklyDate(AtomicPeriodWeek startPeriod){
			//Cr�e une liste de dates hebdomadaires	
			string ItemValue;
			this.Items.Add(new ListItem("-------------------------------------------","0"));			
			for(int i=1;i<=53;i++){							
				ItemValue=startPeriod.Week.ToString().Length==1?startPeriod.Year.ToString()+"0"+startPeriod.Week.ToString() : startPeriod.Year.ToString()+startPeriod.Week.ToString();
				this.Items.Add(new ListItem(GestionWeb.GetWebWord(124,webSession.SiteLanguage)+"  "+ WebFunctions.Dates.dateToString(startPeriod.FirstDay,webSession.SiteLanguage)+"  "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+"  "+WebFunctions.Dates.dateToString(startPeriod.LastDay,webSession.SiteLanguage),ItemValue));						
				startPeriod.SubWeek(1);
			}
		}

		/// <summary>
		/// Cr�e une liste de dates mensuelles	
		/// </summary>
		/// <param name="dt">date</param>
		/// <param name="NbMonth">Le nombre de mois � retrancher</param>
		private void FillMonthlyDate(DateTime dt,int NbMonth){
			string ItemValue="";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			if(NbMonth>0){
				dt=dt.AddMonths(-NbMonth);			
				this.Items.Add(new ListItem("------------------------------","0"));
			}					
			//mois ann�e N
			for(int i=dt.Month;i>0;i--){	
				ItemValue=i.ToString().Length==1?dt.Year.ToString()+"0"+i.ToString():dt.Year.ToString()+i.ToString();
				this.Items.Add(new ListItem(MonthString.GetCharacters(i,cultureInfo,0)+" "+dt.Year.ToString(),ItemValue));												
			}
			//mois ann�e N-1
			dt = dt.AddYears(-1);
			for(int j=12;j>0;j--){	
				ItemValue=j.ToString().Length==1?dt.Year.ToString()+"0"+j.ToString():dt.Year.ToString()+j.ToString();
				this.Items.Add(new ListItem(MonthString.GetCharacters(j,cultureInfo,0)+" "+dt.Year.ToString(),ItemValue));												
				if(j==dt.Month)break;//Ann�e coulante : la dernier mois (chiffre) de l'ann�e N-1 correspond au dernier mois de l'ann�e N � afficher	
			}
			if(webSession.DetailPeriod==CustomerSessions.Period.DisplayLevel.monthly){
				try{
					this.Items.FindByValue(webSession.PeriodBeginningDate).Selected = true;
				}
				catch(System.Exception){
					this.Items.FindByValue("0").Selected = true;				
				}
			}
		}
		#endregion

	}
}
