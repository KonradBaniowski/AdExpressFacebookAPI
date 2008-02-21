#region Informations
// Auteur: Y. Rkaina 
// Date de création: 13/11/2006
// Date de modification: 
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using PeriodConstantes = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Controls.Buttons;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description résumée de DetailInsertionPeriodNavigationWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:DetailInsertionPeriodNavigationWebControl runat=server></{0}:DetailInsertionPeriodNavigationWebControl>")]
	public class DetailInsertionPeriodNavigationWebControl : System.Web.UI.WebControls.WebControl {
		
		#region Variables
		/// <summary>
		/// La période sélectionnée
		/// </summary>
		private string _zoomDate="";
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession = null;
		/// <summary>
		/// Identifiant du média courant
		/// </summary>
		private Int64 _idVehicle;
		/// <summary>
		/// Liste des paramètres postés par le navigateur
		/// </summary>				
		private string _ids="";
		/// <summary>
		/// Position du composant 
		/// </summary>
		private string _align=""; 
		/// <summary>
		/// L'URL de la page
		/// </summary>
		private string _url="";
        /// <summary>
        /// Contrôle de navigation entre péridoe
        /// </summary>
        SubPeriodSelectionWebControl _subPeriodSelectionWebControl = null;
        /// <summary>
        /// Id of the variable containing the current period
        /// </summary>
        protected string _periodContainerName = string.Empty;
        /// <summary>
        /// SPecify if the vehicle must be displayed
        /// </summary>
        protected bool _displayVehicle = true;
        #endregion

		#region Accesseurs
		/// <summary>
        /// Specify if the vehicle must be displayed
		/// </summary>
		[Bindable(false)]
		public bool DisplayVehicle {
			get{return _displayVehicle;}
			set{
                _displayVehicle = value;
            }
		}
		/// <summary>
		/// Définit la période sélectionnée
		/// </summary>
		[Bindable(false)]
		public string ZoomDate {
			get{return _zoomDate;}
			set{
                _zoomDate=value;
                _subPeriodSelectionWebControl.ZoomDate = _zoomDate;
            }
		}
		/// <summary>
		/// Définit la WebSession
		/// </summary>
		[Bindable(false)]
		public WebSession CustomerWebSession {
			set{
                _customerWebSession=value;
                _subPeriodSelectionWebControl.WebSession = _customerWebSession;
            }
		}
		/// <summary>
		/// Définit l'identifiant du média courant
		/// </summary>
		[Bindable(false)]
		public Int64 IdVehicle{
			set{_idVehicle=value;}
		}
		/// <summary>
		/// La liste des paramètres postès^par le navigateur
		/// </summary>
		[Bindable(false)]
		public string Ids {
			set{_ids=value;}
		}
		/// <summary>
		/// Définit la position du composant
		/// </summary>
		[Bindable(false)]
		public string Align {
			set{_align=value;}
		}
		/// <summary>
		/// URL de la page
		/// </summary>
		[Bindable(false)]
		public string Url {
			set{_url=value;}
		}
		[Bindable(true)]
        /// <summary>
        /// Get / set the ID of the web element containing the current period
        /// </summary>
        public string PeriodContainerName
        {
            get
            {
                return _periodContainerName;
            }
            set
            {
                _periodContainerName = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public DetailInsertionPeriodNavigationWebControl()
        {
            _subPeriodSelectionWebControl = new SubPeriodSelectionWebControl();
            this.EnableViewState = true;
        }
        #endregion

        #region Init
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad (e);
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {

            string vehicle = "";

            #region Mise en forme des dates et du media
            switch ((DBClassificationConstantes.Vehicles.names)_idVehicle)
            {
                case DBClassificationConstantes.Vehicles.names.press: vehicle = GestionWeb.GetWebWord(1298, _customerWebSession.SiteLanguage).ToUpper(); break;
                case DBClassificationConstantes.Vehicles.names.radio: vehicle = GestionWeb.GetWebWord(205, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.tv: vehicle = GestionWeb.GetWebWord(206, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.others: vehicle = GestionWeb.GetWebWord(647, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.outdoor: vehicle = GestionWeb.GetWebWord(1779, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.internet: vehicle = GestionWeb.GetWebWord(1301, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.adnettrack: vehicle = GestionWeb.GetWebWord(648, _customerWebSession.SiteLanguage); break;
                case DBClassificationConstantes.Vehicles.names.directMarketing: vehicle = GestionWeb.GetWebWord(2219, _customerWebSession.SiteLanguage); break;
            }
            #endregion

            #region Début du tableau (support et dates)
            output.Write("<TABLE width=\"100%\" bgColor=\"#ffffff\" style=\"MARGIN-LEFT: 0px; MARGIN-RIGHT: 0px;BORDER:SOLID 5px #ffffff;\"");
            output.Write("cellPadding=\"0\" cellSpacing=\"0\" align=\"" + _align + "\" border=\"0\">");
            #endregion

            #region Paramètres (vehicle, dates...)

            #region Vehicle
            if (_displayVehicle)
            {
                output.Write("<TR>");

                output.Write("<TD colSpan=\"11\" valign=\"center\" class=\"txtViolet14Bold\">");
                if (_zoomDate.Length > 0)
                {
                    output.Write(vehicle + "&nbsp;&nbsp;");
                }
                else
                {
                    output.Write(vehicle);
                }
                output.Write("</TD>");
                output.Write("</TR>");
            }
            #endregion

            output.Write("<TR>");
            output.Write("<TD>&nbsp;</TD>");
            output.Write("</TR>");

            #region Période

            _subPeriodSelectionWebControl.PeriodContainerName = string.Format("zoom_param_{0}", this.ID);
            _subPeriodSelectionWebControl.JavascriptRefresh = string.Format("RefreshData_{0}", this.ID);

            output.WriteLine("<script language=\"javascript\">");
            output.WriteLine(string.Format("\tvar zoom_param_{0} = '{1}';", this.ID, _zoomDate));
            output.WriteLine(string.Format("\tfunction RefreshData_{0}()", this.ID));
            output.WriteLine("\t{");
            output.WriteLine(string.Format("\t\tdocument.getElementById('{1}').value = zoom_param_{0};", this.ID, this.PeriodContainerName));
            output.WriteLine(string.Format("\t\t__doPostBack('', '');", this.ID));
            output.WriteLine("\t}");
            output.WriteLine("</script>");


            output.Write("<tr><td>");
            _subPeriodSelectionWebControl.RenderControl(output);
            output.Write("</td></tr>");
            #endregion

            output.Write("<TR>");
            output.Write("<TD>&nbsp;</TD>");
            output.Write("</TR>");
            output.Write("</TABLE>");
            #endregion

        }
		#endregion
        
		#region Méthodes interne
		/// <summary>
		/// Génère un nombre à partir de la date
		/// </summary>
		/// <returns>Nombre généré</returns>
		private static string GenerateNumber()
		{
			DateTime dt=DateTime.Now;
			return(dt.Year.ToString()+dt.Month.ToString()+dt.Day.ToString()+dt.Hour.ToString()+dt.Minute.ToString()+dt.Second.ToString()+dt.Millisecond.ToString()+new Random().Next(1000));
		}
		#endregion
	}
}
