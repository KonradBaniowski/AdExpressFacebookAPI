using System;
using System.Data;

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;

using System.Data.SqlClient;
using TNS.AdExpressI.Classification.DAL;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;

namespace TNS.AdExpress.Web.Controls.Selections.Russia
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SubMediaSelectionWebControl runat=server></{0}:SubMediaSelectionWebControl>")]
    public class SubMediaSelectionWebControl : System.Web.UI.WebControls.RadioButtonList
    {
        /// <summary>
        /// Load list of sub media
        /// </summary>
        protected DataSet _dsSubMediaList = null;
        /// <summary>
        /// Session
        /// </summary>
        protected WebSession _webSession;

        protected bool _hasRecords = false;
        /// <summary>
        /// Evènement qui a été lancé
        /// </summary>
        protected int _eventButton;
        /// <summary>

        /// <summary>
        /// Get user session
        /// </summary>
        public virtual WebSession WebSession
        {
            get { return _webSession; }
            set { _webSession = value; }
        }

        /// <summary>
        /// Définit l'évènement qui a été lancer
        /// </summary>
        public virtual int EventButton
        {
            set { _eventButton = value; }
        }
        #region Constructor
		/// <summary>
        /// Constructor
		/// </summary>
        public SubMediaSelectionWebControl()
            : base()
		{
			this.EnableViewState=true;
		}
		#endregion

        #region  OnLoad
        /// <summary>
        /// On Load
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e)
        {


            if (!Page.IsPostBack)
            {
                //loading data			
                _dsSubMediaList = GetData();// ProductClassificationListDataAccess.SectorList(webSession);

                if (_dsSubMediaList.Equals(System.DBNull.Value) || _dsSubMediaList.Tables[0] == null || _dsSubMediaList.Tables[0].Rows.Count == 0)
                    _hasRecords = false;
                else _hasRecords = true;
            }
            if (_eventButton == 1)
            {
                CreateTreeNode();
                _webSession.Save();
            }

        }
        #endregion 

        #region PreRender
        /// <summary>
        /// Construction de la liste des  éléméents catégories
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (_dsSubMediaList != null && _dsSubMediaList.Tables[0].Rows.Count > 0)
            {
                //Initialization of items list
                this.Items.Clear();

                //Building radio button data list 
                foreach (DataRow currentRow in _dsSubMediaList.Tables[0].Rows)
                {
                    //Add sub media
                    this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["SubMedia"].ToString(), currentRow["id_SubMedia"].ToString()));
                }
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

            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            string title = GestionWeb.GetWebWord(2617, _webSession.SiteLanguage);
         

            #region Construction du code HTML


            if (_dsSubMediaList != null && _dsSubMediaList.Tables[0].Rows.Count > 0)
            {                

                int i = 0;
                Int64 idSubMedia = long.MinValue;

                #region Print sub medias

                #region Début du tableau global
                t.Append("<tr vAlign=\"top\"   align=\"center\" class=\"whiteBackGround\"><td><br><div width=\"100%\" vAlign=\"top\" id=\"submedias\">");
                t.Append("<table width=\"90%\" class=\"violetBorder txtViolet11Bold lightPurple\" vAlign=\"top\" cellSpacing=\"0\">");
                t.Append("<tr ><td colspan=\"3\" class=\"txtViolet11Bold whiteBackGround violetBorderBottom\">" + title + "</td></tr>");
                
                #endregion

                #region Adding  sub medias

                //foreach (DataRow currentRow in _dsSubMediaList.Tables[0].Rows)
                foreach(ListItem item in this.Items){
                
                    //idSubMedia = Int64.Parse(currentRow["Id_SubMedia"].ToString());

                   
                        if ((i % 1) < 1) t.Append("<tr>");
                        t.Append("<td width=\"33%\" ><input type=\"radio\" ");
                        if (item.Selected) output.Write("Checked");
                        t.Append(" id=\"SubMediaSelectionWebControl_" + i + "\" name=\"" + this.ID + "\"  value=\"" + item.Value + "\" >" + item.Text + "</td>");
                        if ((i % 1) > 1) t.Append("</tr>");
                        i++;
                   
                }

                if (i > 0 && ((i - 1) % 1) < 2) t.Append("</tr>");

                #endregion

                #region End of the table
                t.Append("</table></div></td></tr>");
                #endregion

                #endregion
            }
            else
            {
                #region No sub media
                t.Append("<tr vAlign=\"top\" ><td class=\"txtGris11Bold whiteBackGround\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                t.Append(" " + GestionWeb.GetWebWord(2618, _webSession.SiteLanguage) + "</p> ");
                t.Append(" </td> ");
                t.Append(" </tr> ");
                #endregion
            }


            #endregion

            output.Write(t.ToString());

        }
        #endregion


        #region Tests
        #region GetData()
        /// <summary>
        /// This method provides SQL queries to get the media typeclassification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data set 
        /// with media type's identifiers ("idMediaType" column) and media type's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        public DataSet GetData()
        {

            CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = _webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            return classficationDAL.GetSubMediaData();

           


           
        }
        #endregion

      

        #endregion

        #region création de l'arbre
        /// <summary>
        /// Création de l'arbre à partir de la liste des Items
        /// </summary>
        /// <returns>cibles sélectionnées</returns>
        protected void CreateTreeNode()
        {
            System.Windows.Forms.TreeNode submediaTree = new System.Windows.Forms.TreeNode("submedias");

            
                _webSession.CurrentUniversMedia.Nodes.Clear();
            
            #region foreach
            System.Windows.Forms.TreeNode tmp;
            foreach (ListItem item in this.Items)
            {
                if (item.Selected)
                {
                    tmp = new System.Windows.Forms.TreeNode(item.Text);
                    tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.categoryAccess, Int64.Parse(item.Value), item.Text);
                    tmp.Checked = true;
                    _webSession.CurrentUniversMedia.Nodes.Add(tmp);
                   
                }
            }
            #endregion


           

        }
        #endregion
    }
}
