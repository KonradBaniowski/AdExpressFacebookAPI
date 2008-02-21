#region Informations
// Auteur: G. Facon 
// Date de création: 17/05/2004 
// Date de modification: 17/05/2004 
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Contrôle qui contruit une liste de numéros pour la sélection des dates glissantes
	/// </summary>
	[DefaultProperty("Text"),ToolboxData("<{0}:DateListWebControl runat=server></{0}:DateListWebControl>")]
	public class DateListWebControl : System.Web.UI.WebControls.DropDownList{

		#region Constantes

		/// <summary>
		/// Nombre de jours à afficher pour l'alerte 
		/// </summary>
		private const int ALERT_DAYS_NUMBER=31;
		/// <summary>
		/// Nombre de semaines à afficher pour l'alerte 
		/// </summary>
		private const int ALERT_WEEK_NUMBER=16;
		/// <summary>
		/// Nombre de mois à afficher pour l'alerte 
		/// </summary>
		private const int ALERT_MONTH_NUMBER=4;
		/// <summary>
		/// Nombre de semaines à afficher pour l'analyse
		/// </summary>
		private const int ANALYSIS_WEEK_NUMBER=53;		
		/// <summary>
		/// Nombre de mois à afficher pour l'analyse
		/// </summary>
		private const int ANALYSIS_MONTH_NUMBER=12;
		/// <summary>
		/// Nombre d'année à afficher pour l'analyse
		/// </summary>
		private const int ANALYSIS_YEAR_NUMBER=3;

		/// <summary>
		/// Nombre de semaines à afficher pour l'APPM 
		/// </summary>
		private const int APPM_WEEK_NUMBER=70;
		/// <summary>
		/// Nombre de mois à afficher pour l'APPM 
		/// </summary>
		private const int APPM_MONTH_NUMBER=16;

		/// <summary>
		/// Nombre de semaines à afficher pour le PARRAINAGE 
		/// </summary>
		private const int SPONSORSHIP_WEEK_NUMBER=106;
		/// <summary>
		/// Nombre de mois à afficher pour le PARRAINAGE  
		/// </summary>
		private const int SPONSORSHIP_MONTH_NUMBER=24;
		#endregion

		#region Enumérateurs
		/// <summary>
		/// Type de la liste
		/// </summary>
		public enum ListType{
			/// <summary>
			/// Jour
			/// </summary>
			day,
			/// <summary>
			/// Semaine
			/// </summary>
			week,
			/// <summary>
			/// Mois
			/// </summary>
			month,
			/// <summary>
			/// Année
			/// </summary>
			year
		}
		#endregion

		#region Variables
		/// <summary>
		/// Type du module
		/// </summary>
		protected WebConstantes.Module.Type moduleType=WebConstantes.Module.Type.alert;
		/// <summary>
		/// Type de la liste qui s'affiche
		/// </summary>
		protected ListType listTypeDisplay=ListType.day;
		/// <summary>
		/// Nombre d'années accessibles
		/// </summary>
		protected int nbYearsToDisplay=0;
		/// <summary>
		/// Session client
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Période courante
		/// </summary>
		protected  AtomicPeriodWeek currentPeriod;
		/// <summary>
		///Nombre de semaines à afficher pour tableaux de bord
		/// </summary>
		private  int DASHBOARD_WEEK_NUMBER;
		#endregion
	
		#region Accesseurs
		/// <summary>
		/// Obtient et définit le type du module
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(WebConstantes.Module.Type.alert)] 
		public WebConstantes.Module.Type ModuleType{
			get{return moduleType;}
			set{moduleType = value;}
		}
		/// <summary>
		/// Obtient et définit le type du module
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(ListType.day)] 
		public ListType ListTypeDisplay{
			get{return listTypeDisplay;}
			set{listTypeDisplay = value;}
		}

		/// <summary>
		/// Définit le nombre d'année
		/// </summary>
		public int NbYearsToDisplay{
			set{this.nbYearsToDisplay=value;}
		}

		
		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		#endregion




		#region Evènements

		/// <summary>

		/// Initialisation

		/// </summary>

		/// <param name="e">Arguments</param>

		protected override void OnInit(EventArgs e) {

			int i;
			int fin = 0;

			currentPeriod = new AtomicPeriodWeek(DateTime.Now);

			DASHBOARD_WEEK_NUMBER = currentPeriod.Week;

			if (this.listTypeDisplay == ListType.day && moduleType == WebConstantes.Module.Type.analysis)

				throw (new WebControlInitializationException("Il est impossible de définir une liste de jour pour une analyse"));

			if (this.listTypeDisplay == ListType.day && moduleType == WebConstantes.Module.Type.alert) fin = ALERT_DAYS_NUMBER;

			if (this.listTypeDisplay == ListType.week && moduleType == WebConstantes.Module.Type.analysis) fin = ANALYSIS_WEEK_NUMBER;

			if (this.listTypeDisplay == ListType.week && moduleType == WebConstantes.Module.Type.alert) fin = ALERT_WEEK_NUMBER;

			if (this.listTypeDisplay == ListType.month && moduleType == WebConstantes.Module.Type.analysis) fin = ANALYSIS_MONTH_NUMBER;

			if (this.listTypeDisplay == ListType.month && moduleType == WebConstantes.Module.Type.alert) fin = ALERT_MONTH_NUMBER;

			//UnresolvedMergeConflict : Modification GR - 14/05/2007 - Modification Homepage                

			//if (this.listTypeDisplay==ListType.week && moduleType==WebConstantes.Module.Type.dashBoard)fin=DASHBOARD_WEEK_NUMBER;

			if (this.listTypeDisplay == ListType.week &&

				  (webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD

				  || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO

				  || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE

				  || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO

				  || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION)

				  ) fin = DASHBOARD_WEEK_NUMBER;

			//Fin Modification GR - 14/05/2007 - Modification Homepage

			if (this.listTypeDisplay == ListType.week && moduleType == WebConstantes.Module.Type.chronoPress) fin = APPM_WEEK_NUMBER;

			if (this.listTypeDisplay == ListType.month && moduleType == WebConstantes.Module.Type.chronoPress) fin = APPM_MONTH_NUMBER;

			if (this.listTypeDisplay == ListType.week && moduleType == WebConstantes.Module.Type.tvSponsorship) fin = SPONSORSHIP_WEEK_NUMBER;

			if (this.listTypeDisplay == ListType.month && moduleType == WebConstantes.Module.Type.tvSponsorship) fin = SPONSORSHIP_MONTH_NUMBER;



			if (this.listTypeDisplay == ListType.year && moduleType == WebConstantes.Module.Type.analysis) {

				if (nbYearsToDisplay != 0) fin = nbYearsToDisplay;

				else fin = ANALYSIS_YEAR_NUMBER;

			}



			if (DateTime.Now.Year > webSession.DownLoadDate) {

				//UnresolvedMergeConflict : Modification GR - 14/05/2007 - Modification Homepage              

				//if (this.listTypeDisplay==ListType.month  && moduleType==WebConstantes.Module.Type.recap)fin=int.Parse(webSession.LastAvailableRecapMonth.Substring(4,2));

				if (this.listTypeDisplay == ListType.month &&

					 (webSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR

					 || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)

				) fin = int.Parse(webSession.LastAvailableRecapMonth.Substring(4, 2));

				//Fin Modification GR - 14/05/2007 - Modification Homepage

			}

			else {

				//UnresolvedMergeConflict : Modification GR - 14/05/2007 - Modification Homepage              

				//if (this.listTypeDisplay==ListType.month  && moduleType==WebConstantes.Module.Type.recap)fin=DateTime.Now.Month;

				if (this.listTypeDisplay == ListType.month &&

					 (webSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR

					 || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)

				) fin = DateTime.Now.Month;

				//Fin Modification GR - 14/05/2007 - Modification Homepage

			}

			if (this.listTypeDisplay == ListType.year && moduleType == WebConstantes.Module.Type.alert)

				throw (new WebControlInitializationException("Il est impossible de définir une liste d'année pour une alerte"));



			this.Items.Clear();

			this.Items.Add(new System.Web.UI.WebControls.ListItem("----", "0"));

			for (i = 1; i <= fin; i++) {

				this.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));

			}



			base.OnInit(e);

		}

 



		/// <summary>
		/// Evenèment de prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
		}
		#endregion

	}
}
