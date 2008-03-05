#region Informations
// Auteur: D. Mussuma
// Date de création: 
// Date de modification: 15/03/2007
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;


namespace TNS.AdExpress.Web.Controls.Buttons
{
	/// <summary>
	///  Composant affichant une case à cohcer pour le site AdExpress
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AdExpressCheckBox Language='33' Code='0' runat=server></{0}:AdExpressCheckBox>")]
	public class AdExpressCheckBox : System.Web.UI.WebControls.CheckBox
	{
		

		#region Variables

		/// <summary>
		/// Code du texte à afficher
		/// </summary>
		private int _code = 0;

		/// <summary>
		/// Langue du texte à afficher
		/// </summary>
		private int _language=33;
			
		/// <summary>
		/// Attribut pour lancer côté cleint le clic de la case à cocher
		/// </summary>
		private string _onClick = null;
		#endregion

		#region Constructeur

		/// <summary>
		/// Constructeur
		/// </summary>
		public AdExpressCheckBox():base(){ 
		}

		#endregion
	
		#region Accesseurs

		/// <summary>
		/// Obtient et définit du Code du texte à extraire
		/// </summary>
		[Category("Comportement"),
		Description("Indique le code du texte à afficher."),
		DefaultValue(0)]
		public virtual int Code{
			get{return _code;}
			set{_code = value;}
		}

		/// <summary>
		/// Obtient et définit du Code du texte à extraire la Langue du texte à extraire
		/// </summary>
		[Category("Comportement"),
		Description("Langue du texte à afficher."),
		DefaultValue(33)]
		public virtual int Language{
			get{return _language;}
			set{_language = value;}
		}

		/// <summary>
		/// Obtient et définit  Attribut pour lancer côté cleint le clic de la case à cocher
		/// </summary>
		[Category("Comportement"),
		Description(" Attribut pour lancer côté client le clic de la case à cocher."),
		DefaultValue("")]
		public virtual string OnClick{
			get{return _onClick;}
			set{_onClick = value;}
		}
		#endregion

		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			if(_code>0)this.Text = GestionWeb.GetWebWord(_code,_language);
			if(_onClick!=null && _onClick.Length>0) this.Attributes.Add("onClick",_onClick);
			base.Render(output);
		}
	}
}
