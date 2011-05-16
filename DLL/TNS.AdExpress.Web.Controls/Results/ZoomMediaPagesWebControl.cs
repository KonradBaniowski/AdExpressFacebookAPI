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
using TNS.AdExpressI.Visual;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpress.Domain.Classification;

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
        /// <summary>
        /// Is Blur
        /// </summary>
        protected bool _isBlur;
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

        /// <summary>
        /// Page anchor
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public bool IsBlur {
            get {
                return _isBlur;
            }

            set {
                _isBlur = value;
            }
        }
		#endregion

		protected override void Render(HtmlTextWriter output) {
			string pathWeb1 = _idMedia + "/" + _dateCover + "/" + _fileName1 + "";
			string pathWeb2 = _idMedia + "/" + _dateCover + "/" + _fileName2 + "";
			StringBuilder t = new StringBuilder(3000);
            Int64 vehicleId = 0;
            if (VehiclesInformation.Contains(Vehicles.names.press))
                vehicleId = VehiclesInformation.Get(Vehicles.names.press).DatabaseId;
            else if (VehiclesInformation.Contains(Vehicles.names.internationalPress))
                vehicleId = VehiclesInformation.Get(Vehicles.names.internationalPress).DatabaseId;
            else if (VehiclesInformation.Contains(Vehicles.names.magazine))
                vehicleId = VehiclesInformation.Get(Vehicles.names.magazine).DatabaseId;
            else if (VehiclesInformation.Contains(Vehicles.names.newspaper))
                vehicleId = VehiclesInformation.Get(Vehicles.names.newspaper).DatabaseId;

            object[] parameters = new object[2];
            parameters[0] = vehicleId;
            parameters[1] = pathWeb1;

            IVisual visual = (IVisual)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);



            t.Append("<table><tr><td>");
            t.Append("<img src='" + visual.GetVirtualPath(_isBlur) + "' border=\"0\" width=470 height=627>");
            if (_fileName2.Length > 0) {
                parameters = new object[2];
                parameters[0] = vehicleId;
                parameters[1] = pathWeb2;
                visual = (IVisual)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                t.Append("<img src='" + visual.GetVirtualPath(_isBlur) + "' border=\"0\" width=470 height=627>");
            }

            t.Append("</td></tr></table>");

			output.Write(t.ToString());
		}
	}
}
