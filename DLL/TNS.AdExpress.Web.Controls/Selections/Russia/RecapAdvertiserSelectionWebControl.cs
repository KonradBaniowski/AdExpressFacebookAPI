#region Informations
// Auteurs: G. Ragneau
// Date de création: 15/09/2004
// Date de modification: 15/09/2004
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Collections;

using TNS.AdExpress.Web.DataAccess.Selections.Products;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.ProductClassReports.DAL;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;
using CstModule = TNS.AdExpress.Constantes.Web.Module;
using System.Reflection;
using System.Collections.Generic;

namespace TNS.AdExpress.Web.Controls.Selections.Russia
{
	/// <summary>
	/// Composant affichant la liste des annonceurs disponibles suivant la sélection de produits en session
	/// Il est utilisé dans la sélection des annonceurs références et concurrents
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SegmentSelectionWebControl runat=server></{0}:SegmentSelectionWebControl>")]
	public class RecapAdvertiserSelectionWebControl : System.Web.UI.WebControls.CheckBoxList
	{
		#region Variables		     
		/// <summary>
		/// Dataset contenant la liste des variétés
		/// </summary>
		protected DataSet dsListAdvertiser;
		/// <summary>
		/// Session
		/// </summary>
		protected WebSession webSession;										
		/// <summary>
		/// Indique si on recoche les checkbox
		/// </summary>
		protected bool reCheck = false;
		/// <summary>
		/// Liste d'annonceurs à na pas afficher (utiliule pour éviter la sélection d'annonceur
		/// dans les références et dans les concurrents)
		/// </summary>
		public string ExceptionsList = "";
		/// <summary>
		/// Indique si on reconstruit la liste d'items dans checkboxlist
		/// </summary>
		protected bool reBuildCheckboxlist = false;
		/// <summary>
		/// Titre du webControl
		/// </summary>
		protected string title;
        /// <summary>
        /// Key word
        /// </summary>
        protected string _keyWord = "";
        /// <summary>
        /// Event 
        /// </summary>
        protected int _eventButton = -1;

        /// <summary>
        /// 
        /// </summary>
        protected bool _canShowData = true;

        protected List<long> _checkedAdvertisers = new List<long>();
		#endregion
	
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public RecapAdvertiserSelectionWebControl():base()
		{
			this.EnableViewState=true;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit le titre du tableau d'annonceur
		/// </summary>
		public string Title{
			get{return title;}
			set{title=value;}
		}
		/// <summary>
		/// Obtient le dataset avec la liste des variétés 
		/// </summary>
		public DataSet DsListAdvertiser
		{
			get{return dsListAdvertiser;}
		}

		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public virtual WebSession WebSession
		{
			get{return webSession;}
			set{webSession=value;}
		}			
		/// <summary>
		/// Obtient ou définit reCheck
		/// </summary>
		public bool ReCheck{
			get{return reCheck;}
			set{reCheck=value;}
		}
		/// <summary>
		/// Obtient ou définit reBuildCheckboxlist
		/// </summary>
		public bool ReBuildCheckboxlist{
			get{return reBuildCheckboxlist;}
			set{reBuildCheckboxlist=value;}
		}
        /// <summary>
        /// Get / Set Key word
        /// </summary>
        public string KeyWord
        {
            get { return _keyWord; }
            set { _keyWord = value; }
        }
        /// <summary>
        /// Set current event
        /// </summary>
        public virtual int EventButton
        {
            set { _eventButton = value; }
        }
        /// <summary>
        /// Set current event
        /// </summary>
        public virtual List<long> CheckedAdvertisers
        {
            set { _checkedAdvertisers = value; }
        }	
		#endregion

		#region Evènements
	
		#region PreRender
		/// <summary>
		/// Construction de la liste de checkbox
		/// </summary>
		/// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            // Trim keyWord
            _keyWord = _keyWord.Trim();
            dsListAdvertiser = GetAdvertisers();

            // Sélection de tous les fils
            if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SelectAllChilds", TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
            }

            this.Items.Clear();

            //Construction de la liste de checkbox
            foreach (DataRow currentRow in dsListAdvertiser.Tables[0].Rows)
            {
                this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["advertiser"].ToString(), currentRow["id_advertiser"].ToString()));
            }
        }

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) 
		{

			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);

            #region Show Messages
            t.Append(GetMessage());
            #endregion

			#region Construction du code HTML
			
			#region annonceurs presents
			if(dsListAdvertiser != null && dsListAdvertiser.Tables[0].Rows.Count > 0 && _canShowData)
			{
				#region Variables locales
				int i = 0;
				#endregion

				#region Début du tableau global
                t.Append("<tr vAlign=\"top\" height=\"99%\" align=\"left\" class=\"backGroundWhite\"><td><br><div width=\"80%\" vAlign=\"top\" id=\"advertisers\"><table class=\"divTable\" vAlign=\"top\" cellSpacing=\"0\">");
				t.Append("<caption class=\"divTable\">" + title + "</caption>");
				t.Append("<tr><td colspan=\"3\"><a href=\"javascript: SelectAllChilds('advertisers')\" title=\""+GestionWeb.GetWebWord(816,webSession.SiteLanguage)+"\" class=\"roll04\">"+GestionWeb.GetWebWord(816,webSession.SiteLanguage)+"</a></a></td></tr>");
				#endregion

				#region Ajout des annonceurs
				foreach(DataRow currentRow in dsListAdvertiser.Tables[0].Rows){
					if ((i%3)<1)t.Append("<tr>");
					t.Append("<td width=\"33%\"><input type=\"checkBox\"");
                    //if (this.Items[i].Selected) 
                    if(_checkedAdvertisers != null && _checkedAdvertisers.Contains(Convert.ToInt64(currentRow["id_advertiser"].ToString())))
                        t.Append(" Checked");
					t.Append(" id=\"recapAdvertiserSelectionWebControl_"+i+"\" name=\"recapAdvertiserSelectionWebControl$"+i+"\"/><label for=\"recapAdvertiserSelectionWebControl_"+i+"\">"+currentRow["advertiser"]+"</label></td>");
					if ((i%3)>1)t.Append("</tr>");
					i++;
				}
				if (((i-1)%3)<2)t.Append("</tr>");
				#endregion

				#region Fin du tableau
				t.Append("</table></div></td></tr>");
				#endregion
			}
			#endregion
			
		

			#endregion

			output.Write(t.ToString());

		}
		#endregion

		#endregion

        /// <summary>
        /// Get advertisers list
        /// </summary>      
        /// <returns>advertisers list</returns>
        private DataSet GetAdvertisers()
        {
            TNS.AdExpress.Domain.Web.Navigation.Module m = ModulesList.GetModule(CstModule.Name.TABLEAU_DYNAMIQUE);
            if (m.CountryDataAccessLayer == null) throw (new NullReferenceException("Data Access layer is null for the Product Class result"));
            object[] param = new object[1];
            param[0] = webSession;

            if (_eventButton == constEvent.eventSelection.OK_EVENT)
            {
                IProductClassReportsDAL productClassLayer = (IProductClassReportsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + m.CountryDataAccessLayer.AssemblyName, m.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
               return productClassLayer.GetUniversAdvertisers(ExceptionsList,_keyWord);
            }
            else
            {
                IProductClassReportsDAL productClassLayer = (IProductClassReportsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + m.CountryDataAccessLayer.AssemblyName, m.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                return productClassLayer.GetUniversAdvertisers(ExceptionsList);
            }
        }

        #region Message
        /// <summary>
        /// Get Message to show
        /// <example>Error message</example>
        /// </summary>
        /// <returns>Message</returns>
        protected string GetMessage()
        {
            System.Text.StringBuilder t = new System.Text.StringBuilder(3000);
            #region  Message d'erreurs
            // Message d'erreur : veuillez saisir 2 caractères minimums
            if (_keyWord.Length < 2 && _keyWord.Length > 0 && _eventButton == constEvent.eventSelection.OK_EVENT)
            {
                t.Append("<tr class=\"backGroundWhite\" ><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                t.Append(" " + GestionWeb.GetWebWord(1473, webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
                t.Append("</td>");
                t.Append("</tr>");
                _canShowData = false;
            }
            // Message d'erreur : mot incorrect
            else if ( _eventButton == constEvent.eventSelection.OK_EVENT && !CheckedProductText(_keyWord) && _keyWord.Length > 0 )
            {
                t.Append("<tr><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                t.Append(" " + GestionWeb.GetWebWord(1088, webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
                t.Append("</td>");
                t.Append("</tr>");
                _canShowData = false;
            }
            // Message d'erreur : aucun résultat avec le mot clé
            else if (dsListAdvertiser != null && _eventButton == constEvent.eventSelection.OK_EVENT)
            {
                if (dsListAdvertiser.Tables[0].Rows.Count == 0)
                {
                    t.Append("<tr><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                    t.Append(" " + GestionWeb.GetWebWord(819, webSession.SiteLanguage) + " " + _keyWord + ".</p> ");
                    t.Append("</td>");
                    t.Append("</tr>");
                    _canShowData = false;
                }
            }
            else if ((dsListAdvertiser == null || dsListAdvertiser.Tables[0].Rows.Count == 0) && _eventButton != constEvent.eventSelection.OK_EVENT)
            {
                {
                    t.Append("<tr vAlign=\"top\" ><td class=\"backGroundWhite txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                    t.Append(" " + GestionWeb.GetWebWord(1095, webSession.SiteLanguage) + "</p> ");
                    t.Append(" </td> ");
                    t.Append(" </tr> ");
                    _canShowData = false;
                }
            }
            #endregion        

            return t.ToString();
        }


        /// <summary>
        /// Vérifie la cohérence du texte :
        /// Texte accepté : texte contenant seulement des chiffres, les lettres de l'alphabet
        /// où @
        /// </summary>
        /// <param name="text">Texte Source</param>
        /// <returns>True si le texte suit la syntaxe, false sinon</returns>
        public virtual bool CheckedProductText(string text)
        {
            //Regex r = new Regex("^(?<list>([0-9]|[a-z]|[A-Z]|[ ]|[@]))");
            //if (r.Match(text).Success && text.Length >= 2)
            if (!string.IsNullOrEmpty(text) && text.Length >= 2)
                return true;
            else
            {
                return false;
            }
        }
        #endregion
		
	}
}
