#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 27/06/2005
// Date de modification: 
#endregion
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using DBCst = TNS.AdExpress.Constantes.DB;
using FwkSelectionCst=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Web.Controls.Exceptions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Affiche la liste des vagues AEPM que peut s�lectionner un client .
	/// Le client ne peut s�lectionner qu'une vague AEPM dans la liste (classe d�rivant de RadioButtonList)
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:WaveSelectionWebControl runat=server></{0}:WaveSelectionWebControl>")]
	public class WaveSelectionWebControl : System.Web.UI.WebControls.RadioButtonList {
		
		#region variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 
		/// <summary>
		/// Contr�le titre
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText noWaveAdexpresstext;
		/// <summary>
		/// V�rifie la diponibilit� des vagues
		/// </summary>
		public bool noneWave=false;
		/// <summary>
		/// Goupe de donn�es vagues
		/// </summary>
		private DataSet ds=null;
		/// <summary>
		/// Source de donn�es
		/// </summary>
        private TNS.FrameWork.DB.Common.IDataSource _dataSource = null;
		
		#endregion		
		
		#region Accesseurs
		/// <summary>
		/// D�finit la session � utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}
		/// <summary>
		/// Source de donn�es
		/// </summary>
        public TNS.FrameWork.DB.Common.IDataSource Source
        {
			set{_dataSource=value;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public WaveSelectionWebControl():base() {
			this.EnableViewState=true;		
			
		}

		
		#endregion

		#region Propri�t�s
		
		/// <summary>
		/// Option cochage vague  d'�tude la plus r�cente
		/// </summary>
		[Bindable(true),
		Description("Option coch� la vague d'�tude la plus r�cente")]
		protected bool checkMostRecentWave = true;
		/// <summary>Option cochage vague  d'�tude la plus r�cente</summary>
		public bool CheckMostRecentWave {
			get{return checkMostRecentWave ;}
			set{checkMostRecentWave=value;}
		}
		#endregion

		#region Ev�nements

		#region Init
		/// <summary>
		/// Initialisation du composant
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) {
			DateTime dateBegin;
			dateBegin = new DateTime(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)),01);
			if(webSession!=null)			
			ds=WaveListDataAccess.GetWaveListDataAccess(FwkSelectionCst.Wave.Type.AEPM,_dataSource,dateBegin,APPM.TypeDateBegin.none);	
//				ds=WaveListDataAccess.GetWaveListDataAccess(FwkSelectionCst.Wave.Type.AEPM,DBCst.Connection.GRP_CONNECTION_STRING);						
			if(ds==null || ds.Equals(System.DBNull.Value) || ds.Tables[0]==null || ds.Tables[0].Rows.Count==0)
				noneWave=true;	
			base.OnInit (e);
		}

		#endregion

		#region PreRender
		/// <summary>
		/// M�thode lanc�e avant le rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {	
			

			#region Ajout des items � partir de du groupe donn�es ds
			if(webSession!=null){
				if(ds!=null && ds.Tables[0].Rows.Count>0){
					this.Items.Clear();	
					//Construction de la liste de radibouton
					foreach(DataRow currentRow in ds.Tables[0].Rows) {						
							System.Web.UI.WebControls.ListItem radiobutton1 =new System.Web.UI.WebControls.ListItem();
							radiobutton1.Text=currentRow["wave"].ToString();
							radiobutton1.Value=currentRow["id_wave"].ToString();							
							this.Items.Add(radiobutton1);							
					}
					//Coche la vague la plus r�cente
					if(checkMostRecentWave && !Page.IsPostBack){
						try{
							this.Items.FindByValue(ds.Tables[0].Rows[ds.Tables[0].Rows.Count-1]["id_wave"].ToString()).Selected=true;
						}catch(Exception){}
					}
				}
			}
			else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas d�finie."));
			#endregion	
		}
		#endregion

		#region Render
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output) {				
			int i = 0;
			if(noneWave){
				output.Write("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1653,webSession.SiteLanguage)
					+"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.DataLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.DataLanguage+"/button/back_up.gif';\">"
					+"<img src=\"/Images/"+webSession.DataLanguage+"/button/back_up.gif\" border=0 name=bouton></a>"
					+"</div><br>");
			}else{
					output.Write("<table cellpadding=\"0\" cellspacing=\"1\" bgColor=\"#644883\" width=\"90%\">");
					output.Write("<tr><td bgcolor=\"#ffffff\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1650,webSession.SiteLanguage)+"</td></tr><tr><td>");
					output.Write("<TABLE id=\"centerTable\" height=\"100%\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"txtViolet11Bold\" bgcolor=\"#ded8e5\">");
					output.Write("<TR valign=\"top\"><TD>");
					#region Ajout des vagues
					foreach(ListItem item in this.Items){
						output.Write("<tr>");
						output.Write("<td width=\"100%\"><input type=\"radio\"");
						if ( this.Items[i].Selected) output.Write(" Checked"); 
						output.Write(" id=\"WaveSelectionWebControl_"+i+"\" name=\"WaveSelectionWebControl\"/ value=\""+item.Value+"\"><label for=\"WaveSelectionWebControl_"+i+"\">"+ item.Text.ToString()+"</label></td>");
						output.Write("</tr>");
						i++;
					}				
					#endregion
					output.Write("</TD></TR></TABLE>");
					output.Write("</td></tr></table>");
			}						
		}
		#endregion

		#endregion
	}
}
