#region Informations
// Auteur: G. Facon
// Date de cr�ation: 
// Date de modification: 06/07/2004
//	12/08/2005	G. Facon	Nom de variables
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Bastet.Translation;
using System.ComponentModel;
using System.Globalization;
using Obout = OboutInc.Calendar2;


namespace TNS.AdExpress.Bastet.WebControls{
	/// <summary>
	/// Composant affichant un texte extrait de la base de donn�es AdExpress pour le site AdExpress
	/// </summary>
    [ToolboxData("<{0}:BastetText Language='33' Code='0' runat='server'></{0}:BastetText>")]
    public class CalendarWebControl : System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// Code du texte � afficher
		/// </summary>
		private CultureInfo _cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public CalendarWebControl()
            : base() { 
		}

		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient et d�finit du Code du texte � extraire
		/// </summary>
		[Category("Comportement"),
        Description("CultureInfo")]
        public virtual CultureInfo CultureInfo {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
		}
		#endregion

		#region Ev�nements

		/// <summary>
		/// Rendu du texte � extraire
		/// </summary>
		/// <param name="writer">Flux HTML</param>
		protected override void Render( HtmlTextWriter writer) {
		}
		#endregion


    }
}
