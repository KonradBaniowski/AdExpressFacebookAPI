

using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;

/// <summary>
/// Class to register or upadte client's universes 
/// </summary>
public partial class Private_Universe_RegisterUniverse : TNS.AdExpress.Web.UI.PrivateWebPage {

	#region Variables
	/// <summary>
	/// Id Universe client description
	/// </summary>
	protected Int64 idUniverseClientDescription = 0;

	/// <summary>
	/// Branch type product or media
	/// </summary>
	string stringBranch = "";

    protected int _nbMaxItemByLevel = 1000;
	#endregion

	#region Constructor
	/// <summary>
		/// Constructor
		/// </summary>
		public Private_Universe_RegisterUniverse()
		: base() {
		}
		#endregion

	#region Page Load
		/// <summary>
	/// Page loading
	/// </summary>
	/// <param name="sender">sender</param>
	/// <param name="e">Arguments</param>
	protected void Page_Load(object sender, EventArgs e) {
		try {


            switch (_webSession.CurrentModule)
            {               
                case WebConstantes.Module.Name.INDICATEUR:
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    _nbMaxItemByLevel = 100000;
                    break;
            }

			stringBranch = Page.Request.QueryString.Get("brancheType");
			idUniverseClientDescription = Int64.Parse(Page.Request.QueryString.Get("idUniverseClientDescription"));

			#region Buttons and Langage parameters
			//Modification de la langue pour les Textes AdExpress
			//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls, _webSession.SiteLanguage);
			//okButton.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
			//okButton.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";
			#endregion

			#region Creating universe available group

			//Liste des groupes d'univers
			DataSet ds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);

			directoryDropDownList.DataSource = ds.Tables[0];
			directoryDropDownList.DataTextField = ds.Tables[0].Columns[1].ToString();
			directoryDropDownList.DataValueField = ds.Tables[0].Columns[0].ToString();
			directoryDropDownList.DataBind();

			//Liste des univers sauvegardés
			DataSet dsUniverses = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetData(_webSession, string.Empty, string.Empty);

			directoryDropDownList.EnableViewState = false;
			universeDropDownList.EnableViewState = false;

			#endregion

			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("UniverseJavaScriptFunctions"))
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UniverseJavaScriptFunctions", UniverseJavaScriptFunctions(dsUniverses, ds));

			directoryDropDownList.Attributes.Add("onChange", "fillUniverses(this.options[this.selectedIndex].value);");
			universeDropDownList.Attributes.Add("onChange", "selectUniverse();");
			universeTextBox.Attributes.Add("onKeypress", "deleteSelectedUniverse();");
		

		}
		catch (System.Exception exc) {
			if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
			}
		}

	}
	#endregion

	#region Javascript
	/// <summary>
	/// Génére les javascripts utilisés pour la sauvegarde des univers
	/// </summary>
	/// <param name="ds">Liste des univers sauvegardés</param>
	/// <param name="dsDir">Liste des répertoires</param>
	/// <returns>Code Javascript</returns>
	private string UniverseJavaScriptFunctions(DataSet ds, DataSet dsDir) {


		StringBuilder script = new StringBuilder(2000);
		int i = 0, k = 0;

		script.Append("<script language=\"JavaScript\">");
		script.Append("\r\n  var directories = new Array(); ");
		universeDropDownList.Items.Clear();
		universeDropDownList.Items.Insert(k, new ListItem("------------------", "0"));
		k++;
		foreach (DataRow dr in ds.Tables[0].Rows) {

			if (dr["ID_UNIVERSE_CLIENT"] != System.DBNull.Value) {
				script.Append("\r\n directories[" + i + "] = new Array();");
				script.Append("\r\n directories[" + i + "][\"ID_GROUP_UNIVERSE_CLIENT\"] = \"" + dr["ID_GROUP_UNIVERSE_CLIENT"].ToString() + "\"; ");
				script.Append("\r\n directories[" + i + "][\"ID_UNIVERSE_CLIENT\"] = \"" + dr["ID_UNIVERSE_CLIENT"].ToString() + "\"; ");
				script.Append("\r\n directories[" + i + "][\"UNIVERSE_CLIENT\"] = \"" + dr["UNIVERSE_CLIENT"].ToString() + "\"; ");
				i++;
			}
			if (dsDir.Tables[0].Rows.Count > 0 && Int64.Parse(dsDir.Tables[0].Rows[0]["ID_GROUP_UNIVERSE_CLIENT"].ToString()) == Int64.Parse(dr["ID_GROUP_UNIVERSE_CLIENT"].ToString())
				&& dr["ID_UNIVERSE_CLIENT"] != System.DBNull.Value) {
				universeDropDownList.Items.Insert(k, new ListItem(dr["UNIVERSE_CLIENT"].ToString(), dr["ID_UNIVERSE_CLIENT"].ToString()));
				k++;
			}

		}

		script.Append("\r\n function verif()");
		script.Append("\r\n {");
		script.Append("\r\n\t if (document.layers)");
		script.Append("\r\n\t {");
		script.Append("\r\n\t theForm = document.forms.Form1;");
		script.Append("\r\n\t }");
		script.Append("\r\n\t else");
		script.Append("\r\n\t {");
		script.Append("\r\n\t theForm = document.Form1;");
		script.Append("\r\n\t }");
		script.Append("\r\n}");

		script.Append("\r\n function selectUniverse()");
		script.Append("\r\n {");
		script.Append("\r\n\t theForm.universeTextBox.value=\"\";");
		script.Append("\r\n }");

		script.Append("\r\n function deleteSelectedUniverse()");
		script.Append("\r\n {");
		script.Append("\r\n\t theForm.universeDropDownList.options.selectedIndex = 0;");
		script.Append("\r\n }");

		script.Append("\r\n function fillUniverses(codeDirectory)");
		script.Append("\r\n {");
		script.Append("\r\n\t verif();");
		script.Append("\r\n\t if(codeDirectory>0)");
		script.Append("\r\n\t {");
		script.Append("\r\n\t\t theForm.universeDropDownList.options.length = 0;");
		script.Append("\r\n\t\t\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(\"------------------\",\"0\");");
		script.Append("\r\n\t\t for (var i=0;i<directories.length;i++)");
		script.Append("\r\n\t\t {");
		script.Append("\r\n\t\t\t if(i==0)");
		script.Append("\r\n\t\t\t {");
		script.Append("\r\n\t\t\t }");
		script.Append("\r\n\t\t\t if(parseInt(directories[i][\"ID_GROUP_UNIVERSE_CLIENT\"]) == parseInt(codeDirectory))");
		script.Append("\r\n\t\t\t {");
		script.Append("\r\n\t\t\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(directories[i][\"UNIVERSE_CLIENT\"],directories[i][\"ID_UNIVERSE_CLIENT\"]);");
		script.Append("\r\n\t\t\t }");
		script.Append("\r\n\t\t }");
		script.Append("\r\n\t\t theForm.universeDropDownList.options.selectedIndex = 0;");
		script.Append("\r\n\t }");
		script.Append("\r\n\t else");
		script.Append("\r\n\t {");
		script.Append("\r\n\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(\"------------------\",\"0\");");
		script.Append("\r\n\t }");

		script.Append("\r\n }");
		script.Append("\r\n </script>");

		return (script.ToString());
	}
	#endregion

	#region GetBrancheType
	/// <summary>
	/// Obtient la branche de l'univers
	/// </summary>
	/// <param name="stringBranch">chaine de caractère representant une branche</param>
	/// <returns>branche de l'univers</returns>
	private TNS.AdExpress.Constantes.Classification.Branch.type GetBrancheType(string stringBranch) {

		if (stringBranch.Equals(TNS.AdExpress.Constantes.Classification.Branch.type.media.GetHashCode().ToString())) {
			return TNS.AdExpress.Constantes.Classification.Branch.type.media;
		}
		else if (stringBranch.Equals(TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString())) {
			return TNS.AdExpress.Constantes.Classification.Branch.type.product;
		}
        else if (stringBranch.Equals(TNS.AdExpress.Constantes.Classification.Branch.type.advertisingAgency.GetHashCode().ToString())) {
            return TNS.AdExpress.Constantes.Classification.Branch.type.advertisingAgency;
        }
        else if (stringBranch.Equals(TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType.GetHashCode().ToString()))
        {
            return TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType;
        }
        else if (stringBranch.Equals(TNS.AdExpress.Constantes.Classification.Branch.type.profession.GetHashCode().ToString()))
        {
            return TNS.AdExpress.Constantes.Classification.Branch.type.profession;
        }
		else
			return 0;
	}
	#endregion

	#region Ok Button Click
	/// <summary>
	/// Ok button click event
	/// </summary>
	/// <param name="sender">sender</param>
	/// <param name="e">Event Args</param>
	protected void okButton_Click(object sender, EventArgs e) {
		
		
		//Dictionary<int,TNS.AdExpress.Classification.AdExpressUniverse> universes = _webSession.PrincipalProductUniverses;

		Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
		try {

			if (directoryDropDownList.Items.Count != 0) {

				string idSelectedUniverse = Request.Form.GetValues("universeDropDownList")[0];
				string idSelectedDirectory = Request.Form.GetValues("directoryDropDownList")[0];

				//Identification de la branche de l'univers					
				TNS.AdExpress.Constantes.Classification.Branch.type branchType = GetBrancheType(stringBranch);

				//Get universe to save
				TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = GetUniverseToSave();

				if (adExpressUniverse == null || adExpressUniverse.Count() == 0) {
					// Erreur : Aucun groupe d'univers, veuillez en créer un.
					Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(927, _webSession.SiteLanguage)));
				}
				else {
					#region Sauvegarde de l'univers
					string universeName = CheckedText.CheckedAccentText(this.universeTextBox.Text);

					if (universeName.Length == 0 && !idSelectedUniverse.Equals("0")) {
						
						//Add AdExpress universe to collection
						universes.Add(universes.Count, adExpressUniverse);

						if (UniversListDataAccess.UpdateUniverse(Int64.Parse(idSelectedUniverse), _webSession, idUniverseClientDescription, branchType.GetHashCode(), universes)) {

							// Validation : confirmation d'enregistrement de la requête
							_webSession.Source.Close();
							Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(921, _webSession.SiteLanguage)));
						}
						else {
							// Erreur : Echec de l'enregistrement de la requête	
							_webSession.Source.Close();
							Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(922, _webSession.SiteLanguage)));
						}
					}
					else if (universeName.Length != 0 && universeName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT) {
						if (!UniversListDataAccess.IsUniverseExist(_webSession, universeName)) {
							
							//Add AdExpress universe to collection
							universes.Add(universes.Count, adExpressUniverse);

							if (idSelectedDirectory != null && idSelectedDirectory.Length > 0 && UniversListDataAccess.SaveUniverse(Int64.Parse(idSelectedDirectory), universeName, universes, branchType, idUniverseClientDescription, _webSession)) {
								// Validation : confirmation d'enregistrement de l'univers
								_webSession.Source.Close();
								Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(921, _webSession.SiteLanguage)));
							}
							else {
								// Erreur : Echec de l'enregistrement de l'univers
								_webSession.Source.Close();
								Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(922, _webSession.SiteLanguage)));
							}
						}
						else {
							// Erreur : univers déjà existant
							_webSession.Source.Close();
							Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(923, _webSession.SiteLanguage)));
						}
					}
					else if (universeName.Length == 0) {
						// Erreur : Le champs est vide
						_webSession.Source.Close();
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));
					}
					else {
						// Erreur : suppérieur à 50 caractères
						_webSession.Source.Close();
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
					}
				
					#endregion
				}
				}

			else {
				// Erreur : Impossible de sauvegarder, pas de groupe d'univers créé
				Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(925, _webSession.SiteLanguage)));
			}
		}       
        catch (System.Exception err)
        {
            if (err.GetType() == typeof(TNS.Classification.Universe.SecurityException) ||
                    err.GetBaseException().GetType() == typeof(TNS.Classification.Universe.SecurityException))
            {
                _webSession.Source.Close();
                Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(2285, _webSession.SiteLanguage)));
            }
            else if (err.GetType() == typeof(TNS.Classification.Universe.CapacityException))
            {
                _webSession.Source.Close();
                Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(2286, _webSession.SiteLanguage)));
            }
            else if (err.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, err, _webSession));
			}
		}
	}
	#endregion

	#region SetUniverse
	/// <summary>
	/// Define universe
	/// </summary>
	/// <param name="branchType">branch Type de</param>
	private Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> SetUniverse(TNS.AdExpress.Constantes.Classification.Branch.type branchType) {
		Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = _webSession.PrincipalProductUniverses;
		
		switch (branchType) {			
			case TNS.AdExpress.Constantes.Classification.Branch.type.product:
				universes = _webSession.PrincipalProductUniverses;
				break;
			case TNS.AdExpress.Constantes.Classification.Branch.type.media:
                if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA)
                    universes = _webSession.SecondaryMediaUniverses;
                else
                    universes = _webSession.PrincipalMediaUniverses;
				break;
			case TNS.AdExpress.Constantes.Classification.Branch.type.advertisingAgency:
                universes = _webSession.PrincipalAdvertisingAgnecyUniverses;
                break;
			default: break;
		}

		return universes;
	}

	/// <summary>
	/// Get build adepress Universe from parameters passed by the opener window.
	/// This universe will be store in database table My_session
	/// </summary>
	/// <returns>AdExpress Universe</returns>
	private TNS.AdExpress.Classification.AdExpressUniverse GetUniverseToSave() {
		List<long> oldGroupIdList = null;
		//long oldLevelId = -1;
		NomenclatureElementsGroup oGroup = null;
		int groupIndex =0;
		TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = null;
		TNS.AdExpress.Constantes.Classification.Branch.type branchType = GetBrancheType(stringBranch);
		bool first = true;

		foreach (string currentKey in Page.Request.Form.AllKeys) {
			
			if (currentKey.Equals("LevelsIdsHiddenField")) {
			
				oldGroupIdList = new List<long>();

				switch (branchType) {
					case TNS.AdExpress.Constantes.Classification.Branch.type.product:
						adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.product);
						break;
					case TNS.AdExpress.Constantes.Classification.Branch.type.media:
						adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
						break;
                    case TNS.AdExpress.Constantes.Classification.Branch.type.advertisingAgency:
                        adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.advertisingAgency);
                        break;
                    case TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType:
                        adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.advertisementType);
                        break;
                    case TNS.AdExpress.Constantes.Classification.Branch.type.profession:
                        adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.profession);
                        break;
					default: return null;
				}
				
				string[] levelsIdsHiddenField = Page.Request.Form.GetValues(currentKey);

				if (levelsIdsHiddenField != null && levelsIdsHiddenField[0] != null && levelsIdsHiddenField[0].ToString().Trim().Length > 0) {
					
					string[] levelsArr = levelsIdsHiddenField[0].Split('|');
                   
					for (int i = 0; i < levelsArr.Length; i++) {
						string[] tempArr = levelsArr[i].Split(':');
						string[] levelParams = tempArr[0].Split('_');
                        string[] tempArr2 = null;
						//Create a new group
						if (!oldGroupIdList.Contains(long.Parse(levelParams[0]))) {
							if (!first && oGroup != null && oGroup.Count()>0) adExpressUniverse.AddGroup(adExpressUniverse.Count(), oGroup);
							oGroup = new NomenclatureElementsGroup(groupIndex, (AccessType)long.Parse(levelParams[1]));
							first = false;
						}

						if (oGroup != null && tempArr[1] != null && tempArr[1].Length>0) {
                            tempArr2 =  tempArr[1].Split(',');
                            if (tempArr2 != null && tempArr2.Length > _nbMaxItemByLevel) throw new TNS.Classification.Universe.CapacityException("Dépassement du nombre d'éléments autorisés pour un niveau");
							
							oGroup.AddItems(long.Parse(levelParams[2]), tempArr[1]);
						}
						if (!oldGroupIdList.Contains(long.Parse(levelParams[0])))
							oldGroupIdList.Add(long.Parse(levelParams[0]));
					}
					if (!first) adExpressUniverse.AddGroup(adExpressUniverse.Count(), oGroup);
				}
			}
		}

        return adExpressUniverse;
	}
	#endregion
}
