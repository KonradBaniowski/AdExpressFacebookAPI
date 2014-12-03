using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Composant affichant la liste des annonceurs
	/// Il est utilisé dans la sélection de l'annonceur ainsi que dans la sous sélection
	/// au niveau des pages de résultats.
	/// Suivant la valeur de l'entier buttonTarget des actions sont réalisées.
	/// 
	/// </summary>
	
	[ToolboxData("<{0}:AdvertiserSelectionWebControl runat=server></{0}:AdvertiserSelectionWebControl>")]
	public class AdvertiserSelectionWebControl: System.Web.UI.WebControls.CheckBoxList{

		#region Variables
		     
		/// <summary>
		/// Dataset contenant la liste des annonceurs
		/// </summary>
		protected DataSet dsListAdvertiser;
		/// <summary>
		/// Session
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Text saisie pour le choix de la recherhce
		/// </summary>
		protected string advertiserText;
		/// <summary>
		/// Booléen vrai si groupe de société sélectionné 
		/// </summary>
		protected bool holdingCompanyBool;
		/// <summary>
		/// Booléen vrai si annonceur sélectionné
		/// </summary>
		protected bool advertiserBool;
		/// <summary>
		/// Booléen vrai si marque
		/// </summary>
		protected bool brandBool;
		/// <summary>
		/// Booléen vrai si produit sélectionné
		/// </summary>
		protected bool productBool;
		/// <summary>
		/// Booléen vrai si famille sélectionnée
		/// </summary>
		protected bool sectorBool;
		/// <summary>
		///  Booléen vrai si classe sélectionnée
		/// </summary>
		protected bool subSectorBool;
		/// <summary>
		/// Booléen vrai si groupe sélectionnée 
		/// </summary>
		protected bool groupBool;
		/// <summary>
		/// Booléen vrai si variété sélectionnée 
		/// </summary>
		protected bool segmentBool;
		/// <summary>
		/// Tableau avec la liste des groupe de société en accès
		/// </summary>
		protected string[] tabHoldingAccess;
		/// <summary>
		/// Tableau avec la liste des groupe de société en Exception
		/// </summary>
		protected string[] tabHoldingException;
		/// <summary>
		/// Tableau avec la liste des annonceurs en accès
		/// </summary>
		protected string[] tabAdvertiserAccess;
		/// <summary>
		/// Tableau avec la liste des annonceurs en Exception
		/// </summary>
		protected string[] tabAdvertiserException;
		/// <summary>
		/// Tableau avec la liste des marques en accès
		/// </summary>
		protected string[] tabBrandAccess;
		/// <summary>
		/// Tableau avec la liste des marques en exception
		/// </summary>
		protected string[] tabBrandException;
		/// <summary>
		/// Tableau avec la liste des références en accès
		/// </summary>
		protected string[] tabProductAccess;
		/// <summary>
		/// Tableau avec la liste des références en Exception
		/// </summary>
		protected string[] tabProductException;
		/// <summary>
		/// Tableau avec la liste des familles en accès
		/// </summary>
		protected string[] tabSectorAccess;
		/// <summary>
		/// Tableau avec la liste des familles en Exception
		/// </summary>
		protected string[] tabSectorException;
		/// <summary>
		/// Tableau avec la liste des classes en accès
		/// </summary>
		protected string[] tabSubSectorAccess;
		/// <summary>
		/// Tableau avec la liste des classes en Exception
		/// </summary>
		protected string[] tabSubSectorException;
		/// <summary>
		/// Tableau avec la liste des groupes en accès
		/// </summary>
		protected string[] tabGroupAccess;
		/// <summary>
		/// Tableau avec la liste des groupes en Exception
		/// </summary>
		protected string[] tabGroupException;
		/// <summary>
		/// Tableau avec la liste des variétés en accès
		/// </summary>
		protected string[] tabSegmentAccess;
		/// <summary>
		/// Tableau avec la liste des variétés en Exception
		/// </summary>
		protected string[] tabSegmentException;
		/// <summary>
		/// Entier qui détermine qu'elle boutton 
		/// a été cliquer 1 pour valider
		/// 2 pour ok
		/// </summary>
		int buttonTarget;
		/// <summary>
		/// Message d'erreur
		/// </summary>
		string ErrorMessage="";
		/// <summary>
		/// si nbElement=2 un liste contient plus de 1000 éléments
		/// si nbElement=3 aucun élément n'a été séléctionnée
		/// </summary>
		int nbElement=1;
		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public AdvertiserSelectionWebControl(): base()
        {
			this.EnableViewState=true;
		
		}
		#endregion

		#region Accesseurs
		
		/// <summary>
		/// Obtient le dataset avec la liste des annonceurs 
		/// </summary>
		public DataSet DsListAdvertiser{
			get{return dsListAdvertiser;}
		}

		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Obtient ou définit le text saisie au niveau de l'annonceur
		/// </summary>
		public string AdvertiserText{
			get{return advertiserText;}
			set{advertiserText=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher holding Company
		/// </summary>
		public bool HoldingCompanyBool{
			get{return holdingCompanyBool;}
			set{holdingCompanyBool=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher advertiser 
		/// </summary>
		public bool AdvertiserBool{
			get{return advertiserBool;}
			set{advertiserBool=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher advertiser 
		/// </summary>
		public bool BrandBool
		{
			get{return brandBool;}
			set{brandBool=value;}
		}


		/// <summary>
		///  Obtient ou définit la valeur de la case à cocher références 
		/// </summary>
		public bool ProductBool{
			get{return productBool;}
			set{productBool=value;}
		}


		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher famille
		/// </summary>
		public bool SectorBool{
			get{return sectorBool;}
			set{sectorBool=value;}
		}


		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher classe
		/// </summary>
		public bool SubSectorBool{
			get{return subSectorBool;}
			set{subSectorBool=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher groupe
		/// </summary>
		public bool GroupBool{
			get{return groupBool;}
			set{groupBool=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher groupe
		/// </summary>
		public bool SegmentBool {
			get { return segmentBool; }
			set { segmentBool = value; }
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher groupe
		/// </summary>
		public int NbElement{
			get{return nbElement;}
			set{nbElement=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur du boutton qui a été
		/// cliqué
		/// </summary>
		public int ButtonTarget{
			get{return buttonTarget;}
			set{buttonTarget=value;}
		}
		
		#endregion

		#region Chargement de la page
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e) {
		
			#region variables
			string listHoldingCompany="";
			string listHoldingCompanyException="";
			string listHoldingCompanyAccess="";
			string listHoldingCompanyAutomatic="";
			string listAdvertiser="";
			string listAdvertiserAccess="";
			string listAdvertiserException="";
			string listAdvertiserAutomatic="";

			string listBrand="";
			string listBrandAccess="";
			string listBrandException="";
			string listBrandAutomatic="";

			string listProduct="";
			string listProductAccess="";
			string listProductException="";
			string listProductAutomatic="";	
				
			string listSector="";
			string listSectorException="";
			string listSectorAccess="";
			string listSectorAutomatic="";

			string listSubSector="";
			string listSubSectorException="";
			string listSubSectorAccess="";
			string listSubSectorAutomatic="";

			string listGroup="";
			string listGroupException="";
			string listGroupAccess="";
			string listGroupAutomatic="";

			string listSegment = "";
			string listSegmentException = "";
			string listSegmentAccess = "";
			string listSegmentAutomatic = "";



			bool parentBool=false;
			#endregion

			if(buttonTarget==-1){
				Items.Clear();
			}
	
			#region Bouton Ok
			if(buttonTarget==2 || buttonTarget==3){	
	//		if(buttonTarget==2){
				if(Page.IsPostBack){
					#region PostBack

					#region Création des listes
					foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items){
						string[] tabParent=currentItem.Text.Split('_') ;

						if(tabParent[0]=="Parent" && currentItem.Selected){
							parentBool=true;
							if(holdingCompanyBool)
							{
								listHoldingCompanyAccess+=currentItem.Value+",";
								listHoldingCompanyAutomatic+=currentItem.Value+",";
							}
							else if(advertiserBool)
							{
								listAdvertiserAccess+=currentItem.Value+",";
								listAdvertiserAutomatic+=currentItem.Value+",";
							}
							else if(brandBool)
							{
								listBrandAccess+=currentItem.Value+",";
								listBrandAutomatic+=currentItem.Value+",";
							}
							else if(sectorBool)
							{
								listSectorAccess+=currentItem.Value+",";
								listSectorAutomatic+=currentItem.Value+",";
							}
							else if(subSectorBool)
							{
								listSubSectorAccess+=currentItem.Value+",";
								listSubSectorAutomatic+=currentItem.Value+",";
							}
							else if (groupBool) {
								listGroupAccess += currentItem.Value + ",";
								listGroupAutomatic += currentItem.Value + ",";
							}

						}	
						else if(tabParent[0]=="Parent" && !currentItem.Selected){
							parentBool=false;
						}

						else if(tabParent[0]=="Children" && parentBool){

							 if(!currentItem.Selected){
								 if(holdingCompanyBool)
								 {
									 listAdvertiserException+=currentItem.Value+",";
								 }
								 else if(advertiserBool)
								 {
									 listProductException+=currentItem.Value+",";
								 }
								 else if(brandBool){
									 listProductException+=currentItem.Value+",";
								 }
								 else if(sectorBool)
								 {
									 listSubSectorException+=currentItem.Value+",";
								 }
								 else if(subSectorBool)
								 {
									 listGroupException+=currentItem.Value+",";
								 }
								 else if (groupBool) {
									 listSegmentException += currentItem.Value + ",";
								 }
							}
						}
						else if(tabParent[0]=="Children" && !parentBool){
							if(currentItem.Selected){
								if(holdingCompanyBool)
								{
									listAdvertiserAccess+=currentItem.Value+",";
								}
								else if(advertiserBool)
								{
									listProductAccess+=currentItem.Value+",";
								}
								else if(brandBool)
								{
									listProductAccess+=currentItem.Value+",";
								
								}
								else if(productBool)
								{
									listProductAccess+=currentItem.Value+",";
								}
								else if(sectorBool)
								{
									listSubSectorAccess+=currentItem.Value+",";
								}
								else if(subSectorBool)
								{
									listGroupAccess+=currentItem.Value+",";
								}
								else if(groupBool)
								{
									listSegmentAccess+=currentItem.Value+",";
								}

							}
						}
					
					
					}
					#endregion
				
					#region Initialisation des tableaux
					tabHoldingAccess=null;	
					tabHoldingException=null;
					tabAdvertiserAccess=null;
					tabAdvertiserException=null;
					tabBrandAccess=null;
					tabBrandException=null;
					tabProductAccess=null;	
					tabProductException=null;
					tabSectorAccess=null;
					tabSectorException=null;
					tabSubSectorAccess=null;
					tabSubSectorException=null;
					tabGroupAccess=null;
					tabGroupException=null;
					tabSegmentAccess = null;
					tabSegmentException = null;

					#endregion

					#region Création des listes et des tableaux
					//Liste Holding Company
					if(listHoldingCompanyAccess.Length!=0 && listHoldingCompanyAutomatic.Length!=0){
						listHoldingCompany+=listHoldingCompanyAccess;
						tabHoldingAccess=listHoldingCompanyAccess.Split(',');
					}
					else if(listHoldingCompanyAccess.Length!=0){
						listHoldingCompany+=listHoldingCompanyAccess;
						tabHoldingAccess=listHoldingCompanyAccess.Split(',');
					}
			
					if(listHoldingCompanyException.Length!=0){
						listHoldingCompany+=listHoldingCompanyAutomatic;
						tabHoldingException=listHoldingCompanyException.Split(',');
			
					}

					//Liste Advertiser
					if(listAdvertiserAccess.Length!=0 && listAdvertiserAutomatic.Length!=0){
						listAdvertiser+=listAdvertiserAccess;	
						tabAdvertiserAccess=listAdvertiserAccess.Split(',');
					}
					else if(listAdvertiserAccess.Length!=0){
						listAdvertiser+=listAdvertiserAccess;
						tabAdvertiserAccess=listAdvertiserAccess.Split(',');
					}
			 
			
					if(listAdvertiserException.Length!=0){
						listAdvertiser+=listAdvertiserAutomatic;
						tabAdvertiserException=listAdvertiserException.Split(',');
				
					}

					// Liste Marque					
					//if(listAdvertiserAccess.Length!=0 && listAdvertiserAutomatic.Length!=0)
					if (listBrandAccess.Length != 0 && listBrandAutomatic.Length != 0)
					{
						listBrand+=listBrandAccess;	
						tabBrandAccess=listBrandAccess.Split(',');
					}
					else if(listBrandAccess.Length!=0)
					{
						listBrand+=listBrandAccess;
						tabBrandAccess=listBrandAccess.Split(',');
					}
			 
			
					if(listBrandException.Length!=0)
					{
						listBrand+=listBrandAutomatic;
						tabBrandException=listBrandException.Split(',');
				
					}

					//Liste Product
					if(listProductAccess.Length!=0 && listProductAutomatic.Length!=0){
						listProduct+=listProductAccess;
						tabProductAccess=listProductAccess.Split(',');
					}else if(listProductAccess.Length!=0){
						listProduct+=listProductAccess;
						tabProductAccess=listProductAccess.Split(',');
					}

					if(listProductException.Length!=0){
						listProduct+=listProductAutomatic;	
						tabProductException=listProductException.Split(',');
					}


					//Liste Sector
					if(listSectorAccess.Length!=0 && listSectorAutomatic.Length!=0){
						listSector+=listSectorAccess;
						tabSectorAccess=listSectorAccess.Split(',');
					}
					else if(listSectorAccess.Length!=0){
						listSector+=listSectorAccess;
						tabSectorAccess=listSectorAccess.Split(',');
					}
			
					if(listSectorException.Length!=0){
						listSector+=listSectorAutomatic;
						tabSectorException=listSectorException.Split(',');
					}

					//Liste classe
					if(listSubSectorAccess.Length!=0 && listSubSectorAutomatic.Length!=0){
						listSubSector+=listSubSectorAccess;	
						tabSubSectorAccess=listSubSectorAccess.Split(',');
					}
					else if(listSubSectorAccess.Length!=0){
						listSubSector+=listSubSectorAccess;
						tabSubSectorAccess=listSubSectorAccess.Split(',');
					}		 
			
					if(listSubSectorException.Length!=0){
						listSubSector+=listSubSectorAutomatic;
						tabSubSectorException=listSubSectorException.Split(',');
					}


					//Liste Group
					if(listGroupAccess.Length!=0 && listGroupAutomatic.Length!=0){
						listGroup+=listGroupAccess;
						tabGroupAccess=listGroupAccess.Split(',');
					}else if(listGroupAccess.Length!=0){
						listGroup+=listGroupAccess;
						tabGroupAccess=listGroupAccess.Split(',');
					}

					if(listGroupException.Length!=0){
						listGroup+=listGroupAutomatic;	
						tabGroupException=listGroupException.Split(',');
					}

					//Liste segment
					if (listSegmentAccess.Length != 0 && listSegmentAutomatic.Length != 0) {
						listSegment += listSegmentAccess;
						tabSegmentAccess = listSegmentAccess.Split(',');
					}
					else if (listSegmentAccess.Length != 0) {
						listSegment += listSegmentAccess;
						tabSegmentAccess = listSegmentAccess.Split(',');
					}

					if (listSegmentException.Length != 0) {
						listSegment += listSegmentAutomatic;
						tabSegmentException = listSegmentException.Split(',');
					}
	
					//Suppression de la dernière ,
					if(listHoldingCompany.Length>0){
						listHoldingCompany=listHoldingCompany.Substring(0,listHoldingCompany.Length-1);
					}
					if(listAdvertiser.Length>0){
						listAdvertiser=listAdvertiser.Substring(0,listAdvertiser.Length-1);
					}
					if(listBrand.Length>0){
						listBrand=listBrand.Substring(0,listBrand.Length-1);
					}
					if(listProduct.Length>0){
						listProduct=listProduct.Substring(0,listProduct.Length-1);
					}
					if(listSector.Length>0){
						listSector=listSector.Substring(0,listSector.Length-1);
					}
					if(listSubSector.Length>0){
						listSubSector=listSubSector.Substring(0,listSubSector.Length-1);
					}
					if(listGroup.Length>0){
						listGroup=listGroup.Substring(0,listGroup.Length-1);
					}
					if (listSegment.Length > 0) {
						listSegment = listSegment.Substring(0, listSegment.Length - 1);
					}



					#endregion

					// Evènment sauvegarder
					if(buttonTarget==3){
						webSession.SelectionUniversAdvertiser=this.createTreeNode();
						//webSession.CurrentUniversAdvertiser=this.createTreeNode();
						webSession.Save();
					}
					
					if(buttonTarget==2){
						if(advertiserText.Length <2 ||  !TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(advertiserText)){
							advertiserText="";
						}

						// Vérifie que les listes soient inférieur à 1000 éléments
						if(listHoldingCompany.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER 
							&& listAdvertiser.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER 
							&& listBrand.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER 
							&& listProduct.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER
							&& listSegment.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER){
							// Requête pour récupérer la liste des éléments
							if((advertiserText.Length >=2 && TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(advertiserText)) 
								|| (listHoldingCompany.Length!=0 || listAdvertiser.Length!=0 || listBrand.Length!=0 || listProduct.Length!=0 || listSector.Length!=0 || listSubSector.Length!=0 || listGroup.Length!=0 || listSegment.Length!=0)){
								if(holdingCompanyBool || advertiserBool || productBool)
								{
									AdvertiserListDataAccess listAdvertiserDB=new AdvertiserListDataAccess(webSession,advertiserText,holdingCompanyBool,advertiserBool,productBool,false,listHoldingCompany,listAdvertiser,listProduct);
									dsListAdvertiser=listAdvertiserDB.DsListAdvertiser;
								}
								else if(sectorBool || subSectorBool || groupBool || segmentBool)
								{
									AdvertiserListDataAccess listAdvertiserDB=new AdvertiserListDataAccess(webSession,advertiserText,sectorBool,subSectorBool,groupBool,segmentBool,listSector,listSubSector,listGroup,listSegment);
									dsListAdvertiser=listAdvertiserDB.DsListAdvertiser;
								}
								else if(brandBool){
									AdvertiserListDataAccess listAdvertiserDB=new AdvertiserListDataAccess(webSession,advertiserText,brandBool,listBrand);
									dsListAdvertiser=listAdvertiserDB.DsListAdvertiser;								
								}
							}
						}
							// Message d'erreur : Vous devez sélectionner moins de 1000 éléments
						else{
                            ErrorMessage = GestionWeb.GetWebWord(2265, webSession.SiteLanguage);
						}
					}
					#endregion
				}
			}
			#endregion

			#region Bouton Valider ou Recall
			if(buttonTarget==1 || buttonTarget==5 || buttonTarget==-2){
			webSession.SelectionUniversAdvertiser=this.createTreeNode();
			webSession.Save();
			}

			if(buttonTarget==-2){
				this.Items.Clear();
			}

			#endregion

			#region Bouton Valider
			// Bouton valider dans la sousSelection
			if(buttonTarget==7){
				webSession.CurrentUniversAdvertiser=this.createTreeNode();
				if(webSession.CurrentUniversAdvertiser.Nodes.Count>0)
					webSession.Save();
				this.createItems();
			}
			#endregion
			

			base.OnLoad (e);
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			
			#region Ajout des items à partir de dsListAdvertiser
			// Bouton OK
			if(buttonTarget==2){
				if(Page.IsPostBack){
					
					Int64 idParentOld=-1;
					Int64 idParent;
					// Suppression des checkbox contenues dans la liste
					this.Items.Clear();

					#region 2 niveaux
					// Ajout des checkboxs dans la liste
					if(holdingCompanyBool || advertiserBool || sectorBool || subSectorBool || brandBool || groupBool){	
						if(dsListAdvertiser!=null){	
							foreach(DataRow currentRow in dsListAdvertiser.Tables["dsListAdvertiser"].Rows) {
				
								idParent=(Int64)currentRow[0];
								if(idParentOld!=idParent){
									this.Items.Add(new System.Web.UI.WebControls.ListItem("Parent_"+currentRow[1].ToString(),currentRow[0].ToString()));
									idParentOld=idParent;
								}					
								this.Items.Add(new System.Web.UI.WebControls.ListItem("Children_"+currentRow[3].ToString(),currentRow[2].ToString()));					
							}
						}
					}
					#endregion

					#region 1 niveau
					//if(productBool || groupBool){
					if (productBool || segmentBool) {
						if(dsListAdvertiser!=null){	
							foreach(DataRow currentRow in dsListAdvertiser.Tables["dsListAdvertiser"].Rows) {
								this.Items.Add(new System.Web.UI.WebControls.ListItem("Children_"+currentRow[1].ToString(),currentRow[0].ToString()));					

							}
						}
					}
					#endregion
				}
			}
			#endregion

			#region Ajout des items à partir de l'arbre SelectionUniversAdvertiser
			// Chargement de l'univers
			// Ajout des éléments dans listItem
			// Exécuter lors des évènmements suivant: 4 : bouton charger
			// 6 : bouton ok de la popup 8 : Retour à la sélection initiale
			// 9 : lors d'un chargement dans la page de résultat
			//if(buttonTarget==4 || buttonTarget==6 || buttonTarget==8 || buttonTarget==9){
			if(buttonTarget==4 || buttonTarget==3 || buttonTarget==8 || buttonTarget==9){
				//	if(buttonTarget==4){
				// Suppression des checkbox contenues dans la liste
				this.Items.Clear();
				int i=0;
				if(webSession.SelectionUniversAdvertiser.FirstNode!=null){
					if( ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess 
						|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.segmentException 
						|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
						|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException 
						){  
						//((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
						//|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException 
						foreach(System.Windows.Forms.TreeNode currentNode in webSession.SelectionUniversAdvertiser.Nodes){					
							//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
							System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
							checkBox1.Text="Children_"+((LevelInformation)currentNode.Tag).Text;
							checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
							this.Items.Add(checkBox1);
						}
					}
					else{
						foreach(System.Windows.Forms.TreeNode currentNode in webSession.SelectionUniversAdvertiser.Nodes){					
							//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
							System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
							checkBox1.Text="Parent_"+((LevelInformation)currentNode.Tag).Text;
							checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
							this.Items.Add(checkBox1);
							i=0;
							while(i<currentNode.Nodes.Count){
								System.Web.UI.WebControls.ListItem checkBox2 =new System.Web.UI.WebControls.ListItem();
								checkBox2.Text="Children_"+((LevelInformation)currentNode.Nodes[i].Tag).Text;
								checkBox2.Value=((LevelInformation)currentNode.Nodes[i].Tag).ID.ToString();
								this.Items.Add(checkBox2);
								i++;
							}
						}
					}
				}

			}
			#endregion
		}
		#endregion
		
		#region CreateItems
		/// <summary>
		/// Méthode pour créer la liste d'item à partir de l'arbre CurrentUniversAdvertiser
		/// </summary>
		protected void createItems(){
		
			this.Items.Clear();
			int i=0;
			if (((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess 
				|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.segmentException 
				||((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException 
				) {  

				//((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//|| ((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException 
				foreach(System.Windows.Forms.TreeNode currentNode in webSession.CurrentUniversAdvertiser.Nodes){					
					//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
					System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
					checkBox1.Text="Children_"+((LevelInformation)currentNode.Tag).Text;
					checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
					this.Items.Add(checkBox1);
						
					
				}
			}
			else{
				foreach(System.Windows.Forms.TreeNode currentNode in webSession.CurrentUniversAdvertiser.Nodes){					
					//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
					System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
					checkBox1.Text="Parent_"+((LevelInformation)currentNode.Tag).Text;
					checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
					this.Items.Add(checkBox1);
					i=0;
					while(i<currentNode.Nodes.Count){
						System.Web.UI.WebControls.ListItem checkBox2 =new System.Web.UI.WebControls.ListItem();
						checkBox2.Text="Children_"+((LevelInformation)currentNode.Nodes[i].Tag).Text;
						checkBox2.Value=((LevelInformation)currentNode.Nodes[i].Tag).ID.ToString();
						this.Items.Add(checkBox2);
						i++;
					}
				}
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			// Numéro du checkBox
			int i=0;
			int length=50000;
			if(dsListAdvertiser!=null)length=dsListAdvertiser.Tables[0].Rows.Count*500;
			System.Text.StringBuilder t=new System.Text.StringBuilder(length);
			
			
			// ok sauvegarder
			if(buttonTarget==2 ){
				if(Page.IsPostBack){		
				
					#region PostBack	
			
					#region Affichage
				
					if(dsListAdvertiser!=null){
						if(dsListAdvertiser.Tables[0].Rows.Count>0){	
							t.Append("<tr class=\"txtGris11Bold\">");												
							t.Append("<td bgcolor=#FFFFFF  >");
							t.Append(""+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
						}
					}
					t.Append("<tr height=5px bgcolor=\"#ffffff\"><td></td></tr>");
					t.Append("<tr><td bgcolor=\"#ffffff\"><img src=\"images/pixel.gif\" width=\"1\" height=\"5\"></td></tr>");
					t.Append("<tr><td bgcolor=\"#ffffff\"><img src=\"images/pixel.gif\" width=\"1\" height=\"20\"></td></tr> ");
					if(dsListAdvertiser!=null){
						if(dsListAdvertiser.Tables[0].Rows.Count>0){	
							t.Append("<tr> ");
							// liste des Annonceurs
							if(holdingCompanyBool){					
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(814,webSession.SiteLanguage)+"");}
							else if(advertiserBool){
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(813,webSession.SiteLanguage)+"");
							}
							else if(brandBool)
							{
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(1585,webSession.SiteLanguage)+"");
							}
							else if(productBool){
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"");
							}
							else if(SectorBool){
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"");
							}
							else if(subSectorBool){
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(966,webSession.SiteLanguage)+"");
							}
							else if(groupBool){
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(967,webSession.SiteLanguage)+"");
							}
							else if (segmentBool) {
								t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">" + GestionWeb.GetWebWord(1894, webSession.SiteLanguage) + "");
							}
				
							t.Append("</td>");
							t.Append("</tr>");	
						}
					}
					#endregion

					int nbLinesTest=0;
					
					//Tableau global
					t.Append("<tr><td bgColor=\"#ffffff\" vAlign=\"top\"><table>");			
					t.Append("<tr><td vAlign=\"top\">");

					Int64 idParentOld=-1;
					Int64 idParent;				
					string textParent;
					string textParentOld;
					int automatic=0;
				
					int start=-1;
					string checkBox="";	

					#region  2 niveaux
					if(holdingCompanyBool || advertiserBool || sectorBool || subSectorBool || brandBool || groupBool){
						if(dsListAdvertiser!=null)	{
							foreach(DataRow currentRow in dsListAdvertiser.Tables["dsListAdvertiser"].Rows){								
										
								#region Foreach 				
			
					
								idParent=(Int64)currentRow[0];
								textParent=currentRow[1].ToString();
								checkBox="";

								
						
								#region Parcours des tableaux
								// Parcours du tableau pour vérifier si l'élément doit être cocher

								// cas Groupe de société						
								if(holdingCompanyBool){
									if(tabHoldingAccess!=null){
										foreach(string item1  in tabHoldingAccess){
											if(item1==currentRow[0].ToString()){
												checkBox="checked";
												break;
											}
										}
									}
								}

								// cas Annonceur
								if(advertiserBool){
									if(tabAdvertiserAccess!=null){
										foreach(string item1  in tabAdvertiserAccess){
											if(item1==currentRow[0].ToString()){
												checkBox="checked";
												break;
											}
										}
									}
								}

								// cas Marque
								if(brandBool)
								{
									if(tabBrandAccess!=null)
									{
										foreach(string item1  in tabBrandAccess)
										{
											if(item1==currentRow[0].ToString())
											{
												checkBox="checked";
												break;
											}
										}
									}
								}

								// cas famille						
								if(sectorBool){
									if(tabSectorAccess!=null){
										foreach(string item1  in tabSectorAccess){
											if(item1==currentRow[0].ToString()){
												checkBox="checked";
												break;
											}
										}
									}
								}
						
								// cas classe
								if(subSectorBool){
									if(tabSubSectorAccess!=null){
										foreach(string item1  in tabSubSectorAccess){
											if(item1==currentRow[0].ToString()){
												checkBox="checked";
												break;
											}
										}
									}
								}

								// cas groupe
								if (groupBool) {
									if (tabGroupAccess != null) {
										foreach (string item1 in tabGroupAccess) {
											if (item1 == currentRow[0].ToString()) {
												checkBox = "checked";
												break;
											}
										}
									}
								}
								#endregion
						
					
								if(idParentOld!=idParent && start==0){																	
									if(nbLinesTest!=0){
										t.Append("</tr>");
										nbLinesTest=0;
									}
						
									if(checkBox=="checked"){
										automatic=1;
									}else{
										automatic=0;
									}
					
									t.Append("</table>");
									t.Append("</div>");
                                    t.Append("<table style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"100%\">");
									t.Append("<tr>");
									t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
                                    t.Append("<input type=\"checkbox\" " + checkBox + " onclick=\"integration('" + idParent + "'," + i + ",event)\" id=\"AdvertiserSelectionWebControl1_" + i + "\" name=\"AdvertiserSelectionWebControl1$" + i + "\">");
                                    t.Append("<label>"); //for=\"AdvertiserSelectionWebControl1_"+i+"\"
									t.Append(""+currentRow[1].ToString()+"");
									t.Append("</label>");
									t.Append("</td>");
									t.Append("<td width=\"100%\" align=right onClick=\"showHideContent('"+idParent+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
									t.Append("</tr>");
									t.Append("</table>");
									t.Append("<div id=\""+idParent+"Content\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
                                    t.Append("<table id=" + idParent + " style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width=\"100%\">");
									t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('"+idParent+"',"+i+")\" ID=\""+currentRow[0]+"\">");
									if(holdingCompanyBool){
										t.Append(GestionWeb.GetWebWord(816,webSession.SiteLanguage));
									}else if(advertiserBool || brandBool){
										t.Append(GestionWeb.GetWebWord(817,webSession.SiteLanguage));}
									else  if(sectorBool){
										t.Append(GestionWeb.GetWebWord(968,webSession.SiteLanguage));}
									else if(subSectorBool)
										t.Append(GestionWeb.GetWebWord(969,webSession.SiteLanguage));
									else if (groupBool)
										t.Append(GestionWeb.GetWebWord(1067,webSession.SiteLanguage));
						
						
									t.Append("</a></td></tr>");

									idParentOld=idParent;
									textParentOld=textParent;
									i++;
								}
								//Premier						
					
						
								if(idParentOld!=idParent && start!=0){

									if(checkBox=="checked"){
										automatic=1;
									}else{
										automatic=0;
									}

                                    t.Append("<table style=\"border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; border-bottom :#644883 1px solid; \" class=\"txtViolet11Bold\" cellpadding=0 cellspacing=0   width=\"100%\">");
									t.Append("<tr>");
									t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
                                    t.Append("<input type=\"checkbox\" " + checkBox + " onclick=\"integration('" + idParent + "'," + i + ",event)\" ID=\"AdvertiserSelectionWebControl1_" + i + "\" name=\"AdvertiserSelectionWebControl1$" + i + "\">");
									t.Append("<label for=\"AdvertiserSelectionWebControl1_"+i+"\">");
									t.Append(""+currentRow[1].ToString()+"");
									t.Append("</label>");
									t.Append("</td>");
									t.Append("<td width=\"100%\" align=right onClick=\"showHideContent('"+idParent+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");	
									t.Append("</tr>");
									t.Append("</table>");
									t.Append("<div id=\""+idParent+"Content\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\" >");
                                    t.Append("<table id=" + idParent + " style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width=\"100%\">");
									t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" style=\"TEXT-DECORATION: none\" onclick=\"allSelection('"+idParent+"',"+i+")\" ID=\""+currentRow[0]+"\">");
									if(holdingCompanyBool){
										t.Append(GestionWeb.GetWebWord(816,webSession.SiteLanguage));
									}else if(advertiserBool || brandBool) {
										t.Append(GestionWeb.GetWebWord(817,webSession.SiteLanguage));}
									else if(sectorBool){
										t.Append(GestionWeb.GetWebWord(968,webSession.SiteLanguage));}
									else if (subSectorBool)
										t.Append(GestionWeb.GetWebWord(969, webSession.SiteLanguage));
									else if (groupBool)
										t.Append(GestionWeb.GetWebWord(1067, webSession.SiteLanguage));
									t.Append("</a></td></tr>");	


									idParentOld=idParent;
									textParentOld=textParent;
									start=0;
									i++;
								}

								#region Parcours des tableaux
								checkBox="";
							
								if(automatic==0){	
									if(holdingCompanyBool){
										if(tabAdvertiserAccess!=null){
											foreach(string item1  in tabAdvertiserAccess){
												if(item1==currentRow[2].ToString()){
													checkBox="checked";
													break;
												}
											}
										}
									}

									if(advertiserBool || brandBool){
										if(tabProductAccess!=null){
											foreach(string item1  in tabProductAccess){
												if(item1==currentRow[2].ToString()){
													checkBox="checked";
													break;
												}
											}
										}
									}

									if(sectorBool){
										if(tabSubSectorAccess!=null){
											foreach(string item1  in tabSubSectorAccess){
												if(item1==currentRow[2].ToString()){
													checkBox="checked";
													break;
												}
											}
										}
									}

									if(SubSectorBool){
										if(tabGroupAccess!=null){
											foreach(string item1  in tabGroupAccess){
												if(item1==currentRow[2].ToString()){
													checkBox="checked";
													break;
												}
											}
										}
									}

									if (groupBool) {
										if (tabSegmentAccess != null) {
											foreach (string item1 in tabSegmentAccess) {
												if (item1 == currentRow[2].ToString()) {
													checkBox = "checked";
													break;
												}
											}
										}
									}
								}

							

								if(automatic==1) {
									checkBox="checked";	 

									if(holdingCompanyBool){
										if(tabAdvertiserException!=null){
											foreach(string item1  in tabAdvertiserException){
												if(item1==currentRow[2].ToString()){
													checkBox="";
													break;
												}
											}
										}
									}

									if(advertiserBool || brandBool){
										if(tabProductException!=null){
											foreach(string item1  in tabProductException){
												if(item1==currentRow[2].ToString()){
													checkBox="";
													break;
												}
											}
										}
									}

									if(sectorBool){
										if(tabSubSectorException!=null){
											foreach(string item1  in tabSubSectorException){
												if(item1==currentRow[2].ToString()){
													checkBox="";
													break;
												}
											}
										}
									}

									if(SubSectorBool){
										if(tabGroupException!=null){
											foreach(string item1  in tabGroupException){
												if(item1==currentRow[2].ToString()){
													checkBox="";
													break;
												}
											}
										}
									}

									if (groupBool) {
										if (tabSegmentException != null) {
											foreach (string item1 in tabSegmentException) {
												if (item1 == currentRow[2].ToString()) {
													checkBox = "";
													break;
												}
											}
										}
									}

								}
								#endregion

					
								#region Affichage des fils
								if(nbLinesTest==2){								

									t.Append("<td class=\"txtViolet10\" width=215>");
                                    t.Append("<input type=\"checkbox\" " + checkBox + " id=\"AdvertiserSelectionWebControl1_" + i + "\" value=" + idParent + " name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
									t.Append("</td>");
									nbLinesTest=1;
									i++;
					
								}
								else if(nbLinesTest==1 ){
									t.Append("<td class=\"txtViolet10\" width=215>");
                                    t.Append("<input type=\"checkbox\" " + checkBox + " id=\"AdvertiserSelectionWebControl1_" + i + "\"  value=" + idParent + "  name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
									t.Append("</td>");								
									t.Append("</tr>");
									nbLinesTest=0;
									i++;	
					
								}
								else {
									t.Append("<tr>");
									t.Append("<td class=\"txtViolet10\" width=215>");
                                    t.Append("<input type=\"checkbox\" " + checkBox + " id=\"AdvertiserSelectionWebControl1_" + i + "\" value=" + idParent + " name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
									t.Append("</td>");								
									nbLinesTest=2;
									i++;
								}
								#endregion

								#endregion
							}
					
				
							if(dsListAdvertiser.Tables[0].Rows.Count!=0){
								if(nbLinesTest!=0){
									t.Append("</tr>");
									nbLinesTest=0;
								}
								else{
									nbLinesTest=0;
								}
								t.Append("</table></div>");
							}
						}
					
					}
					#endregion				

					#region 1 niveaux

					//if(productBool || groupBool){
					if (productBool || segmentBool) {
						if(dsListAdvertiser!=null){
							if(dsListAdvertiser.Tables[0].Rows.Count!=0){
                                t.Append("<table style=\"border-bottom :#644883 1px solid; border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width=\"100%\">");
								t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" onclick=\"allSelectionRef()\" style=\"TEXT-DECORATION: none\"  ID=\"tab1\">");
								if(productBool){
									t.Append(GestionWeb.GetWebWord(817,webSession.SiteLanguage));}
								else{
									//t.Append(GestionWeb.GetWebWord(969,webSession.SiteLanguage));
										t.Append(GestionWeb.GetWebWord(1067, webSession.SiteLanguage));
								}
								t.Append("</a></td></tr>");	
						
								foreach(DataRow currentRow in dsListAdvertiser.Tables["dsListAdvertiser"].Rows){
						
									#region foreach

									checkBox="";
									if(productBool){
										if(tabProductAccess!=null){
											foreach(string item1  in tabProductAccess){
												if(item1==currentRow[0].ToString()){
													checkBox="checked";
													break;
												}
											}
										}
									}

									//if(groupBool){
									//    if(tabGroupAccess!=null){
									//        foreach(string item1  in tabGroupAccess){
									//            if(item1==currentRow[0].ToString()){
									//                checkBox="checked";
									//                break;
									//            }
									//        }
									//    }
									//}
									if (segmentBool) {
										if (tabSegmentAccess != null) {
											foreach (string item1 in tabSegmentAccess) {
												if (item1 == currentRow[0].ToString()) {
													checkBox = "checked";
													break;
												}
											}
										}
									}

									#region Affichage des fils
									if(nbLinesTest==2){
										t.Append("<td class=\"txtViolet10\" width=215>");
										t.Append("<input type=\"checkbox\" "+checkBox+" id=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[1].ToString()+"<br></label>");
										t.Append("</td>");
										nbLinesTest=1;
										i++;
					
									}
									else if(nbLinesTest==1 ){
										t.Append("<td class=\"txtViolet10\" width=215>");								
										t.Append("<input type=\"checkbox\" "+checkBox+" id=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\"  name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[1].ToString()+"<br></label>");
										t.Append("</td>");								
										t.Append("</tr>");
										nbLinesTest=0;
										i++;	
					
									}
									else {
										t.Append("<tr>");
										t.Append("<td class=\"txtViolet10\" width=215>");
                                        t.Append("<input type=\"checkbox\" " + checkBox + " id=\"AdvertiserSelectionWebControl1_" + i + "\" value=\"child\" name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[1].ToString() + "<br></label>");
										t.Append("</td>");								
										nbLinesTest=2;
										i++;
									}
									#endregion

									#endregion

								}
						
					
								if(nbLinesTest!=0){
									t.Append("</tr>");
									nbLinesTest=0;
								}
								else{
									nbLinesTest=0;
								}
								t.Append("</table>");
							}
						}
					}
					#endregion

				
					if(advertiserText.Length<2 && buttonTarget==2){
						// Saississez 2 caractères au minimum
						t.Append("<tr><td bgcolor=\"#ffffff\" class=\"txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(" "+GestionWeb.GetWebWord(1473,webSession.SiteLanguage)+" "+advertiserText+".</p> ");
						t.Append(" </td> ");
						t.Append(" </tr> ");
					}	

					else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(advertiserText) && buttonTarget==2){
						t.Append("<tr><td bgcolor=\"#ffffff\" class=\"txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(" "+GestionWeb.GetWebWord(820,webSession.SiteLanguage)+" "+advertiserText+".</p> ");
						t.Append(" </td> ");
						t.Append(" </tr> ");
					}	
					else if(dsListAdvertiser!=null){
						if(dsListAdvertiser.Tables[0].Rows.Count==0){
							t.Append("<tr><td bgcolor=\"#ffffff\" class=\"txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
							t.Append(" "+GestionWeb.GetWebWord(819,webSession.SiteLanguage)+" "+advertiserText+".</p> ");
							t.Append(" </td> ");
							t.Append(" </tr> ");
						}
					}else if(ErrorMessage.Length>0){
						t.Append("<tr><td bgcolor=\"#ffffff\" class=\"txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(ErrorMessage);
						t.Append("</td></tr>");
					}			
					//Fin du tableau global
					t.Append("</td></tr></table></td></tr>");				
					t.Append("<tr><td bgColor=\"#ffffff\"><IMG height=\"20\" src=\"images/pixel.gif\" width=\"1\"></td></tr>");
					t.Append("<tr><td></td></tr>");					
					output.Write(t.ToString());				
				
					#endregion		
				}
			}
			
			#region Affichache des Annonceurs à partir d'un arbre
			//if(buttonTarget==4  || buttonTarget==7 || buttonTarget==6 || buttonTarget==8 || buttonTarget==9){
			if(buttonTarget==4  || buttonTarget==7 || buttonTarget==3 || buttonTarget==8 || buttonTarget==9){
				//if(buttonTarget==4 || buttonTarget==6){
				if(buttonTarget==4 || buttonTarget==3){
					t.Append("<tr class=\"txtGris11Bold\">");												
					t.Append("<td bgcolor=#FFFFFF  >");
					t.Append(""+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
				}
				
				t.Append("<tr height=5px bgcolor=\"#ffffff\"><td></td></tr>");				
				t.Append("<tr><td bgColor=\"#ffffff\" vAlign=\"top\">");
				t.Append("<table bgcolor=\"#ffffff\">");				
				t.Append("<tr> ");
				// liste des Annonceurs
				if(holdingCompanyBool){					
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(814,webSession.SiteLanguage)+"");}
				else if(advertiserBool){
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(813,webSession.SiteLanguage)+"");
				}
				else if(brandBool)
				{
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(1585,webSession.SiteLanguage)+"");
				}
				else if(productBool){
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"");
				}
				else if(SectorBool){
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"");
				}
				else if(subSectorBool){
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(966,webSession.SiteLanguage)+"");
				}
				else if(groupBool){
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">"+GestionWeb.GetWebWord(967,webSession.SiteLanguage)+"");
				}
				else if (segmentBool) {
					t.Append("<td class=\"txtViolet11\" bgColor=\"#ffffff\" height=\"13\">" + GestionWeb.GetWebWord(1894, webSession.SiteLanguage) + "");
				}
				t.Append("</td></tr><tr><td>");	
				// Affichage des annonceur à partir de SelectionUniversAdvertiser où CurrentUniversAdvertiser
				//if(buttonTarget==4 || buttonTarget==6 || buttonTarget==8){
				if(buttonTarget==4 || buttonTarget==3 || buttonTarget==8){
                    t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSession.SelectionUniversAdvertiser, true, true, true, 100, true, true, webSession.SiteLanguage, 3, 1, true, webSession.DataLanguage, webSession.CustomerDataFilters.DataSource));
				}
				else{
                    t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSession.CurrentUniversAdvertiser, true, true, true, 100, true, true, webSession.SiteLanguage, 3, 1, true, webSession.DataLanguage, webSession.CustomerDataFilters.DataSource));
				}
				t.Append("</td></tr></table></td></tr>");				
				output.Write(t.ToString());	
			
			}
			#endregion

		}
	#endregion
		
		#region création de l'arbre
		/// <summary>
		/// Création de l'arbre à partir de la liste des Items
		/// </summary>
		/// <returns></returns>
        protected System.Windows.Forms.TreeNode createTreeNode()
        {
			System.Windows.Forms.TreeNode advertiserTree=new System.Windows.Forms.TreeNode();
			
			System.Windows.Forms.TreeNode tmpNode=null;
			System.Windows.Forms.TreeNode tmpParentNode=null;
			// Cas différent du bouton valider de la sous sélection
			if(buttonTarget!=7){
				advertiserTree= ((System.Windows.Forms.TreeNode)webSession.SelectionUniversAdvertiser);
			}

			else{
				advertiserTree= ((System.Windows.Forms.TreeNode)webSession.CurrentUniversAdvertiser);
			}
			int compteurParent=0;
			int compteurChild=0;
			if(buttonTarget!=7){
				if(webSession.SelectionUniversAdvertiser.FirstNode!=null){
					webSession.SelectionUniversAdvertiser.Nodes.Clear();
				}
			}
			else{
				if(webSession.CurrentUniversAdvertiser.FirstNode!=null){
					webSession.CurrentUniversAdvertiser.Nodes.Clear();
				}
			}
			if(holdingCompanyBool || advertiserBool || sectorBool || subSectorBool || brandBool || groupBool)
			{
				#region 2 niveaux
				int integration=0;
				int nbrChild=1;
				foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items)
				{
					#region foreach
				
					string[] tabParent=currentItem.Text.Split('_') ;
				
					// cas Intégration automatique
					if(tabParent[0]=="Parent" && currentItem.Selected)
					{
					
						if(nbrChild==0)
						{
							advertiserTree.LastNode.Remove();
						}
						nbrChild=1;
						tmpParentNode=new System.Windows.Forms.TreeNode(tabParent[1]);
						if(holdingCompanyBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,Int64.Parse(currentItem.Value),tabParent[1]);}									
						else if(advertiserBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(brandBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(sectorBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(subSectorBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if (groupBool) {
								tmpParentNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, Int64.Parse(currentItem.Value), tabParent[1]);
						}
					
					
						tmpParentNode.Checked=true;
						integration=1;
						advertiserTree.Nodes.Add(tmpParentNode);
						compteurParent++;

						
					}
					// Cas Non intégration automatique
					if(tabParent[0]=="Parent" &&  !currentItem.Selected)
					{

						if(nbrChild==0)
						{
							advertiserTree.LastNode.Remove();
						}
						nbrChild=0;
						tmpParentNode=new System.Windows.Forms.TreeNode(tabParent[1]);
						if(holdingCompanyBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,Int64.Parse(currentItem.Value),tabParent[1]);}									
						else if(advertiserBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(brandBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(sectorBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(subSectorBool)
						{
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,Int64.Parse(currentItem.Value),tabParent[1]);
						}
						else if (groupBool) {
								tmpParentNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupException, Int64.Parse(currentItem.Value), tabParent[1]);
							}
						tmpParentNode.Checked=false;
						integration=2;
					
						advertiserTree.Nodes.Add(tmpParentNode);
						//compteurParent++;
					}
			
					if(tabParent[0]=="Children" && integration==1 && !currentItem.Selected)
					{
						tmpNode=new System.Windows.Forms.TreeNode(tabParent[1]);
						if(holdingCompanyBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,Int64.Parse(currentItem.Value),tabParent[1]);}									
						else if(advertiserBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(brandBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(sectorBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(subSectorBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupException,Int64.Parse(currentItem.Value),tabParent[1]);}
							else if (groupBool) {
								tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentException, Int64.Parse(currentItem.Value), tabParent[1]);
							}
						tmpNode.Checked=false;
						nbrChild++;
						tmpParentNode.Nodes.Add(tmpNode);
						compteurChild++;
				
					}

					if(tabParent[0]=="Children" &&  integration ==2 && currentItem.Selected)
					{
						tmpNode=new System.Windows.Forms.TreeNode(tabParent[1]);
						if(holdingCompanyBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,Int64.Parse(currentItem.Value),tabParent[1]);}									
						else if(advertiserBool || brandBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(sectorBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
						else if(subSectorBool)
						{
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,Int64.Parse(currentItem.Value),tabParent[1]);
						}
						else if (groupBool) {
							tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, Int64.Parse(currentItem.Value), tabParent[1]);
						}
						tmpNode.Checked=true;
						nbrChild++;
						tmpParentNode.Nodes.Add(tmpNode);
						compteurChild++;
					}

					#endregion
				}

				if(advertiserTree.LastNode!=null)
				{	
					if(advertiserTree.LastNode.FirstNode==null && integration!=1)
					{
						advertiserTree.LastNode.Remove();
					}
				}
				#endregion
			}
			else if (productBool || segmentBool)//else if(productBool || groupBool)
			{

				#region 1 niveau
				foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items)
				{					

					string[] tabParent=currentItem.Text.Split('_') ;
					if(tabParent[0]=="Children" && currentItem.Selected)
					{					
						tmpParentNode=new System.Windows.Forms.TreeNode(tabParent[1]);
						if(productBool)
						{			
							tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,Int64.Parse(currentItem.Value),tabParent[1]);}
							else if (segmentBool)//else if(groupBool)
						{
								//tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,Int64.Parse(currentItem.Value),tabParent[1]);
								tmpParentNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, Int64.Parse(currentItem.Value), tabParent[1]);
							}
						tmpParentNode.Checked=true;					
						advertiserTree.Nodes.Add(tmpParentNode);
						compteurChild++;
					}
				}
				#endregion

			}
			
			// Fournit une valeur à nbElement pour vérifier la validité du nombre 
			// d'éléments sélectionné
			if(compteurChild > TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER || compteurParent>TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER){
				nbElement=2;
			}
			else if (compteurChild==0 && compteurParent==0){
				nbElement=3;  
			}

			return advertiserTree;

		}
		#endregion

	}
}
