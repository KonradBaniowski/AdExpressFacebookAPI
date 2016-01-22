#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 22/03/2007
// Date de modification: 
//
#endregion

using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// AdNetTrack detail level selection
	/// </summary>
	[ToolboxData("<{0}:GenericAdNetTrackLevelDetailSelectionWebControl runat=server></{0}:GenericAdNetTrackLevelDetailSelectionWebControl>")]
	public class GenericAdNetTrackLevelDetailSelectionWebControl : GenericMediaLevelDetailSelectionWebControl{

		#region Variables
		#endregion

		#region Variables MMI
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public GenericAdNetTrackLevelDetailSelectionWebControl():base(){
		}
		#endregion

		#region Ev�nements

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnInit(EventArgs e) {
			string onChange="";
			base.Init (e);
			
			#region on v�rifie que le niveau s�lectionn� � le droit d'�tre utilis�
			bool canAddDetail = false;
			try{
				canAddDetail = CanAddDetailLevel(_customerWebSession.GenericAdNetTrackDetailLevel);
			}
			catch{}
			if(!canAddDetail){
				// Niveau de d�tail par d�faut
				ArrayList levelsIds=new ArrayList();
				levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
				levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
				_customerWebSession.GenericMediaDetailLevel=new GenericDetailLevel(levelsIds,WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
			}
			#endregion

			#region Niveau de d�taille par d�faut

			_defaultDetail = new DropDownList();
			_defaultDetail.EnableViewState=true;
			_defaultDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value);
			_defaultDetail.ID = "defaultDetail_"+this.ID;
			//_defaultMediaDetail.Attributes["onchage"]="javascript:setLevel('"++"','"++"');";
			_defaultDetail.AutoPostBack=false;
			_defaultDetail.CssClass=_cssListBox;

			_defaultDetail.Items.Add(new ListItem("-------","-1"));
			int DefaultDetailLevelId=0;
			ArrayList DefaultDetailLevels=GetDefaultDetailLevels();
			foreach(GenericDetailLevel currentLevel in DefaultDetailLevels){
				if(CanAddDetailLevel(currentLevel))
					_defaultDetail.Items.Add(new ListItem(currentLevel.GetLabel(_customerWebSession.SiteLanguage),DefaultDetailLevelId.ToString()));
				DefaultDetailLevelId++;
			}
			Controls.Add(_defaultDetail);
			#endregion

			#region Niveau de d�taille par personnalis�
			// Obtient les niveaux de d�tail sauvegard�s		
			ArrayList genericDetailLevelsSaved=GetGenericDetailLevelsSaved();
			
			_customDetail = new DropDownList();
			_customDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value-4);
			_customDetail.ID = "customDetail_"+this.ID;
			_customDetail.AutoPostBack=false;
			_customDetail.CssClass=_cssListBox;

			_customDetail.Items.Add(new ListItem("-------","-1"));
			foreach(GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved){
				if(CanAddDetailLevel(currentGenericLevel) && currentGenericLevel.GetNbLevels<=_nbDetailLevelItemList){
					_customDetail.Items.Add(new ListItem(currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage),currentGenericLevel.Id.ToString()));
					_genericDetailLevelsSaved.Add(currentGenericLevel.Id,currentGenericLevel);
				}
			}

			Controls.Add(_customDetail);
			#endregion

			#region Niveau de d�taille par d�faut
			#region L1
			if(_nbDetailLevelItemList>=1){
				_l1Detail= new DropDownList();
				DetailLevelItemInit(_l1Detail,1);
			}
			#endregion

			#region L2
			if(_nbDetailLevelItemList>=2){
				_l2Detail= new DropDownList();
				DetailLevelItemInit(_l2Detail,2);
			}
			#endregion

			#region L3
			if(_nbDetailLevelItemList>=3){
				_l3Detail= new DropDownList();
				DetailLevelItemInit(_l3Detail,3);
			}
			#endregion

			#region L4
			if(_nbDetailLevelItemList>=4){
				_l4Detail= new DropDownList();
				DetailLevelItemInit(_l4Detail,4);
			}
			#endregion

			#endregion

			#region Ecriture des onchange

			#region OnChange _defaultDetail 
			onChange="javascript:setLevel('"+_customDetail.ID+"'";;
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";
			}
			onChange+=");";
			_defaultDetail.Attributes["onchange"]=onChange;
			#endregion

			#region OnChange _customDetail 
			onChange="javascript:setLevel('"+_defaultDetail.ID+"'";
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";
			}
			onChange+=");";
			_customDetail.Attributes["onchange"]=onChange;
			#endregion

			#endregion

		}
		#endregion

		#region Load

		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			//base.OnLoad (e);
			ArrayList levels=new ArrayList();

			_customerGenericDetailLevel=_customerWebSession.GenericAdNetTrackDetailLevel;

			#region Gestion de la s�lection
			if (Page.IsPostBack){
				if(int.Parse(_defaultDetail.SelectedValue)>=0){
					if (Page.Request.Form.GetValues("defaultDetail_"+this.ID) != null && Page.Request.Form.GetValues("defaultDetail_"+this.ID).Length>0){
						//added by gragneau on december, 12th
						_customerGenericDetailLevel=(GenericDetailLevel) GetDefaultDetailLevels()[int.Parse(Page.Request.Form.GetValues("defaultDetail_"+this.ID)[0])];
					}
					// Pb ICI TODO
					_customerGenericDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels;
				}
				if(int.Parse(_customDetail.SelectedValue)>=0){
					//_customerWebSession.GenericMediaDetailLevel=(GenericDetailLevel) _currentModule.DefaultMediaDetailLevels[int.Parse(Page.Request.Form.GetValues("defaultDetail_"+this.ID)[0])];
					if (Page.Request.Form.GetValues("customDetail_"+this.ID) != null && Page.Request.Form.GetValues("customDetail_"+this.ID).Length>0){
						_customerGenericDetailLevel=(GenericDetailLevel)_genericDetailLevelsSaved[Int64.Parse(Page.Request.Form.GetValues("customDetail_"+this.ID)[0])];
					}
					_customerGenericDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels;
				}
				if(_nbDetailLevelItemList>=1 && int.Parse(_l1Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l1Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=2 && int.Parse(_l2Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l2Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=3 &&int.Parse(_l3Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l3Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=4 && int.Parse(_l4Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l4Detail.SelectedValue));
				}
				if(levels.Count>0){
					_customerGenericDetailLevel=new GenericDetailLevel(levels,WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
					//_customerWebSession.GenericMediaDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.customLevels;
				}
			}
			else{
				if(_customerWebSession.GenericMediaDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels){
					int index=-1;
					foreach(GenericDetailLevel currentLevel in GetDefaultDetailLevels()){
						if(CanAddDetailLevel(currentLevel))index++;
						if(currentLevel.EqualLevelItems(_customerGenericDetailLevel))_defaultDetail.SelectedValue=index.ToString();
					}
				}
				if(_customerGenericDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels){
					//					TODO					
					foreach(GenericDetailLevelSaved currentLevel in _genericDetailLevelsSaved.Values){
						if(CanAddDetailLevel(currentLevel) && currentLevel.EqualLevelItems(_customerWebSession.GenericMediaDetailLevel))_customDetail.SelectedValue=currentLevel.Id.ToString();
					}
				}
				if(_customerGenericDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.customLevels){
					if(_nbDetailLevelItemList>=1 && _customerGenericDetailLevel.GetNbLevels>=1 && _customerGenericDetailLevel.LevelIds[0]!=null){
						_l1Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[0]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=2 && _customerGenericDetailLevel.GetNbLevels>=2 && _customerGenericDetailLevel.LevelIds[1]!=null){
						_l2Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[1]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=3 && _customerGenericDetailLevel.GetNbLevels>=3 && _customerGenericDetailLevel.LevelIds[2]!=null){
						_l3Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[2]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=4 && _customerGenericDetailLevel.GetNbLevels>=4 && _customerGenericDetailLevel.LevelIds[3]!=null){
						_l4Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[3]).GetHashCode().ToString();
					}
					
				}
			}
			#endregion

			_customerWebSession.GenericAdNetTrackDetailLevel=_customerGenericDetailLevel;
		}
		#endregion

		#endregion

		#region Private Methode
		/// <summary>
		/// Test si un niveau de d�tail peut �tre montr�
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <param name="currentDetailLevel">Niveau de d�tail</param>
		/// <param name="module">Module courrant</param>
		/// <returns>True s'il peut �tre ajout�</returns>
		protected override bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel,Int64 module){
			return(CanAddDetailLevel(currentDetailLevel));
		}
		/// <summary>
		/// Test si un niveau de d�tail peut �tre montr�
		/// </summary>
		/// <param name="currentDetailLevel">Niveau de d�tail</param>
		/// <returns>True s'il peut �tre ajout�</returns>
		private bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel){
			ArrayList allowedDetailLevelItems=AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems;
			foreach(DetailLevelItemInformation currentDetailLevelItem in currentDetailLevel.Levels){
				if( !allowedDetailLevelItems.Contains(currentDetailLevelItem))return(false);
				if(!CanAddDetailLevelItem(currentDetailLevelItem))return(false);
			}
			return(true);
		}

		/// <summary>
		/// Test si l'�l�ment de niveau de d�tail peut �tre montr�
		/// </summary>
		/// <param name="currentDetailLevelItem">El�ment de niveau de d�tail</param>
		/// <returns>True si oui false sinon</returns>
		private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem){
			switch(currentDetailLevelItem.Id){
					#region Annonceur produit, famille, classe, groupe, vari�t� 
				case DetailLevelItemInformation.Levels.sector:
				case DetailLevelItemInformation.Levels.subSector:			
				case DetailLevelItemInformation.Levels.advertiser:
					if(// Droit sur les niveaux de d�tail produit
						CheckProductDetailLevelAccess())return(true);
					return(false);
                case DetailLevelItemInformation.Levels.group:
                    if (// Droit sur les niveaux de d�tail group
                        CheckProductDetailLevelAccess() &&
                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_GROUP_LEVEL_ACCESS_FLAG)
                        ) return (true);
                    return (false);
                case DetailLevelItemInformation.Levels.segment:
                    if (// Droit sur les niveaux de d�tail vari�t�
                        CheckProductDetailLevelAccess() &&
                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)
                        ) return (true);
                    return (false);
					#endregion
                
					#region Products
				case DetailLevelItemInformation.Levels.product:
					if (// Droit sur les niveaux de d�tail produit
						CheckProductDetailLevelAccess() &&
						// Products level rights
						_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)
						) return (true);
					return (false);
				#endregion

					#region Marques
				case DetailLevelItemInformation.Levels.brand:
					if(// Droit sur les niveaux de d�tail produit
						CheckProductDetailLevelAccess() &&
						// Droit des Marques
						_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE)
						)return(true);
					return(false);
					#endregion

					#region Groupe de soci�t�
				case DetailLevelItemInformation.Levels.holdingCompany:
					if(// Droit sur les niveaux de d�tail produit
						CheckProductDetailLevelAccess() &&
						// Droit sur les groupe de soci�t�
						_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG)
						)return(true);
					return(false);
					#endregion
				default:
					return(true);
			}
		}
		#endregion		
	}
	
		
}
