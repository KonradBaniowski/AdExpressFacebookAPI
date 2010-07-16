#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 30/06/2005
// Date de modification: 
#endregion
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using DBCst = TNS.AdExpress.Constantes.DB;
using FwkSelectionCst=TNS.AdExpress.Constantes.FrameWork.Selection;
using FwkResultsCst=TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Controls.Exceptions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;


namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Affiche la liste des cibles AEPM que peut s�lectionner un client .
	/// Le client ne peut s�lectionner qu'une cible de base et une cible suppl�mentaire dans la liste.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:TargetSelectionWebControl runat=server></{0}:TargetSelectionWebControl>")]
	public class TargetSelectionWebControl : System.Web.UI.WebControls.RadioButtonList
	{

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
		/// identifiant cible de base
		/// </summary>
		private string idBaseTarget="";
		/// <summary>
		/// Ev�nement qui a �t� lanc�
		/// </summary>
		protected int eventButton;
		/// <summary>
		/// V�rifie si le contr�le affiche la cible de base
		/// </summary>
		protected bool isThisTargetBase = false;
		/// <summary>
		/// Code de traduction
		/// </summary>
		protected int code =0;		
		#endregion		

		#region Accesseurs
		/// <summary>
		/// D�finit la session � utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}

		/// <summary>
		/// D�finit l'�v�nement qui a �t� lancer
		/// </summary>
		public virtual int EventButton {
			set{eventButton = value;}
		}
		
		/// <summary>
		///  V�rifie si le contr�le affiche la cible de base
		/// </summary>
		public virtual bool IsThisTargetBase{
			set{isThisTargetBase = value;}
			get{return isThisTargetBase;}
		}

		/// <summary>
		///  V�rifie si le contr�le affiche la cible de base
		/// </summary>
		public virtual int Code{
			set{code = value;}			
		}
		#endregion

		#region constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public TargetSelectionWebControl():base(){		
			this.EnableViewState=true;
		}
		#endregion
		
		#region Ev�nements

		#region OnLoad
		/// <summary>
		/// Chargement du contr�le
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e) {
			//Vague en acc�s
			string waveAccessList ="";
			if(!Page.IsPostBack){
				if(webSession!=null){
					waveAccessList=((LevelInformation)webSession.SelectionUniversAEPMWave.FirstNode.Tag).ID.ToString();
				}
				if(WebFunctions.CheckedText.IsStringEmpty(waveAccessList)){	
					if(IsThisTargetBase)
					ds=TargetListDataAccess.GetAEPMTargetListDataAccess(waveAccessList,FwkResultsCst.APPM.BaseTarget,webSession.Source);	
					else ds=TargetListDataAccess.GetAEPMTargetListDataAccess(waveAccessList,webSession.Source);	
				}
				if(ds==null || ds.Equals(System.DBNull.Value) || ds.Tables[0]==null || ds.Tables[0].Rows.Count==0)
					noneWave=true;
			}

			#region Bouton Valider ou Recall
			if(eventButton==1 ){
				webSession.SelectionUniversAEPMTarget=this.createTreeNode();
				webSession.Save();
			}


			#endregion
		}
		

		#endregion

		#region PreRender
		/// <summary>
		/// M�thode lanc�e avant le rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {

            if (ViewState["idBaseTarget"] != null)
                idBaseTarget = (string)ViewState["idBaseTarget"];

            #region Ajout des items � partir de du groupe donn�es ds
			if(webSession!=null){
				if(ds!=null && ds.Tables[0].Rows.Count>0){
					this.Items.Clear();						
                    //Construction de la liste de radibouton
					foreach(DataRow currentRow in ds.Tables[0].Rows) {						
						System.Web.UI.WebControls.ListItem radiobutton1 =new System.Web.UI.WebControls.ListItem();
						radiobutton1.Text=currentRow["target"].ToString();
						radiobutton1.Value=currentRow["id_target"].ToString();	
						if(FwkResultsCst.APPM.BaseTarget.ToUpper().Equals(currentRow["target"].ToString().Trim().ToUpper()))
							idBaseTarget=currentRow["id_target"].ToString();
						this.Items.Add(radiobutton1);							
					}
					if(idBaseTarget.Length > 0)
                        ViewState.Add("idBaseTarget", idBaseTarget);
				}
			}
			else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas d�finie."));
			#endregion	

		}
		#endregion

		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output)
		{
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			int i = 0;
			if(noneWave){
                output.Write("<div align=\"center\" class=\"txtBlanc11Bold\">" + GestionWeb.GetWebWord(1656, webSession.SiteLanguage)
                    + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/App_Themes/" + themeName + "/Images/Culture/button/back_down.gif';\" onmouseout=\"bouton.src = '/App_Themes/" + themeName + "/Images/Culture/button/back_up.gif';\">"
                    + "<img src=\"/App_Themes/" + themeName + "/Images/Culture/button/back_up.gif\" border=0 name=bouton></a>"
					+"</div><br>");
			}else{				
				
				//cibles suppl�mentaires
                output.Write("<tr><td colspan=\"3\" class=\"txtViolet11Bold backGroundWhite\">&nbsp;&nbsp;" + GestionWeb.GetWebWord(code, webSession.SiteLanguage) + "</td></tr><tr><td>");
                output.Write("<table id=\"centerTable\" height=\"100%\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"txtViolet11Bold lightPurple\">");
				output.Write("<tr valign=\"top\"><td >");

				#region Ajout des cibles
				foreach(ListItem item in this.Items){
					if((!item.Value.Equals(idBaseTarget) && !IsThisTargetBase) || IsThisTargetBase ) {
						if((i%2)<1)output.Write("<tr>");
						output.Write("<td width=\"50%\"><input type=\"radio\"");
						if (item.Selected || item.Value.Equals(idBaseTarget)) output.Write("Checked");
						output.Write(" id=\"TargetSelectionWebControl_"+i+"\" name=\""+this.ID+"\" value=\""+item.Value+"\"><label for=\"TargetSelectionWebControl_"+i+"\">"+ item.Text.ToString()+"</label></td>");
						if ((i%2)>1)output.Write("</tr>");					
					i++;
					}					
				}	
				if (((i-1)%2)<2)output.Write("</tr>");
				#endregion
                output.Write("</td></tr></table>");
				output.Write("</td></tr>");			
			}				
		}
		#endregion

		#region M�thodes internes

		#region cr�ation de l'arbre
		/// <summary>
		/// Cr�ation de l'arbre � partir de la liste des Items
        /// </summary>
        /// <returns>cibles s�lectionn�es</returns>
        protected System.Windows.Forms.TreeNode createTreeNode()
        {
			System.Windows.Forms.TreeNode targetTree=new System.Windows.Forms.TreeNode("target");					
		
			// product s�lectionn�s		
			targetTree = ((System.Windows.Forms.TreeNode)webSession.SelectionUniversAEPMTarget);					
			int compteurChild=0;
			
			if(webSession.SelectionUniversAEPMTarget.FirstNode!=null){
				webSession.SelectionUniversAEPMTarget.Nodes.Clear();
			}																	
				
			#region foreach				
			System.Windows.Forms.TreeNode tmp;
			foreach(ListItem item in this.Items){
				if (item.Selected){
					tmp = new System.Windows.Forms.TreeNode(item.Text);
					tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.aepmTargetAccess,Int64.Parse(item.Value),item.Text);
					tmp.Checked = true;
					targetTree .Nodes.Add(tmp);
					compteurChild++;				
				}
			}													
			#endregion												
							

			return targetTree ;

		}
		#endregion
		
		#endregion
	}
}
