#region Informations
// Auteur: G. Facon 
// Date de création: 10/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using System.Text;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Controls.Selections
{
    /// <summary>
    /// Affiche la liste des vehicles que peut sélectionner un client en fonction de ses droits.
    /// </summary>
    [DefaultProperty("Text"),
        ToolboxData("<{0}:VehicleSelectionWebControl runat=server></{0}:VehicleSelectionWebControl>")]
    public class VehicleSelectionWebControl : System.Web.UI.WebControls.CheckBoxList
    {

        #region Variables
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _webSession = null;

        protected List<long> _activeMediaTypes = null;

        private bool _checkActiveVehicle = false;
        #endregion

        #region Accesseurs

        /// <summary>
        /// Définit la session à utiliser
        /// </summary>
        public virtual WebSession CustomerWebSession
        {
            set { _webSession = value; }
        }

        /// <summary>
        /// Check Active Media
        /// </summary>
        public bool CheckActiveVehicle
        {
            get { return _checkActiveVehicle; }
            set { _checkActiveVehicle = value; }
        }

        #endregion

        #region Evènements
        /// <summary>
        /// Méthode lancée avant le rendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {

            bool containsSearch = false;
            int searchIndex = -1;
            bool containsSocial = false;
            int socialIndex = -1;
            int vehicleIndex = 0;
            string tmp2 = "";
            Object tmp = this.Parent;

            if (_webSession != null)
            {
                CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[Layers.Id.classification];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                object[] param = new object[1];
                param[0] = _webSession;
                IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.
                    CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance
                    | BindingFlags.Public, null, param, null, null);

                DataTable dt = FilteringWithMediaAgencyFlag(classficationDAL.GetMediaType().Tables[0]);

                this.DataSource = dt;
                this.DataTextField = "mediaType";
                this.DataValueField = "idMediaType";
                this.DataBind();

                if (_checkActiveVehicle
                    && _webSession.CurrentModule == Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    CoreLayer clMediaU = Domain.Web.WebApplicationParameters.CoreLayers[Layers.Id.mediaDetailLevelUtilities];
                    if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
                    Core.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (TNS.AdExpress.Web.Core.Utilities.
                        MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                        , AppDomain.CurrentDomain.BaseDirectory, clMediaU.AssemblyName), clMediaU.Class, false, BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                    _activeMediaTypes = (dt.Rows.Cast<DataRow>()
                                           .Select(dr => Convert.ToInt64(dr["idMediaType"].ToString()))).ToList();

                    _activeMediaTypes = mediaDetailLevelUtilities.GetAllowedVehicles(_activeMediaTypes, _webSession.PrincipalProductUniverses[0]);

                    foreach (ListItem item in this.Items)
                    {
                        if (!_activeMediaTypes.Contains(Convert.ToInt64(item.Value)))
                            item.Enabled = false;
                        if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.search)
                            && item.Value == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.search).DatabaseId.ToString())
                        {
                            item.Attributes.Add("onclick", "selectSearch(this);");
                            containsSearch = true;
                            searchIndex = vehicleIndex;
                        }
                        if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.social)
                          && item.Value == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.social).DatabaseId.ToString())
                        {
                            item.Attributes.Add("onclick", "selectSocial(this);");
                            containsSocial = true;
                            socialIndex = vehicleIndex;
                        }
                        vehicleIndex++;
                    }
                }



            }
            else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas définie"));
            //Edition du javascript permettant la selection de tous les vehicules
            if (!Page.ClientScript.IsClientScriptBlockRegistered("selectAllVehicles"))
            {
                string script = "\n<script language=\"JavaScript\">";

                while (tmp != null && tmp.GetType() != typeof(System.Web.UI.HtmlControls.HtmlForm))
                    tmp = ((Control)tmp).Parent;
                if (tmp != null)
                    tmp2 = ((System.Web.UI.HtmlControls.HtmlForm)tmp).ID;
                else tmp2 = "Form2";

                if (containsSearch || containsSocial)
                {

                    script += "\n\t var searchSelected = false;";
                    script += "\n\t var socialSelected = false;";
                    script += "\n\t function initAllVehicles() {";
                   script += "\n\t\t searchSelected = false; ";
                    script += "\n\t\t socialSelected = false; ";
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        script += "\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".checked = false;";
                        script += "\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".disabled = false;";
                    }
                    script += "\n\t } \n ";
                }

                script += "\n\tfunction selectAllVehicles(){";

                if (containsSearch || containsSocial)
                {
                    script += "\n\t\t if(searchSelected || socialSelected) \n";
                    script += "\n\t\t\t initAllVehicles(); \n";
                }

                script += "\n\tm=";

                string[] idMedias = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaInSelectAll)
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (_checkActiveVehicle && _webSession.CurrentModule == Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    idMedias = idMedias.Where(_activeMediaTypes.ConvertAll(Convert.ToString).Contains).ToArray();
                }
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (idMedias == null || idMedias.Length == 0
                        || Array.Exists(idMedias, s => s == this.Items[i].Value))
                    {
                        script += " document." + tmp2 + "." + this.ID + "_" + i + ".checked &&";
                    }
                }
                script = script.Substring(0, script.Length - 3) + ";";
                for (int i = 0; i < this.Items.Count; i++)
                {
                    script += string.Format("\n\t document.{0}.{1}_{2}.checked = !m && {3};"
                        , tmp2, this.ID, i
                        , (idMedias == null || idMedias.Length == 0 || Array.Exists(idMedias,
                                                                                            s =>
                                                                                            s == this.Items[i].Value)).GetHashCode());
                }
                script += "\n\t}\n</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "selectAllVehicles", script);
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered("selectSearch"))
            {
                if (containsSearch)
                {

                    StringBuilder searchScript = new StringBuilder();

                    searchScript.Append("\n <script language=\"JavaScript\"> ");
                    searchScript.Append("\n\t function selectSearch(obj){ ");
                    searchScript.Append("\n\t\t if(obj.checked == true) { ");
                    searchScript.Append("\n\t\t\t searchSelected = true; ");
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        if (i != searchIndex)
                        {
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".checked = false;");
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".disabled = true;");
                        }
                    }
                    searchScript.Append("\n\t\t } ");
                    searchScript.Append("\n\t\t else { ");
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        searchScript.Append("\n\t\t\t searchSelected = false; ");
                        if (i != searchIndex)
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".disabled = false;");
                    }
                    searchScript.Append("\n\t\t } ");
                    searchScript.Append("\n\t } \n</script> ");
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "selectSearch", searchScript.ToString());
                }
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered("selectSocial"))
            {
                if (containsSocial)
                {

                    StringBuilder searchScript = new StringBuilder();

                    searchScript.Append("\n <script language=\"JavaScript\"> ");
                    searchScript.Append("\n\t function selectSocial(obj){ ");
                    searchScript.Append("\n\t\t if(obj.checked == true) { ");
                    searchScript.Append("\n\t\t\t socialSelected = true; ");
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        if (i != socialIndex)
                        {
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".checked = false;");
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".disabled = true;");
                        }
                    }
                    searchScript.Append("\n\t\t } ");
                    searchScript.Append("\n\t\t else { ");
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        searchScript.Append("\n\t\t\t socialSelected = false; ");
                        if (i != socialIndex)
                            searchScript.Append("\n\t\t\t " + " document." + tmp2 + "." + this.ID + "_" + i + ".disabled = false;");
                    }
                    searchScript.Append("\n\t\t } ");
                    searchScript.Append("\n\t } \n</script> ");
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "selectSocial", searchScript.ToString());
                }
            }
            base.OnLoad(e);
        }
        #endregion


        #region  FilteringWithMediaAgencyFlag
        /// <summary>
        /// Filtering with media agency flag by media type
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <returns>Data Table </returns>
        protected DataTable FilteringWithMediaAgencyFlag(DataTable dt)
        {
            switch (_webSession.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                    DataTable dataTable = new DataTable();
                    DataColumn dataColumn;

                    //ID Vehicle
                    dataColumn = new DataColumn();
                    dataColumn.DataType = Type.GetType("System.Int64");
                    dataColumn.ColumnName = "idMediaType";
                    dataTable.Columns.Add(dataColumn);

                    //Vehicle label
                    dataColumn = new DataColumn();
                    dataColumn.DataType = Type.GetType("System.String");
                    dataColumn.ColumnName = "mediaType";
                    dataTable.Columns.Add(dataColumn);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr;
                        foreach (DataRow row in dt.Rows)
                        {
                            Int64 idV = Convert.ToInt64(row["idMediaType"].ToString());
                            if (_webSession.CustomerLogin.CustomerMediaAgencyFlagAccess(idV))
                            {
                                dr = dataTable.NewRow();
                                dr["idMediaType"] = idV;
                                dr["mediaType"] = row["mediaType"].ToString();
                                dataTable.Rows.Add(dr);
                            }
                        }
                    }
                    return dataTable;
                default: return dt;
            }
        }
        #endregion
    }
}
