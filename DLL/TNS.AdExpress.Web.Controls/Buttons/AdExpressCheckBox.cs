#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 
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
	///  Composant affichant une case � cohcer pour le site AdExpress
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AdExpressCheckBox Language='33' Code='0' runat=server></{0}:AdExpressCheckBox>")]
	public class AdExpressCheckBox : System.Web.UI.WebControls.CheckBox
	{
		

		#region Variables

		/// <summary>
		/// Code du texte � afficher
		/// </summary>
		private int _code = 0;

		/// <summary>
		/// Langue du texte � afficher
		/// </summary>
		private int _language=33;
			
		/// <summary>
		/// Attribut pour lancer c�t� cleint le clic de la case � cocher
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
		/// Obtient et d�finit du Code du texte � extraire
		/// </summary>
		[Category("Comportement"),
		Description("Indique le code du texte � afficher."),
		DefaultValue(0)]
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

		/// <summary>
		/// Obtient et d�finit  Attribut pour lancer c�t� cleint le clic de la case � cocher
		/// </summary>
		[Category("Comportement"),
		Description(" Attribut pour lancer c�t� client le clic de la case � cocher."),
		DefaultValue("")]
		public virtual string OnClick{
			get{return _onClick;}
			set{_onClick = value;}
		}
		#endregion

		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output)
		{
			if(_code>0)this.Text = GestionWeb.GetWebWord(_code,_language);
			if(_onClick!=null && _onClick.Length>0) this.Attributes.Add("onClick",_onClick);
			base.Render(output);
		}
	}
}
