#region Informations
// Auteur: D. Mussuma
// Création: 21/01/2009
// Modification:
#endregion
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpress.Web.Controls.Selections {
	/// <summary>
	/// Control to  select Items in Classification for the Recap
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:SelectRecapItemsInClassificationWebControl runat=server></{0}:SelectRecapItemsInClassificationWebControl>")]
	public class SelectRecapItemsInClassificationWebControl : SelectItemsInClassificationWebControl {

		#region UpdateBranch
		/// <summary>
		/// Function to update branch Id
		/// </summary>
		/// <returns></returns>
		public override string UpdateBranch() {
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nfunction UpdateBranch(branchId){");
			js.Append("\r\n\t currentBranch = branchId;");			
			js.Append("\r\n\t SearchRecapClassificationItems('*',branchId);");
			js.Append("\r\n}\r\n");
			js.Append("\r\n-->\r\n</SCRIPT>");
			return (js.ToString());
		}
		#endregion

		#region GetRecapShowItemsScript

		/// <summary>
		/// Scrit to show items with Ajax
		/// </summary>
		/// <returns></returns>
		protected virtual string GetRecapShowItemsScript() {
			StringBuilder js = new StringBuilder(2000);
			string levelList = string.Empty;
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			if (_allowedLevels != null && _allowedLevels.Count > 0) {
				for (int i = 0; i < _allowedLevels.Count; i++) {
					#region Fonction show_
					js.Append("\r\nfunction showRecap_" + _allowedLevels[i].ID + "(){");
					js.Append("\r\n\t var oN = document.getElementById('universeZone_" + _allowedLevels[i].ID + "');");
					js.Append("\r\n\t " + this.GetType().Namespace + "." + this.GetType().Name + ".GetRecapItems(" + _allowedLevels[i].ID + ",currentBranch,savedParameters,showRecap_" + _allowedLevels[i].ID + "_callback);");
					js.Append("\r\n}");
					js.Append("\r\nfunction showRecap_" + _allowedLevels[i].ID + "_callback(res){");
					js.Append("\r\n\tvar oN = document.getElementById('universeZone_" + _allowedLevels[i].ID + "');");
					js.Append("\r\n\toN.innerHTML = res.value;");
                    js.Append("\r\n\tActualizeListBox_" + this.ID + "('classificationList_" + _allowedLevels[i].ID + "', 'listBoxContent_" + _allowedLevels[i].ID + "', 'listBoxContainer_" + _allowedLevels[i].ID + "');");
					js.Append("\r\n}\r\n");
					#endregion				
				}
			}
			js.Append("\r\n-->\r\n</SCRIPT>");
			return (js.ToString());
		}
		#endregion
		
		#region Load
		/// <summary>
		/// Loading
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			AjaxPro.Utility.RegisterTypeForAjax(typeof(SelectRecapItemsInClassificationWebControl));
			base.OnLoad(e);
		}
		#endregion	

		#region SearchClassificationItems
		/// <summary>
		/// Search items in classification
		/// </summary>
		/// <returns>String items</returns>
		public virtual string SearchRecapClassificationItems() {
			StringBuilder js = new StringBuilder(2000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nfunction SearchRecapClassificationItems(keyWordToSearch,branchId){");//Debut fonction SearchRecapClassificationItems
			js.Append("\r\n\t var list;");
			js.Append("\r\n\t var oN = document.getElementById('GlobalSelectsItemsZone_" + this.ID + "');");
			js.Append("\r\n\t wordToSearch = keyWordToSearch;");
			js.Append("\r\n\t branchId = branchId;");
			js.Append("\r\n\t if(keyWordToSearch==null || keyWordToSearch.trim().length==0)alert('" + GetWebWord(_messageAlertNoWordInput, _siteLanguage) + "');"); 
			js.Append("\r\n\t else{ ");
			js.Append("\r\n\t\t switch(branchId){");
			for (int s = 0; s < _allowedBranchesIds.Count; s++) {
				js.Append("\r\n\t\t case " + _allowedBranchesIds[s] + " : ");
				js.Append("\r\n\t\t\t oN.innerHTML='" + InitMultipleSelectZoneHTML(_allowedBranchesIds[s]) + "';");
				foreach (UniverseLevel lv in UniverseBranches.Get(_allowedBranchesIds[s]).Levels) {
					if (_allowedLevels.Contains(lv)) {
						js.Append("\r\n\t\t\t showRecap_" + lv.ID + "();");
					}
				}
				js.Append("\r\n\t\t\t break;");
			}
			js.Append("\r\n\t\t }");
			js.Append("\r\n\t } ");
			js.Append("\r\n}\r\n");//Fin fonction SearchRecapClassificationItems
			js.Append("\r\n-->\r\n</SCRIPT>");
			return (js.ToString());
		}
		#endregion

		#region AjaxMethod
		[AjaxPro.AjaxMethod]
		public virtual string GetRecapItems(int universeLevelId, int branchId, AjaxPro.JavaScriptObject oParameters) {
			try {
				LoadSavedParameters(oParameters);
				DataTable dt = GetRecapData(universeLevelId, branchId);
				return GetMultipleSelectZone(universeLevelId, branchId, dt, false);
			}
			catch (System.Exception err) {
				string clientErrorMessage = OnAjaxMethodError(err);
				throw new Exception(clientErrorMessage);
			}

		}
		#endregion

		#region PreRender
		/// <summary>
		/// Pre Render
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("GetRecapShowItemsScript"))
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GetRecapShowItemsScript", GetRecapShowItemsScript());			
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SearchRecapClassificationItems"))
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SearchRecapClassificationItems", SearchRecapClassificationItems());			
		}
		#endregion
		
		#region Render
		/// <summary> 
		/// Render
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\n  SearchRecapClassificationItems('*'," + _defaultBranchId + ");");
			js.Append("\r\n-->\r\n</SCRIPT>");

			base.Render(output);
			output.Write(js.ToString());
		}
		#endregion

		#region  GetRecapData
		/// <summary>
		/// Get data for recap level
		/// </summary>
		/// <param name="universeLevelId">level target id</param>
		/// <param name="branchId">Branch id</param>
		/// <returns>datatable like [id_item,item]</returns>
		protected virtual DataTable GetRecapData(int universeLevelId, int branchId) {
			string accessItemIds = "";
			DataTable dt = null;
			try {
				_webSession = (WebSession)WebSession.Load(_idSession);
				accessItemIds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetAccessRights(_webSession, universeLevelId, "id_" + UniverseLevels.Get(universeLevelId).TableName,true);
                _lowerCase = true;
				//Show only access items when first access to the current branch
				if (!string.IsNullOrEmpty(accessItemIds)) {
					dt = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetRecapItems(UniverseLevels.Get(universeLevelId).TableName, _webSession, _dBSchema, universeLevelId, _dimension).Tables[0];
				}
				else {
					//Else show all recap's sectors just for sector level
					if (universeLevelId == TNS.Classification.Universe.TNSClassificationLevels.SECTOR
						&& _webSession.CustomerLogin[CustomerRightConstante.type.subSectorAccess].Length==0
						&& _webSession.CustomerLogin[CustomerRightConstante.type.groupAccess].Length==0
						&& _webSession.CustomerLogin[CustomerRightConstante.type.segmentAccess].Length == 0
                        && _webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length == 0
                        &&  _webSession.CustomerLogin[CustomerRightConstante.type.brandAccess].Length == 0) {
						dt = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.SearchLevelDataAccess.GetItems(UniverseLevels.Get(universeLevelId).TableName,"*", _webSession, _dBSchema, _dimension).Tables[0];
					}
				}
			}
			catch (Exception err) {
				throw new TNS.AdExpress.Web.Controls.Exceptions.SelectRecapItemsInClassificationWebControlException("Impossible d'obtenir les données.", err);
			}
			return dt;
		}
		#endregion	
	
        #region GetItemsInAccess
        /// <summary>
        /// Obtains identifier list of  product classification items authorized for the current customer.
        /// </summary>
        /// <param name="universeLevelId"> product classification level Id of the current universe</param>      
        /// <returns>level classification </returns>
        /// <exception cref="TNS.AdExpress.Web.Controls.Exceptions.SelectRecapItemsInClassificationWebControlException">
        /// "Identifier of classification level unknown</exception>
        protected virtual CustomerRightConstante.type GetAccessRights(long universeLevelId)
        {           
            switch (universeLevelId)
            {
                //Get list of the identifiers of the authorized advertisers
                case TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER:
                    return CustomerRightConstante.type.advertiserAccess;

                //Get list of the identifiers of the authorized categories
                case TNS.Classification.Universe.TNSClassificationLevels.SECTOR:
                    return CustomerRightConstante.type.sectorAccess;

                //Getlist of the identifiers of the authorized sub categories
                case TNS.Classification.Universe.TNSClassificationLevels.SUB_SECTOR:
                    return CustomerRightConstante.type.subSectorAccess;

                //Get list of the identifiers of the authorized groups
                case TNS.Classification.Universe.TNSClassificationLevels.GROUP_:
                    return CustomerRightConstante.type.groupAccess;

                //Get level segment
                case TNS.Classification.Universe.TNSClassificationLevels.SEGMENT:
                    return CustomerRightConstante.type.segmentAccess;
                //Get level brand
                case TNS.Classification.Universe.TNSClassificationLevels.BRAND:
                    return CustomerRightConstante.type.brandAccess;
                default:
                    throw (new Exceptions.SelectRecapItemsInClassificationWebControlException("Identifier of classification level unknown"));
            }
            
        }

        #endregion

	}
}
