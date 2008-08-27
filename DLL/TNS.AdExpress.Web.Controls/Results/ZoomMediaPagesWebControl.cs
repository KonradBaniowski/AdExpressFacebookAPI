#region Informations
// Auteur: D. Mussuma
// Date de création: 26/08/2008
// Date de modification: 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using WebCst = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Results {
	/// <summary>
	/// display zoom of pages of a media
	/// </summary>
	[ToolboxData("<{0}:ZoomMediaPagesWebControl runat=server></{0}:ZoomMediaPagesWebControl>")]
	public class ZoomMediaPagesWebControl : WebControl {
		#region Variables
		/// <summary>
		/// File name 
		/// </summary>
		protected string _fileName1;
		/// <summary>
		///  File name 
		/// </summary>
		protected string _fileName2;
		/// <summary>
		/// Date cover
		/// </summary>
		protected string _dateCover;				
		/// <summary>
		/// Media Id
		/// </summary>
		protected Int64 _idMedia;
		#endregion

		#region Acccessors
		/// <summary>
		/// file name
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string FileName1 {
			get {
				return _fileName1;
			}

			set {
				_fileName1 = value;
			}
		}
		/// <summary>
		/// file name
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string FileName2 {
			get {
				return _fileName2;
			}

			set {
				_fileName2 = value;
			}
		}
		/// <summary>
		/// Date cover
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string DateCover {
			get {
				return _dateCover;
			}

			set {
				_dateCover = value;
			}
		}
				
		/// <summary>
		/// Page anchor
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Localizable(true)]
		public long IdMedia {
			get {
				return _idMedia;
			}

			set {
				_idMedia = value;
			}
		}
		#endregion

		protected override void Render(HtmlTextWriter output) {
			string pathWeb1 = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/" + _fileName1 + "";
			string pathWeb2 = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/" + _fileName2 + "";
			StringBuilder t = new StringBuilder(3000);

			t.Append("<table><tr><td>");
			t.Append("<img src='" + pathWeb1 + "' border=\"0\" width=470 height=627>");
			if (_fileName2.Length > 0)
				t.Append("<img src='" + pathWeb2 + "' border=\"0\" width=470 height=627>");

			t.Append("</td></tr></table>");

			output.Write(t.ToString());
		}
	}
}
