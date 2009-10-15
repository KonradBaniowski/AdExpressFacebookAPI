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



namespace TNS.AdExpress.Bastet.WebControls{
	/// <summary>
	/// Composant affichant un texte extrait de la base de donn�es AdExpress pour le site AdExpress
	/// </summary>
    [ToolboxData("<{0}:BastetText Language='33' Code='0' runat='server'></{0}:BastetText>")]
	public class BastetText: WebControl,TNS.AdExpress.Domain.Translation.ITranslation{

		#region Variables

		/// <summary>
		/// Code du texte � afficher
		/// </summary>
		private int _code = 0;
		/// <summary>
		/// Langue du texte � afficher
		/// </summary>
		private int _language=33;

		#endregion

		#region Constructeur

		/// <summary>
		/// Constructeur
		/// </summary>
        public BastetText()
            : base() { 
		}

		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient et d�finit du Code du texte � extraire
		/// </summary>
		[Category("Comportement"),
		Description("Indique le code du texte � afficher."),
		DefaultValue(1)]
		public virtual int Code{
			get{return _code;}
			set{_code = value;}
		}

		/// <summary>
		/// Obtient et d�finit du Code du texte � extraire la Langue du texte � extraire
		/// </summary>
		[Category("Comportement"),
		Description("Langue du texte � afficher."),
		DefaultValue(33)]
		public virtual int Language{
			get{return _language;}
			set{_language = value;}
		}

		#endregion

		#region Ev�nements

		/// <summary>
		/// Rendu du texte � extraire
		/// </summary>
		/// <param name="writer">Flux HTML</param>
		protected override void Render( HtmlTextWriter writer) {
			writer.Write(GestionWeb.GetWebWord(_code,_language));    
		}

		#endregion


    }
}
