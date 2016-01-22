#region Informations
// Auteur: D. Mussuma 
// Date de création: 28/07/2006
// Date de modification:
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.IO;

using TNS.AdExpress.Web.Core.Sessions;
using CustomerCst=TNS.AdExpress.Constantes.Customer;

using TNS.FrameWork.DB.Common;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpress.Web.Controls.Results.Appm
{
	/// <summary>
	/// Sert de classe de base abstraite pour des contrôles graphiques de l'APPM, tels que PdvPlanAppmChartWebControl et PeriodicityAppmChartWebControl.
	///  Cette classe fournit les méthodes et propriétés communes à tous les  contrôles graphiques de l'APPM. 
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:BaseAppmChartWebControl runat=server></{0}:BaseAppmChartWebControl>")]
	public abstract class BaseAppmChartWebControl : Chart
	{
		#region Variables	
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession=null;

		/// <summary>
		/// Identifiant cible APPM de base
		/// </summary>
		protected Int64 _idBaseTarget=0;
		
		/// <summary>
		/// Identifiant cible APPM suppplémentaire
		/// </summary>
		protected Int64 _idAdditionalTarget=0;

		/// <summary>
		/// Identifiant vague APPM 
		/// </summary>
		protected Int64 _idWave=0;

		/// <summary>
		/// Date de début
		/// </summary>
		protected int _dateBegin=0;

		/// <summary>
		/// Date de fin
		/// </summary>
		protected int _dateEnd = 0;
		
		/// <summary>
		/// Obtient ou définit le type de l'image
		/// </summary>		
		protected ChartImageType _imageType = ChartImageType.Jpeg;

		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;
		
		#endregion

		#region Accesseurs	

		
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">Source de données</param>
		/// <param name="appmImageType">Type de l'image Appm (jpg, flash...)</param>
        public BaseAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType): base()
        {
			_customerWebSession=webSession;

			if(_customerWebSession!=null){
				_idBaseTarget=	_idBaseTarget = Int64.Parse(_customerWebSession.GetSelection(_customerWebSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
                _idAdditionalTarget = Int64.Parse(_customerWebSession.GetSelection(_customerWebSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));
                _idWave=Int64.Parse(_customerWebSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
				_dateBegin = int.Parse(_customerWebSession.PeriodBeginningDate);
				_dateEnd = int.Parse(_customerWebSession.PeriodEndDate);
				_imageType = appmImageType;
				
			}
			_dataSource = dataSource;
            this.ImageUrl = WebApplicationParameters.DundasConfiguration.ImageURL;
		}
		#endregion
	
		#region Méthodes Méthodes abstraites
		/// <summary>
		/// Définit les données au moment du design du contrôle. 
		/// </summary>
		public abstract void SetDesignMode();
		
		#endregion

		#region Render
		/// <summary>
		/// Overrided to add "param" tags for contextual menu managment
		/// </summary>
		/// <param name="writer">Writer</param>
		protected override void Render(HtmlTextWriter writer) {
			
			HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
			base.Render(txt);
			string str = txt.InnerWriter.ToString();
			int i = -1;
			if((i = str.IndexOf("<PARAM name=\"movie\""))>-1){
				str = str.Insert(i, "<PARAM name=\"wmode\" value=\"transparent\">");
			}
			writer.Write(str);
		}

		#endregion

		#region Add Logo
		/// <summary>
		/// Ajoute copyright
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="isNotFlashType">type image</param>
		protected virtual void AddCopyRight(WebSession webSession, bool isNotFlashType) {
			if (isNotFlashType) {
				Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + " "+ DateTime.Now.Year.ToString());
				title.Font = new Font("Arial", (float)8);
				title.DockInsideChartArea = false;
				title.Docking = Docking.Bottom;
				title.Alignment = ContentAlignment.BottomCenter;
				this.Titles.Add(title);				
				 				
			}
		}
		#endregion

	}
}
