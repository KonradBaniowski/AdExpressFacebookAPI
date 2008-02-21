#region Informations
// Auteur: D.V. MuSSUMA
// Date de création: 21/09/2004
// Date de modification: 21/09/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;
using CustomerWebConstantes=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Calendrier afficher en Années,mois
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MonthCalendarWebControl runat=server></{0}:MonthCalendarWebControl>")]
	public class MonthCalendarWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Enumérateur
		/// <summary>
		/// Type du calendrier
		/// Cette énumérateur sert à déterminer le titre du calendrier
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Indéfini
			/// </summary>
			unknown,
			/// <summary>
			/// Date de début
			/// </summary>
			dateBegin,
			/// <summary>
			/// Date de fin
			/// </summary>
			dateEnd
		}

		/// <summary>
		/// Type d'affichage
		/// </summary>
		public enum Display
		{
//			/// <summary>
//			/// Mois et semaines
//			/// </summary>
//			all,
			/// <summary>
			/// mois
			/// </summary>
			month,
//			/// <summary>
//			/// semaine
//			/// </summary>
//			week
		}
		#endregion

		#region Constantes
		/// <summary>
		/// Titre du début de période en français
		/// </summary>
		private const string TITLE_DATE_BEGIN_PICTURE="Starting_date.gif";
		/// <summary>
		/// Titre de la fin de période en français
		/// </summary>
		private const string TITLE_DATE_END_PICTURE="Ending_date.gif";
		/// <summary>
		/// Couleur de la bordure
		/// </summary>
		private const string TABLE_BORDER_COLOR="#9D9885";
		/// <summary>
		/// Couleur de fond
		/// </summary>
		private const string TABLE_COLOR="#FFFFFF";
		/// <summary>
		/// Couleur de fond de la date sélectionnée
		/// </summary>
		private const string TABLE_COLOR_SELECTED_DATE="#644883";
		/// <summary>
		/// Couleur de fond du titre dans les colonnes
		/// </summary>
		private const string TITLE_BACKGROUND_COLOR="#644883";
		/// <summary>
		/// Couleur de la font
		/// </summary>
		private const string FONT_COLOR="#644883";
		/// <summary>
		/// Couleur de la font de la date sélectionnée
		/// </summary>
		private const string FONT_COLOR_SELECTED_DATE="#FFFFFF";
		/// <summary>
		/// Couleur de fond quand il n'y a pas de date
		/// </summary>
		private const string TABLE_COLOR_NO_DATE="#9378B3";
		/// <summary>
		/// Couleur de fond cellule
		/// </summary>
		private const string TD_BGCOLOR="#FFFFFF";

		#endregion

		#region Variables
		/// <summary>
		/// Type d'affichage
		/// </summary>
		private MonthCalendarWebControl.Display display=MonthCalendarWebControl.Display.month;
		/// <summary>
		/// Année du début de calendrier
		/// </summary>
		protected int startYear;
		/// <summary>
		/// Année du fin de calendrier
		/// </summary>
		protected int stopYear;
		/// <summary>
		/// Année sélectionnée
		/// </summary>
		protected int selectedYear=DateTime.Now.Year;
		/// <summary>
		/// Date sélectionnée
		/// </summary>
		protected int selectedDate;
		/// <summary>
		/// Type de date sélectionnée
		/// </summary>
		protected CustomerWebConstantes.Period.Type selectedDateType;

		/// <summary>
		/// Langue utilisé
		/// </summary>
		private int language;
		/// <summary>
		/// type du calendrier, il détermine le titre du calendrier
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type calendarType=Type.unknown;
		/// <summary>
		/// Calendrier en mémoire
		/// </summary>
		private YearMonthCalendar memoryCalendar=null;
		/// <summary>
		/// Date de chargement pour les recaps
		/// </summary>
		protected int downloadDate=0;

		#endregion

		#region Accesseurs
		/// <summary>
		/// Ontient et définit l'année à afficher
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("2005")] 
		public int SelectedYear
		{
			get{return selectedYear;}
			set
			{
				selectedYear = value;
				if(memoryCalendar==null)new YearMonthCalendar(value); 
				else memoryCalendar.Year=value;
			}
		}

		/// <summary>
		/// Ontient et définit la langue à utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public int Language
		{
			get{return language;}
			set{language = value;}
		}
		
	
		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Ontient et définit le type de calendrier
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("33")] 
		public MonthCalendarWebControl.Type CalendarType
		{
			get{return calendarType;}
			set{calendarType = value;}
		}

		/// <summary>
		/// Définit le niveau d'affichage
		/// </summary>
		public MonthCalendarWebControl.Display DisplayType
		{
			set{this.display = value;}
		}

		/// <summary>
		/// Obtient le type de période sélectionné
		/// </summary>
		public CustomerWebConstantes.Period.Type SelectedDateType
		{
			get{return(this.selectedDateType);}
		}

		/// <summary>
		/// Obtient la période sélectionnée
		/// </summary>
		public int SelectedDate
		{
			get{return(this.selectedDate);}
		}

		#endregion
		
		#region Evènements

		#region Initialisation
		/// <summary>
		/// Initialisation de l'objet
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) 
		{
			base.OnInit (e);
			
			// Attaquer une table
			downloadDate=webSession.DownLoadDate;

			if(downloadDate==DateTime.Now.Year){
				startYear=selectedYear-2;
				stopYear=DateTime.Now.Year;
			}
			else{
				selectedYear=selectedYear-1;
				startYear=selectedYear-2;
				stopYear=DateTime.Now.Year-1;
			}
			selectedDate=-1;
			if (this.ViewState["selectedDate"]!=null)
			{
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
		}

		#endregion

		#region Chargement
		/// <summary>
		/// Evènement de chargement du composant
		/// </summary>
		/// <param name="e">Argument</param>
		protected override void OnLoad(EventArgs e) 
		{
			if (this.ViewState["selectedDate"]!=null)
			{
				selectedDate=(int)this.ViewState["selectedDate"];
				selectedDateType=(CustomerWebConstantes.Period.Type)this.ViewState["selectedDateType"];
			}
			if (this.ViewState["selectedYear"]!=null)
			{
				selectedYear=(int)this.ViewState["selectedYear"];
			}
			if (Page.IsPostBack)
			{
				// TEST A FAIRE POUR VERIFIER QUE L'ON PREND LE BON CALENDARMONTH
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput==this.ID)
				{
					string valueInput=Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
					switch(valueInput.Length)
					{
						case 4:
							selectedYear=int.Parse(valueInput);
							ViewState.Add("selectedYear",selectedYear);
							//memoryCalendar=new YearMonthWeekCalendar(this.selectedYear);
							break;
						case 6:
							selectedDate=int.Parse(valueInput);
							ViewState.Add("selectedDate",selectedDate);
							selectedDateType=CustomerWebConstantes.Period.Type.dateToDateMonth;
							ViewState.Add("selectedDateType",selectedDateType);							
							break;						
					}
				}	
			}
		}
		#endregion

		#region Affichage
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			StringBuilder htmlBuilder=new StringBuilder(10500);
			string dateSelectedString="&nbsp;";
			if(isDateSelected())
			{
				dateSelectedString=MonthString.Get(int.Parse(selectedDate.ToString().Substring(4,2)),language,12)+"&nbsp;"+selectedYear.ToString();
				
			}
			htmlBuilder.Append("<table cellspacing=1 cellpadding=0 border=0 bgcolor="+TABLE_BORDER_COLOR+">");
			string tmpHTML="";
			string titleImage=this.getTitle();
			// Titre
			if(titleImage.Length>0)
			{
				tmpHTML="<td colspan=6 align=\"center\"><img src=\"/Images/"+language+"/Calendar/"+titleImage+"\"></td>";
			}
			else
			{
				tmpHTML="<td colspan=6 align=\"center\">&nbsp;</td>";
			}
			// Flèche
			string left_arrow_HTML;
			string right_arrow_HTML;
			//Old version
			if(selectedYear>startYear)
				left_arrow_HTML="<td align=\"left\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear-1).ToString()+"')\"><img border=0 src=\"/Images/"+language+"/Calendar/Arrow_left_up.gif\"></a></td>";
			else
				left_arrow_HTML="<td><img width=\"11px\" src=\"/Images/Common/pixel.gif\"></td>";
			if(selectedYear<stopYear)
				right_arrow_HTML="<td align=\"right\"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear+1).ToString()+"')\"><img border=0 src=\"/Images/"+language+"/Calendar/Arrow_right_up.gif\"></a></td>";
			else
				right_arrow_HTML="<td><img width=\"11px\" src=\"/Images/Common/pixel.gif\"></td>";

			

			htmlBuilder.Append("<tr bgcolor="+TABLE_COLOR+">");
			htmlBuilder.Append(tmpHTML);
			htmlBuilder.Append("</tr>");
			//Affichage année en cours
			htmlBuilder.Append("<tr>");

			htmlBuilder.Append("<td>");
			htmlBuilder.Append("<table width=\"100%\"cellspacing=0 cellpadding=0 border=0 bgcolor="+TABLE_COLOR+">");
			htmlBuilder.Append("<tr>");

			htmlBuilder.Append(left_arrow_HTML);
			htmlBuilder.Append("<td colspan=3 align=\"center\"><font size=1 color="+FONT_COLOR+" face=\"Arial\">"+this.selectedYear.ToString()+"</font></td>");
			htmlBuilder.Append(right_arrow_HTML);

			htmlBuilder.Append("</tr>");
			htmlBuilder.Append("</table>");
			htmlBuilder.Append("</td>");

			htmlBuilder.Append("</tr>");
			//Titre  colonne mois
			htmlBuilder.Append("<tr bgcolor=\""+TITLE_BACKGROUND_COLOR+"\" >");
			htmlBuilder.Append("<td colspan=6 align=\"center\"><img src=\"/Images/"+Language.ToString()+"/Calendar/monthTitle.gif\" border=0></td>");			
			htmlBuilder.Append("</tr>");
			int i;
			if(memoryCalendar==null) memoryCalendar=new YearMonthCalendar(this.selectedYear);
			int[] months=this.memoryCalendar.MonthsTable;
			for(i=0;i<months.Length;i++)
			{
				htmlBuilder.Append("<tr>");
				if(display==MonthCalendarWebControl.Display.month) htmlBuilder.Append("<td colspan=6 bgcolor="+TD_BGCOLOR+"><a href=\"javascript:__doPostBack('"+this.ID+"','"+(this.selectedYear).ToString()+(i+1).ToString("00")+"')\"><img border=0 src=\"/Images/"+language.ToString()+"/Calendar/Month_"+(i+1).ToString()+".gif\"></a></td>");				
				htmlBuilder.Append("</tr>");
			}
			htmlBuilder.Append("<tr><td colspan=6  bgcolor="+TABLE_COLOR_SELECTED_DATE+"><font size=1 color="+FONT_COLOR_SELECTED_DATE+" face=\"Arial\">"+dateSelectedString+"</font></td></tr>");
			htmlBuilder.Append("</table>");
			output.Write(htmlBuilder.ToString());
		}
		#endregion

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Donne le titre du calendar
		/// </summary>
		/// <returns></returns>
		private string getTitle()
		{
			switch(this.calendarType)
			{
				case TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type.dateBegin:
					return(MonthCalendarWebControl.TITLE_DATE_BEGIN_PICTURE);
				case TNS.AdExpress.Web.Controls.Selections.MonthCalendarWebControl.Type.dateEnd:
					return(MonthCalendarWebControl.TITLE_DATE_END_PICTURE);
			}
			return("");
		}
		#endregion

		#region Méthode externes

		/// <summary>
		/// Indique si une date est sélectionnée
		/// </summary>
		/// <returns>True si une date est sélectionnée, false sinon</returns>
		public bool isDateSelected()
		{
			if(selectedDate>0) return(true);
			else return(false);
		}

		#endregion

		
	}
}
