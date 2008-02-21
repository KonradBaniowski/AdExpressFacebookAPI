#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of creation: 05/08/5005
#endregion
using System;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using System.Drawing;


namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// This class is just used to inherit the chart class of the DUNDAS.
	/// This webcontrol is added in the ASPX page but is parameterized in the other classes like PDVPlanChartUI or PeriodicityPlanChartUI
	/// The main objective of doing this was that as we had three tabs in which graphs were to be shown we couldnt add 3 controls in the ASPX.
	/// So we have added one control APPMChartUI in the ASPX page and then the reference to this object is sent to our classes which 
	/// parameterized it according to their needs. 
	/// This also helped us do the development in parallel as everyone had its own class for the chart. 
	/// </summary>
	public class APPMChartUI:Chart
	{
		#region constructor
		/// <summary>
		/// Constructor of the class
		/// </summary>
		public APPMChartUI():base(){
		}
		#endregion

		
	}
}
