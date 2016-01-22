#region Informations
// Auteur: G. Facon
// Date de création: 17/11/2006
// Date de modification:
#endregion

using System;
using System.Text;
using CstDB = TNS.AdExpress.Constantes.Customer.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.WebResultUI.Functions;

namespace TNS.AdExpress.Web.Core.Result{
	/// <summary>
	/// Cellule contenant un niveau de détail pour AdExpress
	/// </summary> 
	/// <remarks>
	/// La Cellule est capable de gérer l'affichage du Gad spécifiquement à AdExpress
	/// </remarks>
	[System.Serializable]
	public class AdExpressCellLevel:CellLevel{

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Adresse d'appelle du plan media
		/// </summary>
		protected string _link="javascript:OpenGad('{0}','{1}','{2}');";
		/// <summary>
		/// Niveau de détail générique
		/// </summary>
		protected GenericDetailLevel _genericDetailLevel = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'élément du niveau</param>
		/// <param name="label">Libellé du niveau</param>
		/// <param name="level">Niveau de profondeur du niveau</param>
		/// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
		public AdExpressCellLevel(long id, string label, int level,int lineIndexInResultTable) : base(id,label,level,lineIndexInResultTable){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'élément du niveau</param>
		/// <param name="label">Libellé du niveau</param>
		/// <param name="parentLevel">Niveau supérieur</param>
		/// <param name="level">Niveau de profondeur du niveau</param>
		/// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable)
            : base(id, label, parentLevel, level, lineIndexInResultTable)
        {
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'élément du niveau</param>
		/// <param name="label">Libellé du niveau</param>
		/// <param name="level">Niveau de profondeur du niveau</param>
		/// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
		/// <param name="webSession">Session du client</param>
        public AdExpressCellLevel(long id, string label, int level, int lineIndexInResultTable, WebSession webSession)
            : base(id, label, level, lineIndexInResultTable)
        {
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			_webSession=webSession;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'élément du niveau</param>
		/// <param name="label">Libellé du niveau</param>
		/// <param name="parentLevel">Niveau supérieur</param>
		/// <param name="level">Niveau de profondeur du niveau</param>
		/// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
		/// <param name="webSession">Session du client</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable, WebSession webSession)
            : base(id, label, parentLevel, level, lineIndexInResultTable)
        {
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			_webSession=webSession;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'élément du niveau</param>
		/// <param name="label">Libellé du niveau</param>
		/// <param name="parentLevel">Niveau supérieur</param>
		/// <param name="level">Niveau de profondeur du niveau</param>
		/// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="genericDetailLevel">Niveau de détail générique</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable, WebSession webSession, GenericDetailLevel genericDetailLevel)
            : base(id, label, parentLevel, level, lineIndexInResultTable)
        {
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			if(genericDetailLevel==null)throw(new ArgumentNullException("L'objet GenericDetailLevel est null"));
			_webSession=webSession;
			_genericDetailLevel =genericDetailLevel;
		}

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">identifiant de l'élément du niveau</param>
        /// <param name="label">Libellé du niveau</param>
        /// <param name="level">Niveau de profondeur du niveau</param>
        /// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        /// <param name="nbChar">Nombre de caractère sur une ligne</param>
        /// <param name="tolerance">Tolérence du nombre de caratère sur une ligne</param>
        public AdExpressCellLevel(long id, string label, int level, int lineIndexInResultTable, int nbChar, int tolerance)
            : base(id, label, level, lineIndexInResultTable, nbChar,tolerance)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">identifiant de l'élément du niveau</param>
        /// <param name="label">Libellé du niveau</param>
        /// <param name="parentLevel">Niveau supérieur</param>
        /// <param name="level">Niveau de profondeur du niveau</param>
        /// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        /// <param name="nbChar">Nombre de caractère sur une ligne</param>
        /// <param name="tolerance">Tolérence du nombre de caratère sur une ligne</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable, int nbChar, int tolerance)
            : base(id, label, parentLevel, level, lineIndexInResultTable, nbChar, tolerance)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">identifiant de l'élément du niveau</param>
        /// <param name="label">Libellé du niveau</param>
        /// <param name="level">Niveau de profondeur du niveau</param>
        /// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="nbChar">Nombre de caractère sur une ligne</param>
        /// <param name="tolerance">Tolérence du nombre de caratère sur une ligne</param>
        public AdExpressCellLevel(long id, string label, int level, int lineIndexInResultTable, WebSession webSession, int nbChar, int tolerance)
            : base(id, label, level, lineIndexInResultTable, nbChar, tolerance)
        {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _webSession = webSession;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">identifiant de l'élément du niveau</param>
        /// <param name="label">Libellé du niveau</param>
        /// <param name="parentLevel">Niveau supérieur</param>
        /// <param name="level">Niveau de profondeur du niveau</param>
        /// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="nbChar">Nombre de caractère sur une ligne</param>
        /// <param name="tolerance">Tolérence du nombre de caratère sur une ligne</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable, WebSession webSession, int nbChar, int tolerance)
            : base(id, label, parentLevel, level, lineIndexInResultTable, nbChar, tolerance)
        {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            _webSession = webSession;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">identifiant de l'élément du niveau</param>
        /// <param name="label">Libellé du niveau</param>
        /// <param name="parentLevel">Niveau supérieur</param>
        /// <param name="level">Niveau de profondeur du niveau</param>
        /// <param name="lineIndexInResultTable">Index de la ligne du niveau dans le tableau de resultat</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="nbChar">Nombre de caractère sur une ligne</param>
        /// <param name="tolerance">Tolérence du nombre de caratère sur une ligne</param>
        public AdExpressCellLevel(long id, string label, CellLevel parentLevel, int level, int lineIndexInResultTable, WebSession webSession, GenericDetailLevel genericDetailLevel, int nbChar, int tolerance)
            : base(id, label, parentLevel, level, lineIndexInResultTable, nbChar, tolerance)
        {
            if (webSession == null) throw (new ArgumentNullException("L'objet WebSession est null"));
            if (genericDetailLevel == null) throw (new ArgumentNullException("L'objet GenericDetailLevel est null"));
            _webSession = webSession;
            _genericDetailLevel = genericDetailLevel;
        }
		#endregion

		#region Implémentation de ICell (par héritage de Cell)

		/// <summary>
		/// Rendu de code HTML
		/// </summary>
		/// <returns>Code HTML</returns>
		public override string Render(){
			return Render(_cssClass);
		}

		/// <summary>
		/// Rendu de code HTML
		/// </summary>
		/// <returns>Code HTML</returns>
		public override string Render(string cssClass){
			StringBuilder html = new StringBuilder(100); 
			if(_genericDetailLevel == null)  _genericDetailLevel = _webSession.GenericProductDetailLevel;
			if( (_webSession != null && _addressId>0 
				&& _genericDetailLevel.GetDetailLevelItemInformation(this.Level).Equals(DetailLevelItemInformation.Levels.advertiser)
				)
				|| (_webSession == null && _addressId>0 )){

				html.AppendFormat("<td "+ ((cssClass.Length>0)?" class=\"" + cssClass + "\"":""));
				html.Append("align=\"left\">");
				html.AppendFormat("<a class=\"gad\" href=\""+_link+"\">", _webSession.IdSession, _label, _addressId);
                StringBuilder separator = new StringBuilder(100);
                for (int i = 0; i < _level; i++)
                        separator.Append("&nbsp;");
                if (_isWrapped)
                {
                    html.AppendFormat("> {0}</a></td>", TextWrap.WrapHtml(_label, _nbChar, _tolerance, separator.ToString()));
                }
                else
                {
                    html.Append(separator.ToString());
                    html.AppendFormat("> {0}</a></td>", this._label);
                }
				if (_newGroup)
					html.Insert(0,this.RenderSeparator());
			}
			else{
				html.AppendFormat("<td "+ ((cssClass.Length>0)?" class=\"" + cssClass + "\"":""));
				html.Append("align=\"left\">");
                StringBuilder separator = new StringBuilder(100);
				for(int i = 0; i < _level; i++)
                    separator.Append("&nbsp;");
                if (_isWrapped)
                    html.AppendFormat("{0}</td>", TextWrap.WrapHtml(this._label, _nbChar, _tolerance, separator.ToString()));
                else
                {
                    html.Append(separator.ToString());
                    html.AppendFormat("{0}</td>", this._label);
                }
				if (_newGroup)
					html.Insert(0,this.RenderSeparator());
			}
			return html.ToString();
		}
		#endregion
	}
}
