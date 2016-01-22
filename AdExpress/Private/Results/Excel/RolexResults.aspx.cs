using System;
using System.Reflection;
using System.Web;
using TNS.AdExpressI.Rolex;

namespace Private.Results.Excel
{
    public partial class RolexResults : TNS.AdExpress.Web.UI.ExcelWebPage
    {
        #region Variables
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string result = "";
        /// <summary>
        /// Identifiant de session
        /// </summary>
        public string _idsession = "";
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public RolexResults()
            : base()
        {
			_idsession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ROLEX);
                object[] param = new object[1] { _webSession };
                var rolexResult = (IRolexResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                rolexResult.ResultControlId = this.ID;
                result =rolexResult.GetExcelHtml();

            }            
            catch (Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }

        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
           
        }
    }
}