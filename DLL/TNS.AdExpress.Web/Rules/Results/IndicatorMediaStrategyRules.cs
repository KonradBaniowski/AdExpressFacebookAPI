#region Informations
// Auteur: D. Mussuma 
// Date de cr�ation : 2/11/2004 
// Date de modification : 29/11/2004 
#endregion

#region Namespace
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.Date;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Translation;
using DateFunctions = TNS.FrameWork.Date;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Constantes.Web;


#endregion

namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Classe m�tier ("Rules") pour l'indicateur Strat�gie M�dia
	/// </summary>
	public class IndicatorMediaStrategyRules{		
		
		#region r�sultats de r�partition m�dia (pour sortie sous forme tableau)
		/// <summary>
		/// La r�partition m�dia sur le total de la p�riode dans un tableau
		/// contenant les �l�ments ci-apr�s :
		/// en ligne :
		/// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
		/// total march� (en option)
		/// -les �l�ments de r�f�rences
		/// -les �l�ments concurrents 
		/// en colonne :
		/// -Les investissements de la p�riode N
		/// -une PDM (part de march� ) exprimant la r�partition media en %
		/// -une �volution N vs N-1 en % (uniquement dans le cas d'une �tude comparative)
		/// -le 1er annonceur en Keuros uniquement  pour les lignes total produits �ventuels
		/// -la 1ere r�f�rence en Keuros uniquement  pour les lignes total produits �ventuels
		/// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
		/// -si plusieurs media, le tableau sera d�clin� par media
		/// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
		/// </summary>
		/// <param name="webSession">session du client</param>
		///<param name="comparisonCriterion">crit�re de comparaison</param>		
		/// <returns>tableau de r�sultats</returns>
		public static object[,] GetFormattedTable(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion ){
			
			#region variables
			//donn�es 1ere �l�ment plurim�dia
			#region variables tempon
			//	string tempInvest="";
			string tempEvol="";
			string tempPDM="";
			#endregion

			#region variables pour calculer les PDM (part de march�)
			//liste des investissments total pour l'ensemble de l'�tude
			Hashtable hPdmTotAdvert = new Hashtable();
			//liste des investissments par m�dia (vehicle)
			Hashtable hPdmVehAdvert = new Hashtable();
			//liste des investissments par cat�gorie
			Hashtable hPdmCatAdvert = new Hashtable();
//			//PDM d'un media (vehicle)
//			double PdmVehicle = (double)0.0;
//			//PDM d'un support
//			double PdmMedia  = (double)0.0;
//			//PDM d'une cat�gorie
//			double	PdmCategory = (double)0.0;
			#endregion	
			
			#region variables pour les dimensions du tableau de r�sultats
			// tableau de resultats
			object[,] tab = null;
			//nombre maximal de lignes
			//int nbMaxLines = 0;
			//Index d'une ligne du tableau
			int indexTabRow=0;
			//bool�en incr�mentation d'une ligne
			bool increment =false;
			#endregion
			
			#region Variables annonceurs de r�f�rences ou concurrents
			//identifiant  annonceurs de r�f�rences ou concurrents
			string idAdvertiser="";
			//nombre  annonceurs de r�f�rences ou concurrents
			//int nbAdvertiser=0;
			//Y a t'il des annonceurs de r�f�rences ou concurrents
			bool hasAdvertiser=false;
			//Investissement d'un annonceur sur la p�riode N
			//string AdvertiserInvest="";
			//Investissement d'un annonceur sur la p�riode N-1
			//string AdvertiserInvest_N1="";
			//Investissement total des annonceurs sur la p�riode N
			string AdvertiserTotalInvest="";
			//Investissement total des annonceurs sur la p�riode N-1
			string AdvertiserTotalInvest_N1 = "";
			//Investissement d'un annonceur par cat�gorie sur la p�riode N
			string AdvertiserInvestByCat="";
			//Investissement d'un annonceur par cat�gorie sur la p�riode N-1
			string AdvertiserInvestByCat_N1="";
			//investissement pour une cat�gorie sur p�riode N
			//double CatInvest=(double)0.0;
			//investissement pour une cat�gorie sur p�riode N-1
			//double CatInvest_N1=(double)0.0;
			//Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N
			string AdvertiserInvestByVeh="";
			//Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N-1
			string AdvertiserInvestByVeh_N1="";						
			//Evolution N/N-1 pour total annonceurs
			//double AdvertiserTotalEvolution = (double)0.0;
			//Evolution N/N-1 pour un annonceur
			//double AdvertiserEvolution = (double)0.0;
			//collection des annonceurs d�j� trait�s
			ArrayList OldIdAdvertiserArr = new ArrayList();
			//collection des annonceurs d�j� trait�s pour le total univers
			ArrayList inTotUnivAdvertAlreadyUsedArr= new ArrayList();	
			//collection des annonceurs d�j� trait�s pour un m�dia (vehicle)
			ArrayList inAdvertVehicleAlreadyUsedArr= new ArrayList();
			//collection des annonceurs d�j� trait�s pour une cxat�gorie
			ArrayList inAdvertCategoryAlreadyUsedArr = new ArrayList();
			//collection des annonceurs d�j� trait�s pour un support (media)
			ArrayList inAdvertMediaAlreadyUsedArr = new ArrayList();
			//			string OldIdAdvertiser="";				
			//			string OldIdCatForAdvert="0";
			//			string containt="";
			#endregion

			#region Variables des supports
			//depart de la boucle 
			int start=0;
			//identifiant d'un m�dia (vehicle)
			string idVehicle="";
			//identifiant m�dia pr�c�dent
			string OldIdVehicle="0";
			//lib�ll� d'un m�dia
			string Vehicle="";
			//nombre de m�dia (vehicle)
			//int nbVehicle=0;
			//identifiant d'une cat�gorie
			string idCategory="";
			//identifiant pr�c�dent d'une cat�gorie
			string OldIdCategory="0";
			//collection identifiant pr�c�dentes cat�gories
			ArrayList OldIdCategoryArr = new ArrayList();
			//lib�ll� d'une cat�gorie
			string Category="";
			//nombre de cat�gories
			//int nbCategory=0;
			//identifiant d'un support
			string idMedia="";
			//lib�ll� d'un support
			string Media="";
			//identifiant du pr�c�dent support
			string OldIdMedia="0";
			//collection identifiant pr�c�dents supports
			ArrayList OldIdMediaArr = new ArrayList();
			//nombre de supports				
			//double TotalUnivVehicleEvolution =(double)0.0;						
			#endregion

			#region construction des listes de produits, media, annonceurs s�lectionn�s				
			DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
			string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
			string CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
			string CategoryAccessList = recapUniversSelection.CategoryAccessList;
			string MediaAccessList = recapUniversSelection.MediaAccessList;
			string VehicleAccessList = recapUniversSelection.VehicleAccessList;
			#endregion

			#region variable pour total univers
			//Y a t'il des donn�es pour calculer les valeurs du total univers
			bool hasTotalUniv =false;	
			//Collecton des m�dia d�j� trait�
			ArrayList inTotUnivVehicleAlreadyUsedArr= new ArrayList();
			//Collecton des cat�gories d�j� trait�
			ArrayList inTotUnivCategoryAlreadyUsedArr = new ArrayList();
			//Collecton des supports d�j� trait�
			ArrayList inTotUnivMediaAlreadyUsedArr = new ArrayList();
			//investissement premier annonceur	
			//	string  maxFirstAdvertInvest = "";
			//investissement premiere r�f�rence	
			//	string  maxFirstProdInvest = "";
			//investissment total univers par support p�riode N
			string TotalUnivMediaInvest="";
			//investissment total univers par support p�riode N-1
			string TotalUnivMediaInvest_N1="";
			//investissment total univers par m�dia p�riode N
			string TotalUnivVehicleInvest="";
			//investissment total univers par m�dia p�riode N-1
			string TotalUnivVehicleInvest_N1="";
			//investissment total univers par cat�gorie p�riode N
			string TotalUnivCategoryInvest="";
			//investissment total univers par cat�gorie p�riode N
			string TotalUnivCategoryInvest_N1="";
			//investissment total univers p�riode N
			string TotalUnivInvest="";
			//investissment total univers p�riode N-1
			string TotalUnivInvest_N1="";
			//Evolution N/N-1 total univers
			//double TotalUnivEvolution = (double)0.0;
			//Evolution N/N-1 par support
			//double TotalUnivMediaEvolution = (double)0.0;	
			//Evolution N/N-1 par cat�gorie
			//double TotalUnivCategoryEvolution = (double)0.0;
			//groupe de donn�es pour univers
			DataSet dsTotalUniverse = null;
			//table de donn�es pour univers
			DataTable dtTotalUniverse = null;
			#region ancienne version
			//groupe de donn�es pour premiere r�f�rence univers
			//DataSet dsTotalUniverseFirstProduct = null;
			//table de donn�es pour premiere r�f�rence univers
			//DataTable dtTotalUniverseFirstProduct = null;
			//groupe de donn�es pour premier annonceur univers
			//DataSet dsTotalUniverseFirstAvertiser = null;	
			//table de donn�e pour premier annonceur univers
			//DataTable dtTotalUniverseFirstAvertiser = null;	
			#endregion
			#region variables 1er �l�ment par m�dia pour univers
			//table de donn�es pour 1ere r�f�rence par m�dia (vehicle) pour total univers
			DataTable dtUniverse1stProductByVeh=null;
			//table de donn�es pour 1er annonceur par m�dia (vehicle) pour total univers
			DataTable dtUniverse1stAdvertiserByVeh=null;
			//table de donn�es pour 1ere r�f�rence par cat�gorie pour total univers
			DataTable dtUniverse1stProductByCat=null;
			//table de donn�es pour 1er annonceur par cat�gorie pour total univers
			DataTable dtUniverse1stAdvertiserByCat=null;
			//table de donn�es pour 1ere r�f�rence par support pour total univers
			DataTable dtUniverse1stProductByMed=null;
			//table de donn�es pour 1er annonceur par support pour total univers
			DataTable dtUniverse1stAdvertiserByMed=null;
			#endregion
			#endregion

			#region variable pour total march� ou famille	
			// y a t'il des donn�es pour le total march�
			bool hasTotalMarketOrSector=false;
			//investissemnt total march� par support p�riode N
			string TotalMarketOrSectorMediaInvest="";
			//investissemnt total march� par support p�riode N-1
			string TotalMarketOrSectorMediaInvest_N1="";
			//investissemnt total cat�gorie par cat�gorie p�riode N
			string TotalMarketOrSectorCategoryInvest="";
			//investissemnt total march� par cat�gorie p�riode N-1
			string TotalMarketOrSectorCategoryInvest_N1="";
			//investissemnt total march� par m�dia p�riode N
			string TotalMarketOrSectorVehicleInvest="";
			//investissemnt total march� par m�dia p�riode N-1
			string TotalMarketOrSectorVehicleInvest_N1="";	
			//investissemnt total march�  p�riode N
			string TotalMarketOrSectorInvest="";
			//investissemnt total march�  p�riode N-1
			string TotalMarketOrSectorInvest_N1="";
			//identifiant plurim�dia
			ArrayList pluriArr = new ArrayList();
			//identifiant des m�dias d�j� trait�s
			ArrayList inTotMarketOrSectorVehicleAlreadyUsedArr = new ArrayList();
			//identifiant des cat�gories d�j� trait�s
			ArrayList inTotMarketOrSectorCategoryAlreadyUsedArr =new ArrayList();
			//identifiant des supports d�j� trait�s
			ArrayList inTotMarketOrSectorMediaAlreadyUsedArr =new ArrayList();
			//Evolution total march� par support
			//double TotalMarketOrSectorMediaEvolution = (double)0.0;
			//Evolution total march� par m�dia
			//double TotalMarketOrSectorVehicleEvolution = (double)0.0;
			//Evolution total march� par cat�gorie
			//double TotalMarketOrSectorCategoryEvolution = (double)0.0;
			//Evolution total march�
			//double TotalMarketOrSectorEvolution = (double)0.0;
			//Groupe de donn�es pour total march�
			DataSet dsTotalMarketOrSector = null;
			//table de donn�es pour total march�
			DataTable dtTotalMarketOrSector = null;
			#region ancienne version
			//Groupe de donn�es premiere r�f�rence total march�
			//DataSet dsTotalMarketOrSectorFirstProduct = null;
			//table de donn�es premiere r�f�rence total march�
			//DataTable dtTotalMarketOrSectorFirstProduct = null;
			//Goupe de donn�es premier annonceur total march�
			//DataSet dsTotalMarketOrSectorFirstAdvertiser = null;
			//table de donn�es premier annonceur total march�
			//DataTable dtTotalMarketOrSectorFirstAdvertiser = null;
			#endregion
			#region variables 1er �l�ment par m�dia pour march� ou famille
			//table de donn�es pour 1ere r�f�rence par m�dia (vehicle) pour total march� ou famille
			DataTable dtMarketOrSector1stProductByVeh=null;
			//table de donn�es pour 1er annonceur par m�dia (vehicle) pour  total march� ou famille
			DataTable dtMarketOrSector1stAdvertiserByVeh=null;
			//table de donn�es pour 1ere r�f�rence par cat�gorie pour  total march� ou famille
			DataTable dtMarketOrSector1stProductByCat=null;
			//table de donn�es pour 1er annonceur par cat�gorie pour  total march� ou famille
			DataTable dtMarketOrSector1stAdvertiserByCat=null;
			//table de donn�es pour 1ere r�f�rence par support pour  total march� ou famille
			DataTable dtMarketOrSector1stProductByMed=null;
			//table de donn�es pour 1er annonceur par support pour  total march� ou famille
			DataTable dtMarketOrSector1stAdvertiserByMed=null;
			#endregion
			#endregion

			#region variable pour annonceur de r�f�rences ou concurrent
			//chaine pour colonne � r�cuperer dans collection de DataRow annonceurs
			string strExpr = "";
			//filtre pour colonne � r�cuperer dans collection de DataRow annonceurs
			string strSort = "";
			//DataRow annonceurs
			DataRow[] foundRows=null;
			//Groupe de donn�es annonceurs de r�f�rence ou concurrents
			DataSet dsAdvertiser = null;
			//Table de donn�es annonceurs de r�f�rences ou concurrents
			DataTable dtAdvertiser = null;
			#endregion

			#endregion					
			try{
				#region Chargement des donn�es

				#region chargement des donn�es pour les annonceurs de r�f�rences et/ou concurrents
				/* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
				 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
				 * pour chaque annonceur et par niveau support
				 */
				if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
					dsAdvertiser = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,true);
					if(dsAdvertiser!=null && dsAdvertiser.Tables[0]!=null && dsAdvertiser.Tables[0].Rows.Count>0){
						dtAdvertiser = dsAdvertiser.Tables[0];
					}
				}
				#endregion

				#region chargement des donn�es totaux univers
				/*Chargement des donn�es pour les totaux univers
					* dsTotalUniverse : permet de r�cuperer pour chaque niveau support
					* l'investissement,l'evolution,le PDM
					*/				
				dsTotalUniverse = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,false);
				if( dsTotalUniverse!=null && dsTotalUniverse.Tables[0]!=null && dsTotalUniverse.Tables[0].Rows.Count>0){
					dtTotalUniverse = dsTotalUniverse.Tables[0];
				}
				/*Chargement des donn�es pour les totaux univers
					* dsTotalUniverseFirstProduct : permet de r�cuperer pour chaque niveau support
					* l'investissement et le libell� de la premi�re r�f�rence
					*/					
				FrameWorkResultConstantes.MediaStrategy.MediaLevel mediaLevel=FrameWorkResultConstantes.MediaStrategy.MediaLevel.vehicleLevel;					
				mediaLevel =  SwitchMedia(webSession);						
				Get1stElmtDataTbleByMedia(webSession,ref dtUniverse1stProductByVeh,ref dtUniverse1stAdvertiserByVeh,ref dtUniverse1stProductByCat,ref dtUniverse1stAdvertiserByCat,ref dtUniverse1stProductByMed, ref dtUniverse1stAdvertiserByMed,CustomerSessions.ComparisonCriterion.universTotal,mediaLevel);
				#endregion

				#region chargement des donn�es totaux march�s ou famille(s)
				/* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
				 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
				 * pour chaque annonceur et par niveau support
				 */			
				if (comparisonCriterion == CustomerSessions.ComparisonCriterion.sectorTotal)
					dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.sectorTotal,false);
				else if (comparisonCriterion == CustomerSessions.ComparisonCriterion.marketTotal)
					dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.marketTotal,false);
				if(dsTotalMarketOrSector!=null && dsTotalMarketOrSector.Tables[0]!=null &&  dsTotalMarketOrSector.Tables[0].Rows.Count>0){
					dtTotalMarketOrSector =  dsTotalMarketOrSector.Tables[0];
				}
				/*Chargement des donn�es pour les totaux univers
					* dsTotalMarketOrSectorFirstProduct : permet de r�cuperer pour chaque niveau support
					* l'investissement et le lib�ll� de la premi�re r�f�rence
					*/				
				Get1stElmtDataTbleByMedia(webSession,ref dtMarketOrSector1stProductByVeh,ref dtMarketOrSector1stAdvertiserByVeh,ref dtMarketOrSector1stProductByCat,ref dtMarketOrSector1stAdvertiserByCat,ref dtMarketOrSector1stProductByMed, ref dtMarketOrSector1stAdvertiserByMed,comparisonCriterion,mediaLevel);		
				#endregion
			
				#endregion
		
				#region  Construction du tableau de r�sultats	
	
				#region Instanciation du tableau de r�sultats
				/*On instancie le tableau de r�sultats pour strat�gie m�dia
				 */		
				//cr�ation du tableau 				
				tab = TabInstance(webSession,dtTotalMarketOrSector,dtAdvertiser,VehicleAccessList,ConstResults.MediaStrategy.NB_MAX_COLUMNS);
				// Il n'y a pas de donn�es
				if(tab==null)return(new object[0,0]);	
				#endregion

				#region Remplissage chaque ligne m�dia dans table
				/* Chaque ligne du tableau contient toutes les donn�es d'un annonceur de r�f�rence ou concurrent
				 * (Investissement,Evolution,PDM,lib�ll�) ou d'un total support (media ou cat�gorie ou support)
				 * avec ses param�tres (Investissement,Evolution,PDM,lib�ll�, 1er annonceur et son investissement, idem pour 1er r�f�rence)
				*/
				if(dtTotalMarketOrSector!=null && dtTotalMarketOrSector.Rows.Count>0){	
					/*Si utilisateur a s�lectionn� PLURIMEDIA on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
					if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList) && DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList)){
						#region ligne total march� ou famille pour PLURIMEDIA
						//Traitement ligne total march� ou famille pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
						if(dtTotalMarketOrSector.Columns.Contains("total_N") && !hasTotalMarketOrSector){						
							ComputeInvestPdmEvol(dtTotalMarketOrSector,ref TotalMarketOrSectorInvest ,ref TotalMarketOrSectorInvest,ref TotalMarketOrSectorInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","",webSession.ComparativeStudy);
							// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� et plurim�dia 
							if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorInvest.Trim())){
								tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,VehicleAccessList,GestionWeb.GetWebWord(210,webSession.SiteLanguage),true,TotalMarketOrSectorInvest,"100",tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
								increment=true;
							}							
							tab=FillTabFisrtElmt(webSession,tab,comparisonCriterion,TotalMarketOrSectorInvest,ref indexTabRow,increment);
							increment=false;
							hasTotalMarketOrSector=true;						
						}
						#endregion

						#region ligne univers pour PLURIMEDIA
						//Traitement ligne total univers pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
						if(dtTotalUniverse.Columns.Contains("total_N")&& !hasTotalUniv)	{							
							ComputeInvestPdmEvol(dtTotalUniverse,ref TotalUnivInvest,ref TotalUnivInvest,ref TotalUnivInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","",webSession.ComparativeStudy);
							// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et plurim�dia 
							if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivInvest.Trim())){
								tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,VehicleAccessList,GestionWeb.GetWebWord(210,webSession.SiteLanguage),true,TotalUnivInvest,"100",tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
								increment=true;
							}						
							tab=FillTabFisrtElmt(webSession,tab,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,TotalUnivInvest,ref indexTabRow,increment);
							increment=false;//ligne suivante
							hasTotalUniv=true;
						}
						#endregion
				
						#region lignes  annonceurs de r�f�rences ou concurrents	pour s�lection PLURIMEDIA
						//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par PLURIMEDIA : Investissment,Evolution,PDM
						if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList) && dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && !hasAdvertiser){
							//Pour chaque ligne  TOTAL annonceur de r�f�rence ou concurrent on r�cup�re les donn�es							
							FillAdvertisers(webSession,tab,dtAdvertiser,dtTotalMarketOrSector,"Sum(total_N)","Sum(total_N1)","",inTotUnivAdvertAlreadyUsedArr,ref hPdmTotAdvert,ref hPdmTotAdvert,ref AdvertiserTotalInvest,ref AdvertiserTotalInvest_N1,ref indexTabRow,VehicleAccessList,idVehicle,Vehicle,true, ref hasAdvertiser);
						}
						#endregion
					}			
					//Pour chaque ligne  media du total univers on r�cup�re les donn�es										
					foreach(DataRow currentRow in dtTotalMarketOrSector.Rows){
														
						#region lignes univers et march� ou famille par m�dia (vehicle)
						/*Si utilisateur a s�lectionn� un MEDIA (vehicle) on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
						#region ligne total march� ou famille par m�dia (vehicle)
						//Colonne m�dia (vehicle)
						if(dtTotalMarketOrSector.Columns.Contains("id_vehicle") && dtTotalMarketOrSector.Columns.Contains("vehicle")){																			
							idVehicle = currentRow["id_vehicle"].ToString();
							Vehicle = currentRow["vehicle"].ToString();						
							if(!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle)){		
								if(dtTotalMarketOrSector.Columns.Contains("total_N")){
									//investissement total univers pour m�dia (vehicle)									
									if(DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList))
									ComputeInvestPdmEvol(dtTotalMarketOrSector,ref TotalMarketOrSectorInvest ,ref TotalMarketOrSectorVehicleInvest,ref TotalMarketOrSectorVehicleInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle = "+idVehicle+"",webSession.ComparativeStudy);
									else ComputeInvestPdmEvol(dtTotalMarketOrSector,ref TotalMarketOrSectorVehicleInvest ,ref TotalMarketOrSectorVehicleInvest,ref TotalMarketOrSectorVehicleInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle = "+idVehicle+"",webSession.ComparativeStudy);
									// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 
									if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorVehicleInvest.Trim())){
										tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,false,TotalMarketOrSectorVehicleInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
										increment=true;	
									}
								}									
								tab = FillTabFisrtElmt(tab,ref TotalMarketOrSectorVehicleInvest,dtMarketOrSector1stAdvertiserByVeh,dtMarketOrSector1stProductByVeh,"id_vehicle",idVehicle,ref inTotMarketOrSectorVehicleAlreadyUsedArr,ref indexTabRow,increment,false);
								increment=false;
							}
						}										
						#endregion

						#region ligne total univers par m�dia (vehicle)				
						if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_vehicle") && dtTotalUniverse.Columns.Contains("vehicle")){																
							idVehicle = currentRow["id_vehicle"].ToString();
							Vehicle = currentRow["vehicle"].ToString();		
							if(!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle)){
								if(dtTotalUniverse.Columns.Contains("total_N"))	{
									//investissement total univers pour m�dia (vehicle)									
									if(DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList))
									ComputeInvestPdmEvol(dtTotalUniverse,ref TotalUnivInvest,ref TotalUnivVehicleInvest,ref TotalUnivVehicleInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle = "+idVehicle+"",webSession.ComparativeStudy);
									else ComputeInvestPdmEvol(dtTotalUniverse,ref TotalUnivVehicleInvest,ref TotalUnivVehicleInvest,ref TotalUnivVehicleInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle = "+idVehicle+"",webSession.ComparativeStudy);
									// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 
									if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivVehicleInvest.Trim())){
										tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,false,TotalUnivVehicleInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
										increment=true;
									}
								}								
								tab = FillTabFisrtElmt(tab,ref TotalUnivVehicleInvest,dtUniverse1stAdvertiserByVeh,dtUniverse1stProductByVeh,"id_vehicle",idVehicle,ref inTotUnivVehicleAlreadyUsedArr,ref indexTabRow,increment,false);
								increment=false;						
													
								#endregion

								if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){											
									#region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent Niveau m�dia (vehicle)
									//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par MEDIA (vehicle) : Investissment,Evolution,PDM
									if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_advertiser")){
										//Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
										if((int.Parse(idVehicle)!=int.Parse(OldIdVehicle))){ 										
											inAdvertVehicleAlreadyUsedArr = null;
											inAdvertVehicleAlreadyUsedArr = new ArrayList();
											hPdmVehAdvert =null;
											hPdmVehAdvert = new Hashtable();																				
											strSort = " id_vehicle="+idVehicle;
											FillAdvertisers(webSession,tab,dtAdvertiser,dtTotalMarketOrSector,"Sum(total_N)","Sum(total_N1)",strSort,inAdvertVehicleAlreadyUsedArr,ref hPdmVehAdvert,ref hPdmTotAdvert,ref AdvertiserInvestByVeh,ref AdvertiserInvestByVeh_N1,ref indexTabRow,VehicleAccessList,idVehicle,Vehicle,false, ref hasAdvertiser);										
										}
									}
							

									#endregion											
								}
							}
						}		
						#endregion

						#region lignes univers et march� ou famille par cat�gorie
						/*Si utilisateur a s�lectionn� des cat�gories on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */
						if(TreatCategory(webSession)){
							#region ligne total march� ou famille par cat�gorie
					
							if(dtTotalMarketOrSector.Columns.Contains("id_category") && dtTotalMarketOrSector.Columns.Contains("category")){						
								idCategory = currentRow["id_category"].ToString();
								Category = currentRow["category"].ToString();						
								//On vide la liste des cat�gories anciennement trait�s d�s qu'on change de m�dia (vehicle)
								if(start==1 && (int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) && inTotMarketOrSectorCategoryAlreadyUsedArr!=null && inTotMarketOrSectorCategoryAlreadyUsedArr.Count>0 ){
									inTotMarketOrSectorCategoryAlreadyUsedArr = null;
									inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
								}
								if(!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory)){
									if(dtTotalMarketOrSector.Columns.Contains("total_N")){								
										//investissement total march� ou famille par cat�gorie										
										ComputeInvestPdmEvol(dtTotalMarketOrSector,ref TotalMarketOrSectorVehicleInvest ,ref TotalMarketOrSectorCategoryInvest,ref TotalMarketOrSectorCategoryInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle="+idVehicle+" AND id_category = "+idCategory,webSession.ComparativeStudy);
										// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie
										if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorCategoryInvest.Trim())){
											tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","",idCategory,Category,"","","","",false,TotalMarketOrSectorCategoryInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);
											increment=true;
										}
									}									
									tab = FillTabFisrtElmt(tab,ref TotalMarketOrSectorCategoryInvest,dtMarketOrSector1stAdvertiserByCat,dtMarketOrSector1stProductByCat,"id_category",idCategory,ref inTotMarketOrSectorCategoryAlreadyUsedArr,ref indexTabRow,increment,false);
									increment=false;
								}
							}
					
							#endregion

							#region ligne total univers par cat�gorie					
							if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_category") && dtTotalUniverse.Columns.Contains("category")){						
								idCategory = currentRow["id_category"].ToString();
								Category = currentRow["category"].ToString();						
								//On vide la liste des cat�gories trait�s lorsqu'on change de m�dia (vehicle)
								if(start==1 && (int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) && inTotUnivCategoryAlreadyUsedArr!=null && inTotUnivCategoryAlreadyUsedArr.Count>0 ){
									inTotUnivCategoryAlreadyUsedArr=null;
									inTotUnivCategoryAlreadyUsedArr = new ArrayList();
								}
								//pour chaque cat�gorie distincte
								if(!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory)){
									if(dtTotalUniverse.Columns.Contains("total_N"))	{							
										//investissement total univers par cat�gorie										
										ComputeInvestPdmEvol(dtTotalUniverse,ref TotalUnivVehicleInvest,ref TotalUnivCategoryInvest,ref TotalUnivCategoryInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle="+idVehicle+" AND id_category = "+idCategory,webSession.ComparativeStudy);
										// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie 
										if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivCategoryInvest.Trim())){
											tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","",idCategory,Category,"","","","",false,TotalUnivCategoryInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);
											increment=true;
										}
									}																	
									tab = FillTabFisrtElmt(tab,ref TotalUnivCategoryInvest,dtUniverse1stAdvertiserByCat,dtUniverse1stProductByCat,"id_category",idCategory,ref inTotUnivCategoryAlreadyUsedArr,ref indexTabRow,increment,false);
									increment=false;
									if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
										#region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau cat�gorie
										//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
										if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_advertiser")){
											//Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
											if(dtTotalUniverse.Columns.Contains("id_media")){
												//si le niveau support est le plus bas alors vider la collection des annonceurs anciennement trait�s
												if(start==1 && ((int.Parse(idCategory)!=int.Parse(OldIdCategory)) ||  (int.Parse(idMedia)!=int.Parse(OldIdMedia))) ){
													inAdvertCategoryAlreadyUsedArr = null;
													inAdvertCategoryAlreadyUsedArr = new ArrayList();
											
												}																				
											}
											else{
												//si le niveau cat�gorie est le plus bas alors vider la collection des cat�gories anciennement trait�s
												if(start==1 && ((int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) || (int.Parse(idCategory)!=int.Parse(OldIdCategory)))){
													inAdvertCategoryAlreadyUsedArr = null;
													inAdvertCategoryAlreadyUsedArr = new ArrayList();											
												}																			
											}	
											//si on change de cat�gorie ou support la liste des annonceurs d�j� trait�s et leur investissment est vid�e.
											if(start==1 && ((int.Parse(idCategory)!=int.Parse(OldIdCategory)) || (int.Parse(idMedia)!=int.Parse(OldIdMedia)))){
												hPdmCatAdvert=null;
												hPdmCatAdvert = new Hashtable();
											}									
											if((int.Parse(idCategory)!=int.Parse(OldIdCategory)) || (int.Parse(idVehicle)!=int.Parse(OldIdVehicle))){
												//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
										
												foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){																						 
													#region ligne annonceurs de r�f�rences ou concurrents par  cat�gorie																																			
													idAdvertiser=currentAdvertRow["id_advertiser"].ToString();																																																					
													if(dtAdvertiser.Columns.Contains("total_N")){
														if(!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser)){
															#region  support est le niveau le plus bas
															if(dtTotalUniverse.Columns.Contains("id_media")){
																strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory+ " AND id_advertiser="+idAdvertiser;																								
																strSort = "id_media ASC";																
																tab = FillAdvertisers(webSession,tab,dtAdvertiser,dtTotalMarketOrSector,strExpr,strSort,ref hPdmVehAdvert,ref hPdmCatAdvert,ref indexTabRow,idVehicle,Vehicle,idCategory,Category,idMedia,Media,idAdvertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);
															}														
															#endregion
															if(!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser))inAdvertCategoryAlreadyUsedArr.Add(idAdvertiser);//annonceurs trait�s doivent �tre distincts au niveau cat�gorie
														}
														if(!inAdvertCategoryAlreadyUsedArr.Contains(idCategory) && !dtTotalUniverse.Columns.Contains("id_media")){
															#region  cat�gorie est le niveau le plus bas																																							
															strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory;
															strSort = "id_category ASC";												
															foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);																											
															tab = FillAdvertisers(webSession,tab,dtTotalMarketOrSector,foundRows,ref AdvertiserInvestByCat,ref AdvertiserInvestByCat_N1,ref hPdmVehAdvert,ref hPdmCatAdvert,ref indexTabRow,idVehicle,Vehicle,idCategory,Category,"","",CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory,false);
															#endregion																								
															if(!inAdvertCategoryAlreadyUsedArr.Contains(idCategory))inAdvertCategoryAlreadyUsedArr.Add(idCategory);//cat�gories trait�s doivent �tre distincts 
														}
													}																							
													#endregion
												}	
											}
										}								
										#endregion
									}								
									if(!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory))inTotUnivCategoryAlreadyUsedArr.Add(idCategory);
								
								}
							}
					
							#endregion
						}										
						#endregion

						#region lignes univers et march� ou famille par supports (media)
						/*Si utilisateur a s�lectionn� de(s) support(s) on calcule investissement total march� ou famille et univers, et
					 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					 */ if(TreatMedia(webSession)){
							#region ligne total march� ou famille par support (media)					
							if(dtTotalMarketOrSector.Columns.Contains("id_media") && dtTotalMarketOrSector.Columns.Contains("media")){						
								idMedia = currentRow["id_media"].ToString();
								Media = currentRow["media"].ToString();
								//On vide la liste des supports anciennement trait�s lorsqu'on change de cat�gorie
								if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) && inTotMarketOrSectorMediaAlreadyUsedArr!=null && inTotMarketOrSectorMediaAlreadyUsedArr.Count>0 ){
									inTotMarketOrSectorMediaAlreadyUsedArr = null;
									inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
								}	
								if(!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia)){																		
									if(dtTotalMarketOrSector.Columns.Contains("total_N"))	{
										//investissement total univers par support (media)									
										ComputeInvestPdmEvol(dtTotalMarketOrSector,ref TotalMarketOrSectorCategoryInvest ,ref TotalMarketOrSectorMediaInvest,ref TotalMarketOrSectorMediaInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle="+idVehicle+" AND id_category="+idCategory+" AND id_media = "+idMedia,webSession.ComparativeStudy);
										// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
										if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim())){
											tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","","","",idMedia,Media,"","",false,TotalMarketOrSectorMediaInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia);
											increment=true;
										}
									}									
									tab = FillTabFisrtElmt(tab,ref TotalMarketOrSectorMediaInvest,dtMarketOrSector1stAdvertiserByMed,dtMarketOrSector1stProductByMed,"id_media",idMedia,ref inTotMarketOrSectorMediaAlreadyUsedArr,ref indexTabRow,increment,false);
									increment=false;
								}
							}
					
							#endregion

							#region ligne total univers par support (media)					
							if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_media") && dtTotalUniverse.Columns.Contains("media")){						
								idMedia = currentRow["id_media"].ToString();
								Media = currentRow["media"].ToString();
								//On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
								if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) && inTotUnivMediaAlreadyUsedArr!=null && inTotUnivMediaAlreadyUsedArr.Count>0 ){
									inTotUnivMediaAlreadyUsedArr = null;
									inTotUnivMediaAlreadyUsedArr = new ArrayList();
								}
								if(!inTotUnivMediaAlreadyUsedArr.Contains(idMedia)){																			
									if(dtTotalUniverse.Columns.Contains("total_N"))	{
										//investissement total univers support (media)										
										ComputeInvestPdmEvol(dtTotalUniverse,ref TotalUnivCategoryInvest,ref TotalUnivMediaInvest,ref TotalUnivMediaInvest_N1,ref tempPDM,ref tempEvol,"Sum(total_N)","Sum(total_N1)","id_vehicle="+idVehicle+" AND id_category="+idCategory+" AND id_media ="+idMedia,webSession.ComparativeStudy);
										// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
										if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivMediaInvest.ToString().Trim())){
											tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","","","",idMedia,Media,"","",false,TotalUnivMediaInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia);
											increment=true;
										}
									}									
									tab = FillTabFisrtElmt(tab,ref TotalUnivMediaInvest,dtUniverse1stAdvertiserByMed,dtUniverse1stProductByMed,"id_media",idMedia,ref inTotUnivMediaAlreadyUsedArr,ref indexTabRow,increment,false);
									increment=false;
									if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
										#region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau support (media)								
										if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_media") && dtAdvertiser.Columns.Contains("id_advertiser")){
											//On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
											if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) ){
												inAdvertMediaAlreadyUsedArr = null;
												inAdvertMediaAlreadyUsedArr = new ArrayList();										
											}
											foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){
									
												#region ligne annonceurs de r�f�rences ou concurrents par  supports
												//Pour chaque annonceur distinct concurrent ou r�f�rence																						
												idAdvertiser = currentAdvertRow["id_advertiser"].ToString();																		
												if(WebFunctions.CheckedText.IsStringEmpty(idMedia) && !inAdvertMediaAlreadyUsedArr.Contains(idMedia)){									
													if(dtAdvertiser.Columns.Contains("total_N")){
														strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory+" AND id_media="+idMedia;
														strSort = "id_media ASC";							
														foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);																
														tab = FillAdvertisers(webSession,tab,dtTotalMarketOrSector,foundRows,ref AdvertiserInvestByCat,ref AdvertiserInvestByCat_N1,ref hPdmCatAdvert,ref hPdmCatAdvert,ref indexTabRow,idVehicle,Vehicle,idCategory,Category,idMedia,Media,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia,false);
														if(!inAdvertMediaAlreadyUsedArr.Contains(idMedia))inAdvertMediaAlreadyUsedArr.Add(idMedia);//Traitement unique de support											
													}
												}
								
											}
										}
										#endregion

										#endregion
									}								
									if(!inTotUnivMediaAlreadyUsedArr.Contains(idMedia))inTotUnivMediaAlreadyUsedArr.Add(idMedia);
								
								}
							}					
							#endregion
						}					
						#endregion
															
						OldIdVehicle = idVehicle;
						OldIdCategory = idCategory;
						OldIdMedia = idMedia;
						start=1;
					}
				}
				#endregion
		
				#endregion
			
			}catch(Exception ex){
				throw new WebExceptions.IndicatorMediaStrategyException("GetFormattedTable(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion )::Impossible de traiter les donn�es pour la strat�gir m�dia.",ex);
			}

			return tab;
		}
		#endregion

		#region R�sultats de r�partition m�dia (pour sortie sous forme graphique)
		/// <summary>
		/// La r�partition m�dia sur le total de la p�riode dans un tableau
		/// contenant les �l�ments ci-apr�s :
		/// en ligne :
		/// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
		/// total march� (en option)
		/// -les �l�ments de r�f�rences
		/// -les �l�ments concurrents 
		/// en colonne :
		/// -Les investissements de la p�riode N		
		/// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
		/// -si plusieurs media, le tableau sera d�clin� par media
		/// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
		/// </summary>
		/// <remarks>Cette m�thode est utilis�e pour la pr�sentation graphique des r�sultats.</remarks>
		/// <param name="webSession">session du client</param>
		///<param name="comparisonCriterion">crit�re de comparaison</param>		
		/// <returns>tableau de r�sultats</returns>
		public static object[,] GetChartFormattedTable(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion ){
			
			#region variables
			
			#region variables tempon			
			string tempEvol="";
			string tempPDM="";
			#endregion			
			
			#region variables pour les dimensions du tableau de r�sultats
			// tableau de resultats
			object[,] tab = null;			
			//Index d'une ligne du tableau
			int indexTabRow=0;
			//bool�en incr�mentation d'une ligne
			bool increment =false;
			#endregion
			
			#region Variables annonceurs de r�f�rences ou concurrents
			//identifiant  annonceurs de r�f�rences ou concurrents
			string idAdvertiser="";			
			//Y a t'il des annonceurs de r�f�rences ou concurrents
			bool hasAdvertiser=false;
			//Investissement d'un annonceur sur la p�riode N
			string AdvertiserInvest="";			
			//Investissement total des annonceurs sur la p�riode N
			string AdvertiserTotalInvest="";			
			//Investissement d'un annonceur par cat�gorie sur la p�riode N
			string AdvertiserInvestByCat="";			
			//investissement pour une cat�gorie sur p�riode N
			double CatInvest=(double)0.0;
			//investissement pour une cat�gorie sur p�riode N-1
			double CatInvest_N1=(double)0.0;
			//Investissement d'un annonceur par m�dia (vehicle) sur la p�riode N
			string AdvertiserInvestByVeh="";														
			//collection des annonceurs d�j� trait�s
			ArrayList OldIdAdvertiserArr = new ArrayList();
			//collection des annonceurs d�j� trait�s pour le total univers
			ArrayList inTotUnivAdvertAlreadyUsedArr= new ArrayList();	
			//collection des annonceurs d�j� trait�s pour un m�dia (vehicle)
			ArrayList inAdvertVehicleAlreadyUsedArr= new ArrayList();
			//collection des annonceurs d�j� trait�s pour une cxat�gorie
			ArrayList inAdvertCategoryAlreadyUsedArr = new ArrayList();
			//collection des annonceurs d�j� trait�s pour un support (media)
			ArrayList inAdvertMediaAlreadyUsedArr = new ArrayList();			
			#endregion

			#region Variables des supports
			//depart de la boucle 
			int start=0;
			//identifiant d'un m�dia (vehicle)
			string idVehicle="";
			//identifiant m�dia pr�c�dent
			string OldIdVehicle="0";
			//lib�ll� d'un m�dia
			string Vehicle="";			
			//identifiant d'une cat�gorie
			string idCategory="";
			//identifiant pr�c�dent d'une cat�gorie
			string OldIdCategory="0";
			//collection identifiant pr�c�dentes cat�gories
			ArrayList OldIdCategoryArr = new ArrayList();
			//lib�ll� d'une cat�gorie
			string Category="";			
			//identifiant d'un support
			string idMedia="";
			//lib�ll� d'un support
			string Media="";
			//identifiant du pr�c�dent support
			string OldIdMedia="0";
			//collection identifiant pr�c�dents supports
			ArrayList OldIdMediaArr = new ArrayList();																	
			#endregion

			#region construction des listes de produits, media, annonceurs s�lectionn�s				
			DataAccess.Functions.RecapUniversSelection recapUniversSelection = new DataAccess.Functions.RecapUniversSelection(webSession);
			string AdvertiserAccessList = recapUniversSelection.AdvertiserAccessList;
			string CompetitorAdvertiserAccessList = recapUniversSelection.CompetitorAdvertiserAccessList;
			string CategoryAccessList = recapUniversSelection.CategoryAccessList;
			string MediaAccessList = recapUniversSelection.MediaAccessList;
			string VehicleAccessList = recapUniversSelection.VehicleAccessList;
			#endregion

			#region variable pour total univers
			//Y a t'il des donn�es pour calculer les valeurs du total univers
			bool hasTotalUniv =false;	
			//Collecton des m�dia d�j� trait�
			ArrayList inTotUnivVehicleAlreadyUsedArr= new ArrayList();
			//Collecton des cat�gories d�j� trait�
			ArrayList inTotUnivCategoryAlreadyUsedArr = new ArrayList();
			//Collecton des supports d�j� trait�
			ArrayList inTotUnivMediaAlreadyUsedArr = new ArrayList();						
			//investissment total univers par support p�riode N
			string TotalUnivMediaInvest="";			
			//investissment total univers par m�dia p�riode N
			string TotalUnivVehicleInvest="";			
			//investissment total univers par cat�gorie p�riode N
			string TotalUnivCategoryInvest="";			
			//investissment total univers p�riode N
			string TotalUnivInvest="";						
			//groupe de donn�es pour univers
			DataSet dsTotalUniverse = null;
			//table de donn�es pour univers
			DataTable dtTotalUniverse = null;			
			#endregion

			#region variable pour total march� ou famille	
			// y a t'il des donn�es pour le total march�
			bool hasTotalMarketOrSector=false;
			//investissemnt total march� par support p�riode N
			string TotalMarketOrSectorMediaInvest="";			
			//investissemnt total cat�gorie par cat�gorie p�riode N
			string TotalMarketOrSectorCategoryInvest="";			
			//investissemnt total march� par m�dia p�riode N
			string TotalMarketOrSectorVehicleInvest="";			
			//investissemnt total march�  p�riode N
			string TotalMarketOrSectorInvest="";			
			//identifiant des m�dias d�j� trait�s
			ArrayList inTotMarketOrSectorVehicleAlreadyUsedArr =new ArrayList();
			//identifiant des cat�gories d�j� trait�s
			ArrayList inTotMarketOrSectorCategoryAlreadyUsedArr =new ArrayList();
			//identifiant des supports d�j� trait�s
			ArrayList inTotMarketOrSectorMediaAlreadyUsedArr =new ArrayList();			
			//Groupe de donn�es pour total march�
			DataSet dsTotalMarketOrSector = null;
			//table de donn�es pour total march�
			DataTable dtTotalMarketOrSector = null;			
			#endregion

			#region variable pour annonceur de r�f�rences ou concurrent
			//chaine pour colonne � r�cuperer dans collection de DataRow annonceurs
			string strExpr = "";
			//filtre pour colonne � r�cuperer dans collection de DataRow annonceurs
			string strSort = "";
			//DataRow annonceurs
			DataRow[] foundRows=null;
			//Groupe de donn�es annonceurs de r�f�rence ou concurrents
			DataSet dsAdvertiser = null;
			//Table de donn�es annonceurs de r�f�rences ou concurrents
			DataTable dtAdvertiser = null;
			#endregion

			#endregion			

			#region Chargement des donn�es

			#region chargement des donn�es pour les annonceurs de r�f�rences et/ou concurrents
			/* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
			 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
			 * pour chaque annonceur et par niveau support
			 */
			if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
				dsAdvertiser = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,true);
				if(dsAdvertiser!=null && dsAdvertiser.Tables[0]!=null && dsAdvertiser.Tables[0].Rows.Count>0){
					dtAdvertiser = dsAdvertiser.Tables[0];
				}
			}
			#endregion

			#region chargement des donn�es totaux univers
			/*Chargement des donn�es pour les totaux univers
				* dsTotalUniverse : permet de r�cuperer pour chaque niveau support
				* l'investissement,l'evolution,le PDM
				*/				
			dsTotalUniverse = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.universTotal,false);
			if( dsTotalUniverse!=null && dsTotalUniverse.Tables[0]!=null && dsTotalUniverse.Tables[0].Rows.Count>0){
				dtTotalUniverse = dsTotalUniverse.Tables[0];
			}			
			#endregion

			#region chargement des donn�es totaux march�s ou famille(s)
			/* Chargement des donn�es pour les annonceurs de r�f�rences et,ou concurrents
			 * A partir de ces donn�es on peut calculer l'investissement,l'evolution,le PDM,
			 * pour chaque annonceur et par niveau support
			 */			
			if (comparisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
				dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.sectorTotal,false);
			else if (comparisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal)
				dsTotalMarketOrSector = IndicatorDataAccess.getMediaStrategyData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,CustomerSessions.ComparisonCriterion.marketTotal,false);
			if(dsTotalMarketOrSector!=null && dsTotalMarketOrSector.Tables[0]!=null &&  dsTotalMarketOrSector.Tables[0].Rows.Count>0){
				dtTotalMarketOrSector =  dsTotalMarketOrSector.Tables[0];
			}			
			#endregion
			
			#endregion

			#region  construction du tableau de r�sultats	
	
			#region instanciation du tableau de r�sultats
			/*On instancie le tableau de r�sultats pour strat�gie m�dia
			 */			
			//cr�ation du tableau 			
			tab = TabInstance(webSession,dtTotalMarketOrSector,dtAdvertiser,VehicleAccessList,ConstResults.MediaStrategy.NB_CHART_MAX_COLUMNS);

			// Il n'y a pas de donn�es
			if(tab==null)return(new object[0,0]);	
			#endregion

			#region Remplissage chaque ligne m�dia dans table
			/* Chaque ligne du tableau contient toutes les donn�es d'un annonceur de r�f�rence ou concurrent
			 * (Investissement,Evolution,PDM,lib�ll�) ou d'un total support (media ou cat�gorie ou support)
			 * avec ses param�tres (Investissement,Evolution,PDM,lib�ll�, 1er annonceur et son investissement, idem pour 1er r�f�rence)
			*/			
			if(dtTotalMarketOrSector!=null && dtTotalMarketOrSector.Rows.Count>0){
				/*Si utilisateur a s�lectionn� PLURIMEDIA on calcule investissement total march� ou famille et univers, et
				 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
				 */
				if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList) && DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList)){
					#region ligne total march� ou famille pour PLURIMEDIA
					//Traitement ligne total march� ou famille pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
					if(dtTotalMarketOrSector.Columns.Contains("total_N") && !hasTotalMarketOrSector){
						//investissement total 										
						TotalMarketOrSectorInvest = dtTotalMarketOrSector.Compute("Sum(total_N)","").ToString();						
						if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorInvest.Trim()))	
//							TotalMarketOrSectorInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorInvest)/(double)1000));																																									
						// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� et plurim�dia 
						if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorInvest.Trim())){
							tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,VehicleAccessList,GestionWeb.GetWebWord(210,webSession.SiteLanguage),"","","","","","",true,TotalMarketOrSectorInvest,"",tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
							increment=true;
						}
						if(increment)indexTabRow++; //ligne suivante
						hasTotalMarketOrSector=true;
						increment=false;
					}
					#endregion

					#region ligne univers pour PLURIMEDIA
					//Traitement ligne total univers pour Plurimedia : Investissment,Evolution,PDM,1er nnonceur et son investissment, premi�re r�f�rence et son investissment
					if(dtTotalUniverse.Columns.Contains("total_N")&& !hasTotalUniv)	{
						//investissement total 	univers plurim�dia					
						if(dtTotalUniverse.Columns.Contains("total_N"))	
							TotalUnivInvest=dtTotalUniverse.Compute("Sum(total_N)","").ToString();	
						if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivInvest.Trim()))	
//							TotalUnivInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivInvest)/(double)1000));																						
						// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et plurim�dia 
						if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivInvest.Trim())){
							tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,VehicleAccessList,GestionWeb.GetWebWord(210,webSession.SiteLanguage),"","","","","","",true,TotalUnivInvest,"",tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
							increment=true;
						}
						if(increment)indexTabRow++; 
						increment=false;
						//ligne suivante
						hasTotalUniv=true;
					}
					#endregion
				
					#region lignes  annonceurs de r�f�rences ou concurrents	pour s�lection PLURIMEDIA
					//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par PLURIMEDIA : Investissment,Evolution,PDM
					if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList) && dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && !hasAdvertiser){
						//Pour chaque ligne  TOTAL annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
						foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){					
							if(dtAdvertiser.Columns.Contains("id_advertiser") && dtAdvertiser.Columns.Contains("total_N")){														
								idAdvertiser=currentAdvertRow["id_advertiser"].ToString();	
								if(!inTotUnivAdvertAlreadyUsedArr.Contains(idAdvertiser)){						
									//investissement TOTAL annonceur de r�f�rence ou concurrent 
									AdvertiserTotalInvest = dtAdvertiser.Compute("Sum(total_N)", "id_advertiser = "+idAdvertiser+"").ToString();																										
									if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserTotalInvest.Trim())){
//										AdvertiserTotalInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserTotalInvest)/(double)1000));																														
										//Insertion des donn�es dans la ligne courante pour un annonceur
										if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserTotalInvest.Trim()))
											tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","","","","","",currentAdvertRow["id_advertiser"].ToString(),currentAdvertRow["advertiser"].ToString(),true,AdvertiserTotalInvest,"",tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);										
										indexTabRow++;
										if(!inTotUnivAdvertAlreadyUsedArr.Contains(idAdvertiser))inTotUnivAdvertAlreadyUsedArr.Add(idAdvertiser);					
									}
								}
							}
							hasAdvertiser=true;
						}
					}
					#endregion
				}
			
				//Pour chaque ligne  media du total univers on r�cup�re les donn�es				
				foreach(DataRow currentRow in dtTotalMarketOrSector.Rows){
								
					#region lignes univers et march� ou famille par m�dia (vehicle)
					/*Si utilisateur a s�lectionn� un MEDIA (vehicle) on calcule investissement total march� ou famille et univers, et
					* les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
					*/
					#region ligne total march� ou famille par m�dia (vehicle)
					//colonne m�dia (vehicle)
					if(dtTotalMarketOrSector.Columns.Contains("id_vehicle") && dtTotalMarketOrSector.Columns.Contains("vehicle")){																			
						idVehicle = currentRow["id_vehicle"].ToString();
						Vehicle = currentRow["vehicle"].ToString();		
						if(!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle)){		
							if(dtTotalMarketOrSector.Columns.Contains("total_N")){
								//investissement total univers pour m�dia (vehicle)
								TotalMarketOrSectorVehicleInvest=dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle = "+idVehicle+"").ToString();																									
								if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorVehicleInvest.ToString().Trim())){
//									TotalMarketOrSectorVehicleInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorVehicleInvest)/(double)1000));																																
									// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 								
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,"","","","","","",false,TotalMarketOrSectorVehicleInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
									increment=true;
								}
							}																					
							if(!inTotMarketOrSectorVehicleAlreadyUsedArr.Contains(idVehicle))inTotMarketOrSectorVehicleAlreadyUsedArr.Add(idVehicle);
							if(increment)indexTabRow++; //ligne suivante
							increment=false;
						}
					}										
					#endregion

					#region ligne total univers par m�dia (vehicle)				
					if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_vehicle") && dtTotalUniverse.Columns.Contains("vehicle")){																
						idVehicle = currentRow["id_vehicle"].ToString();
						Vehicle = currentRow["vehicle"].ToString();		
						if(!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle)){
							if(dtTotalUniverse.Columns.Contains("total_N"))	{
								//investissement total univers pour m�dia (vehicle)
								TotalUnivVehicleInvest=dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle = "+idVehicle+"").ToString();																	
								if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivVehicleInvest.ToString().Trim())){
//									TotalUnivVehicleInvest =  String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivVehicleInvest)/(double)1000));																									
									// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et media (vehicle) 								
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,"","","","","","",false,TotalUnivVehicleInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
									increment=true;
								}
							}														
							if(!inTotUnivVehicleAlreadyUsedArr.Contains(idVehicle))inTotUnivVehicleAlreadyUsedArr.Add(idVehicle);
							if(increment)indexTabRow++; //ligne suivante
							increment=false;						
													
							#endregion

							if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){											
								#region Investissement pour annonceur de r�f�rence ou concurrent Niveau m�dia (vehicle)
								//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par MEDIA (vehicle) : Investissment
								if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_advertiser")){
									//Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
									if((int.Parse(idVehicle)!=int.Parse(OldIdVehicle))){ 
										inAdvertVehicleAlreadyUsedArr = null;
										inAdvertVehicleAlreadyUsedArr = new ArrayList();										
										foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){
											//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par media (vehicle) : Investissment,Evolution,PDM
											#region ligne annonceurs de r�f�rences ou concurrents par m�dia (vehicle)
											//colonne m�dia (vehicle)																						
											idAdvertiser = currentAdvertRow["id_advertiser"].ToString();																					
											if(!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser)){									
												if(dtAdvertiser.Columns.Contains("total_N"))	{
													//investissement annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
													AdvertiserInvestByVeh = dtAdvertiser.Compute("Sum(total_N)", "id_advertiser = "+idAdvertiser+" AND id_vehicle="+idVehicle).ToString();																	
//													if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh.Trim())){
//														AdvertiserInvestByVeh = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh)/(double)1000));														
//													}													
													//Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia (vehicle)
													if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh.Trim()))
														tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,"","","","",currentAdvertRow["id_advertiser"].ToString(),currentAdvertRow["advertiser"].ToString(),false,AdvertiserInvestByVeh,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);													
													if(!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser))inAdvertVehicleAlreadyUsedArr.Add(idAdvertiser);
													indexTabRow++;	
												}
																		
											}
										}
									}
								}
								#endregion

								#endregion											
							}
						}
					}		
					#endregion

					#region lignes univers et march� ou famille par cat�gorie
					/*Si utilisateur a s�lectionn� des cat�gories on calcule investissement total march� ou famille et univers, et
				 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
				 */
					#region ligne total march� ou famille par cat�gorie					
					if(dtTotalMarketOrSector.Columns.Contains("id_category") && dtTotalMarketOrSector.Columns.Contains("category")){						
						idCategory = currentRow["id_category"].ToString();
						Category = currentRow["category"].ToString();						
						//On vide la liste des cat�gories anciennement trait�s d�s qu'on change de m�dia (vehicle)
						if(start==1 && (int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) && inTotMarketOrSectorCategoryAlreadyUsedArr!=null && inTotMarketOrSectorCategoryAlreadyUsedArr.Count>0 ){
							inTotMarketOrSectorCategoryAlreadyUsedArr = null;
							inTotMarketOrSectorCategoryAlreadyUsedArr = new ArrayList();
						}
						if(!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory)){
							if(dtTotalMarketOrSector.Columns.Contains("total_N")){								
								//investissement total march� ou famille par cat�gorie
								TotalMarketOrSectorCategoryInvest=dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle="+idVehicle+" AND id_category = "+idCategory).ToString();																									
								if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorCategoryInvest.ToString().Trim())){
//									TotalMarketOrSectorCategoryInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorCategoryInvest)/(double)1000));
									increment=true;
								}																													
								// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie
								if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorCategoryInvest.Trim()))
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","",idCategory,Category,"","","","",false,TotalMarketOrSectorCategoryInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);
							}														
							if(!inTotMarketOrSectorCategoryAlreadyUsedArr.Contains(idCategory))inTotMarketOrSectorCategoryAlreadyUsedArr.Add(idCategory);//Les cat�gories trait�es doivent �tre distincts
							if(increment)indexTabRow++; //ligne suivante
							increment=false;
						}
					}
					
					#endregion

					#region ligne total univers par cat�gorie					
					if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_category") && dtTotalUniverse.Columns.Contains("category")){						
						idCategory = currentRow["id_category"].ToString();
						Category = currentRow["category"].ToString();						
						//On vide la liste des cat�gories trait�s lorsqu'on change de m�dia (vehicle)
						if(start==1 && (int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) && inTotUnivCategoryAlreadyUsedArr!=null && inTotUnivCategoryAlreadyUsedArr.Count>0 ){
							inTotUnivCategoryAlreadyUsedArr=null;
							inTotUnivCategoryAlreadyUsedArr = new ArrayList();
						}
						//pour chaque cat�gorie distincte
						if(!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory)){
							if(dtTotalUniverse.Columns.Contains("total_N"))	{							
								//investissement total univers par cat�gorie
								TotalUnivCategoryInvest=dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle="+idVehicle+" AND id_category = "+idCategory).ToString();																	
//								if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivCategoryInvest.ToString().Trim()))
//									TotalUnivCategoryInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivCategoryInvest)/(double)1000));																
								// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et cat�gorie 
								if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivCategoryInvest.Trim())){
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","",idCategory,Category,"","","","",false,TotalUnivCategoryInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);
									increment=true;
								}
							}																	
							if(increment)indexTabRow++;
							increment=false;
							if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
								#region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau cat�gorie
								//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
								if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_advertiser")){
									//Pour chaque ligne  annonceur de r�f�rence ou concurrent on r�cup�re les donn�es
									if(dtTotalUniverse.Columns.Contains("id_media")){
										//si le niveau support est le plus bas alors vider la collection des annonceurs anciennement trait�s
										if(start==1 && ((int.Parse(idCategory)!=int.Parse(OldIdCategory)) ||  (int.Parse(idMedia)!=int.Parse(OldIdMedia))) ){
											inAdvertCategoryAlreadyUsedArr = null;
											inAdvertCategoryAlreadyUsedArr = new ArrayList();											
										}																				
									}
									else{
										//si le niveau cat�gorie est le plus bas alors vider la collection des cat�gories anciennement trait�s
										if(start==1 && ((int.Parse(idVehicle)!=int.Parse(OldIdVehicle)) || (int.Parse(idCategory)!=int.Parse(OldIdCategory)))){
											inAdvertCategoryAlreadyUsedArr = null;
											inAdvertCategoryAlreadyUsedArr = new ArrayList();											
										}																			
									}																	
									if((int.Parse(idCategory)!=int.Parse(OldIdCategory)) || (int.Parse(idVehicle)!=int.Parse(OldIdVehicle))){
										//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par cat�gorie : Investissment,Evolution,PDM
										
										foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){																						
											#region ligne annonceurs de r�f�rences ou concurrents par  cat�gorie																																			
											idAdvertiser=currentAdvertRow["id_advertiser"].ToString();																																																					
											if(dtAdvertiser.Columns.Contains("total_N")){
												if(!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser)){
													#region  support est le niveau le plus bas
													if(dtTotalUniverse.Columns.Contains("id_media")){
														strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory+ " AND id_advertiser="+idAdvertiser;																								
														strSort = "id_media ASC";												
														foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);												
														if(foundRows!=null && foundRows.Length>0 ){
															CatInvest=(double)0.0;
															CatInvest_N1=(double)0.0;
															//investissement annonceur de r�f�rence ou concurrent  par cat�gorie sur p�riode N
															for(int n=0; n<foundRows.Length;n++){
																CatInvest+=double.Parse(foundRows[n]["total_N"].ToString());
																if(webSession.ComparativeStudy)CatInvest_N1+=double.Parse(foundRows[n]["total_N1"].ToString());																
															}																														
															AdvertiserInvestByCat = CatInvest.ToString();
//															if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat.Trim())){
//																AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat)/(double)1000));																
//															}																														
															//Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
															if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat.Trim()))
																tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,idCategory,Category,"","",foundRows[0]["id_advertiser"].ToString(),foundRows[0]["advertiser"].ToString(),false,AdvertiserInvestByCat,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);																														
															indexTabRow++;															
														}
													}
													#endregion
													if(!inAdvertCategoryAlreadyUsedArr.Contains(idAdvertiser))inAdvertCategoryAlreadyUsedArr.Add(idAdvertiser);//annonceurs trait�s doivent �tre distincts au niveau cat�gorie
												}
												if(!inAdvertCategoryAlreadyUsedArr.Contains(idCategory) && !dtTotalUniverse.Columns.Contains("id_media")){
													#region  cat�gorie est le niveau le plus bas																																							
													strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory;
													strSort = "id_category ASC";												
													foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);												
													if(foundRows!=null && foundRows.Length>0 ){
														for(int n=0; n<foundRows.Length;n++){																																															
															//investissement annonceur de r�f�rence ou concurrent  par cat�gorie
//															if(WebFunctions.CheckedText.IsStringEmpty(foundRows[n]["total_N"].ToString()))
//																AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N"].ToString())/(double)1000));																																													
															//Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
															if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat.Trim()))
																tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,idCategory,Category,"","",foundRows[n]["id_advertiser"].ToString(),foundRows[n]["advertiser"].ToString(),false,AdvertiserInvestByCat,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory);															
															indexTabRow++;
														}
													}													
													#endregion																								
													if(!inAdvertCategoryAlreadyUsedArr.Contains(idCategory))inAdvertCategoryAlreadyUsedArr.Add(idCategory);//acat�gories trait�s doivent �tre distincts 
												}
											}																							
											#endregion
										}	
									}
								}								
								#endregion
							}
							if(!inTotUnivCategoryAlreadyUsedArr.Contains(idCategory))inTotUnivCategoryAlreadyUsedArr.Add(idCategory);
						}
					}
					
					#endregion
					
					#endregion

					#region lignes univers et march� ou famille par supports (media)
					/*Si utilisateur a s�lectionn� de(s) support(s) on calcule investissement total march� ou famille et univers, et
				 * les param�tres associ�s (PDM,evolution,1er annonceurs et r�f�rences)
				 */
					#region ligne total march� ou famille par support (media)					
					if(dtTotalMarketOrSector.Columns.Contains("id_media") && dtTotalMarketOrSector.Columns.Contains("media")){						
						idMedia = currentRow["id_media"].ToString();
						Media = currentRow["media"].ToString();
						//On vide la liste des supports anciennement trait�s lorsqu'on change de cat�gorie
						if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) && inTotMarketOrSectorMediaAlreadyUsedArr!=null && inTotMarketOrSectorMediaAlreadyUsedArr.Count>0 ){
							inTotMarketOrSectorMediaAlreadyUsedArr = null;
							inTotMarketOrSectorMediaAlreadyUsedArr = new ArrayList();
						}	
						if(!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia)){																		
							if(dtTotalMarketOrSector.Columns.Contains("total_N"))	{
								//investissement total univers par support (media)								
								TotalMarketOrSectorMediaInvest=dtTotalMarketOrSector.Compute("Sum(total_N)", "id_vehicle="+idVehicle+" AND id_category="+idCategory+" AND id_media = "+idMedia).ToString();																	
								if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim())){
//									TotalMarketOrSectorMediaInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalMarketOrSectorMediaInvest)/(double)1000));
									increment=true;
								}																														
								// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
								if(WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorMediaInvest.ToString().Trim()))
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","","","",idMedia,Media,"","",false,TotalMarketOrSectorMediaInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia);
							}							
							if(!inTotMarketOrSectorMediaAlreadyUsedArr.Contains(idMedia))inTotMarketOrSectorMediaAlreadyUsedArr.Add(idMedia);//On traite des m�dia distincts
							if(increment)indexTabRow++; //ligne suivante
							increment=false;
						}
					}					
					#endregion

					#region ligne total univers par support (media)					
					if(dtTotalUniverse!=null && dtTotalUniverse.Columns.Contains("id_media") && dtTotalUniverse.Columns.Contains("media")){						
						idMedia = currentRow["id_media"].ToString();
						Media = currentRow["media"].ToString();
						//On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
						if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) && inTotUnivMediaAlreadyUsedArr!=null && inTotUnivMediaAlreadyUsedArr.Count>0 ){
							inTotUnivMediaAlreadyUsedArr = null;
							inTotUnivMediaAlreadyUsedArr = new ArrayList();
						}
						if(!inTotUnivMediaAlreadyUsedArr.Contains(idMedia)){																			
							if(dtTotalUniverse.Columns.Contains("total_N"))	{
								//investissement total univers support (media)
								TotalUnivMediaInvest=dtTotalUniverse.Compute("Sum(total_N)", "id_vehicle="+idVehicle+" AND id_category="+idCategory+" AND id_media ="+idMedia).ToString();																	
//								if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivMediaInvest.ToString().Trim()))
//									TotalUnivMediaInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(TotalUnivMediaInvest)/(double)1000));																
								// Remplit la ligne courante avec investissement,Evolution et PDM pour le total march� ou famille et support 
								if(WebFunctions.CheckedText.IsStringEmpty(TotalUnivMediaInvest.ToString().Trim())){
									tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,"","","","",idMedia,Media,"","",false,TotalUnivMediaInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.total,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia);
									increment=true;
								}
							}										
							if(increment)indexTabRow++;
							increment=false;

							if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList)){
								#region Investissement,PDM,EVOL pour annonceur de r�f�rence ou concurrent par Niveau support (media)								
								if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0 && dtAdvertiser.Columns.Contains("id_vehicle") && dtAdvertiser.Columns.Contains("id_category") && dtAdvertiser.Columns.Contains("id_media") && dtAdvertiser.Columns.Contains("id_advertiser")){
									//On vide la liste des supports anciennement trat�s d�s qu'on change de cat�gorie
									if(start==1 && (int.Parse(idCategory)!=int.Parse(OldIdCategory)) ){
										inAdvertMediaAlreadyUsedArr = null;
										inAdvertMediaAlreadyUsedArr = new ArrayList();										
									}
									foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){									
										#region ligne annonceurs de r�f�rences ou concurrents par  supports
										//Pour chaque annonceur distinct concurrent ou r�f�rence																						
										idAdvertiser = currentAdvertRow["id_advertiser"].ToString();																		
										if(WebFunctions.CheckedText.IsStringEmpty(idMedia) && !inAdvertMediaAlreadyUsedArr.Contains(idMedia)){									
											if(dtAdvertiser.Columns.Contains("total_N")){
												strExpr = "id_vehicle = "+idVehicle+" AND id_category = "+idCategory+" AND id_media="+idMedia;
												strSort = "id_media ASC";							
												foundRows = dtAdvertiser.Select(strExpr, strSort, DataViewRowState.OriginalRows);		
												if(foundRows!=null && foundRows.Length>0 ){
													for(int i=0; i<foundRows.Length;i++){//														
														//investissement annonceur de r�f�rence ou concurrent  par support												
														AdvertiserInvest = foundRows[i]["total_N"].ToString();
//														if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvest.Trim()))
//															AdvertiserInvest=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvest)/(double)1000));																												
														//Insertion des donn�es dans la ligne courante pour un annonceur et par support
														if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh.Trim()))
															tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,idCategory,Category,idMedia,Media,foundRows[i]["id_advertiser"].ToString(),foundRows[i]["advertiser"].ToString(),false,AdvertiserInvest,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia);
														indexTabRow++;		
													}																						
												}
												if(!inAdvertMediaAlreadyUsedArr.Contains(idMedia))inAdvertMediaAlreadyUsedArr.Add(idMedia);//Traitement unique de support											
											}
										}
								
									}
								}
								#endregion
								#endregion
							}
							if(!inTotUnivMediaAlreadyUsedArr.Contains(idMedia))inTotUnivMediaAlreadyUsedArr.Add(idMedia);
						}
					}					
					#endregion
					
					#endregion
															
					OldIdVehicle = idVehicle;
					OldIdCategory = idCategory;
					OldIdMedia = idMedia;
					start=1;
				}
			}
			#endregion
		
			#endregion
			return tab;
		}
		#endregion

		#region M�thodes internes								

		#region Annonceurs de r�f�rence ou concurrents
		/// <summary>
		/// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'une cat�gorie ou d'un m�dia(support).
		/// </summary>
		/// <param name="webSession"></param>
		/// <param name="tab">session du client</param>
		/// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
		/// <param name="foundRows">liste d'annonceurs</param>
		/// <param name="AdvertiserInvest">investissement par annonceur</param>
		/// <param name="AdvertiserInvest_N1">investissement par annonceur ann�e pr�c�dente</param>
		/// <param name="hPdmParentAdvert">PDM �l�ment parent</param>
		/// <param name="hPdmChildAdvert">PDM �l�ment enfant</param>
		/// <param name="indexTabRow">index ligne tableau</param>
		/// <param name="idVehicle">Identifiant m�dia</param>
		/// <param name="Vehicle">Libell� m�dia</param>
		/// <param name="idCategory">identifiant cat�gorie</param>
		/// <param name="Category">Libell� cat�gorie</param>
		/// <param name="idMedia">identifiant support</param>
		/// <param name="Media">Libell� support</param>
		/// <param name="preformatedMediaDetails">niveau de d�tail m�dia</param>
		/// <param name="isPluri">vrai si plurim�dia</param>
		/// <returns>Tableau de r�sultats</returns>
		private static object[,] FillAdvertisers(WebSession webSession,object[,] tab,DataTable dtTotalMarketOrSector, DataRow[] foundRows,ref string AdvertiserInvest,ref string AdvertiserInvest_N1,ref Hashtable hPdmParentAdvert,ref Hashtable hPdmChildAdvert,ref int indexTabRow,string idVehicle,string Vehicle,string idCategory,string Category,string idMedia,string Media,CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetails,bool isPluri){
			string tempEvol="";
			string tempPDM="";
			double PdmVehicle=0;
			double AdvertiserEvolution=0;

			for(int n=0; n<foundRows.Length;n++){																																															
				//investissement annonceur de r�f�rence ou concurrent  par m�dia
//				if(WebFunctions.CheckedText.IsStringEmpty(foundRows[n]["total_N"].ToString()))
//					AdvertiserInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N"].ToString())/(double)1000));															
				//PDM annonceur de r�f�rence ou concurrent  par m�dia																
				if(hPdmParentAdvert!=null && foundRows[n]["id_advertiser"]!=null && hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()]!=null && WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvest) 
					&& WebFunctions.CheckedText.IsStringEmpty(foundRows[n]["id_advertiser"].ToString().Trim()) && double.Parse(hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()].ToString().Trim())>(double)0.0){
					PdmVehicle = (double.Parse(AdvertiserInvest.ToString().Trim())*(double)100.0)/double.Parse(hPdmParentAdvert[foundRows[n]["id_advertiser"].ToString()].ToString().Trim());																
					tempPDM = PdmVehicle.ToString();
				}else tempPDM="";
				//Evolution par m�dia anne� N par rapport N-1= ((N-(N-1))*100)/N-1 																
				if(webSession.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1")){
					AdvertiserEvolution = (double)0.0;
					//investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par m�dia
//					if(WebFunctions.CheckedText.IsStringEmpty(foundRows[n]["total_N1"].ToString()))
//						AdvertiserInvest_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[n]["total_N1"].ToString())/(double)1000));
					if( WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvest_N1) && WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvest) && double.Parse(AdvertiserInvest_N1)>(double)0.0){
						AdvertiserEvolution = ((double.Parse(AdvertiserInvest) - double.Parse(AdvertiserInvest_N1))*(double)100.0)/double.Parse(AdvertiserInvest_N1);																	
						tempEvol = AdvertiserEvolution.ToString();
					}else tempEvol="";
				}else tempEvol="";
				//Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia
				if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvest.Trim()))
					tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,idCategory,Category,idMedia,Media,foundRows[n]["id_advertiser"].ToString(),foundRows[n]["advertiser"].ToString(),isPluri,AdvertiserInvest,tempPDM,tempEvol,webSession.ComparaisonCriterion,ConstResults.MediaStrategy.InvestmentType.advertiser,preformatedMediaDetails);
				if(CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia!=preformatedMediaDetails &&hPdmChildAdvert[foundRows[n]["id_advertiser"].ToString()]==null)
					hPdmChildAdvert.Add(foundRows[n]["id_advertiser"].ToString().Trim(),AdvertiserInvest);
				indexTabRow++;
			}
			return tab;
		}

		/// <summary>
		/// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'une cat�gorie ou d'un m�dia(support).
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="dtAdvertiser">tableau de donn�es annonceurs</param>
		/// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
		/// <param name="expression">expression de s�lection</param>
		/// <param name="filter">expression de filtrage</param>
		/// <param name="hPdmVehAdvert">PDM annonceur par m�dia (vehicle)</param>
		/// <param name="hPdmCatAdvert">PDM annonceur par cat�gorie</param>
		/// <param name="indexTabRow">index ligne tableau</param>
		/// <param name="idVehicle">ID m�dia (vehicle)</param>
		/// <param name="Vehicle">Libell� m�dia (vehicle)</param>
		/// <param name="idCategory">ID cat�gorie </param>
		/// <param name="Category">libell� cat�gorie</param>
		/// <param name="idMedia">ID support</param>
		/// <param name="Media">libell� support</param>
		/// <param name="idAdvertiser">ID annonceur</param>
		/// <param name="preformatedMediaDetails">niveau de d�tail m�dia</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillAdvertisers(WebSession webSession,object[,] tab,DataTable dtAdvertiser,DataTable dtTotalMarketOrSector,string expression,string filter,ref Hashtable hPdmVehAdvert,ref Hashtable hPdmCatAdvert,ref int indexTabRow,string idVehicle,string Vehicle,string idCategory,string Category,string idMedia,string Media,string idAdvertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetails){
			
			double CatInvest=0;
			double CatInvest_N1=0;
			string tempEvol="";
			string tempPDM="";
			double AdvertiserEvolution=0;
			string AdvertiserInvestByCat="";
			string AdvertiserInvestByCat_N1="";
			double PdmVehicle=0;

			DataRow[] foundRows = dtAdvertiser.Select(expression,filter,DataViewRowState.OriginalRows);												
			if(foundRows!=null && foundRows.Length>0 ){
				CatInvest=(double)0.0;
				CatInvest_N1=(double)0.0;
				//investissement annonceur de r�f�rence ou concurrent  par cat�gorie sur p�riode N
				for(int n=0; n<foundRows.Length;n++){
					CatInvest+=double.Parse(foundRows[n]["total_N"].ToString());
					if(webSession.ComparativeStudy)CatInvest_N1+=double.Parse(foundRows[n]["total_N1"].ToString());																
				}																													
				AdvertiserInvestByCat = CatInvest.ToString();
//				if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat.Trim())){
//					AdvertiserInvestByCat = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat)/(double)1000));																
//				}
				if(hPdmVehAdvert!=null && hPdmVehAdvert[idAdvertiser]!=null && WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat) && WebFunctions.CheckedText.IsStringEmpty(hPdmVehAdvert[idAdvertiser].ToString().Trim()) && double.Parse(hPdmVehAdvert[idAdvertiser].ToString().Trim())>(double)0.0){
					PdmVehicle = (double.Parse(AdvertiserInvestByCat.ToString())*(double)100.0)/double.Parse(hPdmVehAdvert[idAdvertiser].ToString().Trim());																
					tempPDM = PdmVehicle.ToString();
				}else tempPDM="";
				//Evolution par cat�gorie anne� N par rapport N-1 = ((N-(N-1))*100)/N-1 	
				if(webSession.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1")){
					AdvertiserEvolution = (double)0.0;
					//investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par cat�gorie																
					AdvertiserInvestByCat_N1 = CatInvest_N1.ToString();																
					if( WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat_N1) && WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat) ){																	
//						AdvertiserInvestByCat_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByCat_N1)/(double)1000));
						if(double.Parse(AdvertiserInvestByCat_N1)>(double)0.0){
							AdvertiserEvolution = ((double.Parse(AdvertiserInvestByCat) - double.Parse(AdvertiserInvestByCat_N1))*(double)100.0)/double.Parse(AdvertiserInvestByCat_N1.Trim());																	
							tempEvol = AdvertiserEvolution.ToString();
						}else tempEvol="";
					}else tempEvol="";
				}else tempEvol="";
				//Insertion des donn�es dans la ligne courante pour un annonceur et par cat�gorie
				if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByCat.Trim()))
					tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,idCategory,Category,idMedia,Media,foundRows[0]["id_advertiser"].ToString(),foundRows[0]["advertiser"].ToString(),false,AdvertiserInvestByCat,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,preformatedMediaDetails);				
				hPdmCatAdvert.Add(idAdvertiser,AdvertiserInvestByCat);															
				indexTabRow++;															
			}
		

			return tab;
		}

		/// <summary>
		/// Obtient les annonceurs de r�f�rence ou concurrents au niveau d'un vehicle (m�dia) ou plurim�dia.
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="dtAdvertiser">tableau de donn�es annonceurs</param>
		/// <param name="dtTotalMarketOrSector">tableau de donn�es annonceurs total</param>
		/// <param name="expression">expression de s�lection</param>
		/// <param name="expressionPreviuousYear">expression de s�lection ann�e pr�c�dente</param>
		/// <param name="filter">filtre de donn�es</param>
		/// <param name="inAdvertVehicleAlreadyUsedArr">m�dia d�j� trait�s</param>
		/// <param name="hPdmVehAdvert">PDM m�dia (vehicle)</param>
		/// <param name="hPdmTotAdvert">PDM total</param>
		/// <param name="AdvertiserInvestByVeh">investissement annonceur par vehicle(media)</param>
		/// <param name="AdvertiserInvestByVeh_N1">investissement annonceur par vehicle(media) ann�e pr�c�dente</param>
		/// <param name="indexTabRow">index ligne du tableau</param>
		/// <param name="VehicleAccessList">liste des m�dia</param>
		/// <param name="idVehicle">identifiant des m�dia</param>
		/// <param name="Vehicle">libell� du vehicle</param>
		/// <param name="isPluri">vrai si plurim�dia</param>
		/// <param name="hasAdvertiser">vrai si poss�de des annonceurs</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillAdvertisers(WebSession webSession,object[,] tab,DataTable dtAdvertiser,DataTable dtTotalMarketOrSector, string expression,string expressionPreviuousYear,string filter,ArrayList inAdvertVehicleAlreadyUsedArr,ref Hashtable hPdmVehAdvert,ref Hashtable hPdmTotAdvert,ref string AdvertiserInvestByVeh,ref string AdvertiserInvestByVeh_N1,ref int indexTabRow,string VehicleAccessList,string idVehicle,string Vehicle,bool isPluri,ref bool hasAdvertiser){
			
			string idAdvertiser="";
			double AdvertiserEvolution = 0;
			double PdmVehicle = 0;
			string tempEvol="";
			string tempPDM="";
			string localFilter="";


			foreach(DataRow currentAdvertRow in dtAdvertiser.Rows){
				//Pour les annonceurs de r�f�rence ou concurrents s�lectionn�s par media (vehicle) : Investissment,Evolution,PDM
				
				//colonne m�dia (vehicle)																						
				idAdvertiser = currentAdvertRow["id_advertiser"].ToString();																					
				if(!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser)){									
					if(dtAdvertiser.Columns.Contains("total_N"))	{
						if(filter.Length>0)localFilter=filter+" AND id_advertiser="+idAdvertiser;
						else localFilter=" id_advertiser="+idAdvertiser;
						//investissement annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
						AdvertiserInvestByVeh = dtAdvertiser.Compute(expression,localFilter).ToString();																										
//						if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh.Trim())){
//							AdvertiserInvestByVeh = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh)/(double)1000));														
//						}
						//PDM annonceur de r�f�rence ou concurrent  par m�dia (vehicle)												
						if(!isPluri &&  DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList)){
							if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh) && hPdmTotAdvert!=null && hPdmTotAdvert[idAdvertiser]!=null && WebFunctions.CheckedText.IsStringEmpty(hPdmTotAdvert[idAdvertiser].ToString().Trim()) && double.Parse(hPdmTotAdvert[idAdvertiser].ToString().Trim())>(double)0.0){
								PdmVehicle = (double.Parse(AdvertiserInvestByVeh.ToString())*(double)100.0)/double.Parse(hPdmTotAdvert[idAdvertiser].ToString().Trim());
								tempPDM=PdmVehicle.ToString();
							}else tempPDM="";
						}else tab[indexTabRow,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]="100";
						//Evolution pour m�dia (vehicle) anne� N par rapport N-1	= ((N-(N-1))*100)/N-1 	
						if(webSession.ComparativeStudy && dtTotalMarketOrSector.Columns.Contains("total_N") && dtTotalMarketOrSector.Columns.Contains("total_N1")){
							AdvertiserEvolution = (double)0.0;
							//investissement p�riode N-1 annonceur de r�f�rence ou concurrent  par m�dia (vehicle)
							AdvertiserInvestByVeh_N1 = dtAdvertiser.Compute(expressionPreviuousYear,localFilter).ToString();																			
							if( WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh_N1) && WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh)){
//								AdvertiserInvestByVeh_N1 = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(AdvertiserInvestByVeh_N1)/(double)1000));
								if( double.Parse(AdvertiserInvestByVeh_N1)>(double)0.0){
									AdvertiserEvolution = ((double.Parse(AdvertiserInvestByVeh) - double.Parse(AdvertiserInvestByVeh_N1))*(double)100.0)/double.Parse(AdvertiserInvestByVeh_N1);															
									tempEvol = AdvertiserEvolution.ToString();
								}else tempEvol="";
							}else tempEvol="";
						}else tempEvol="";
						//Insertion des donn�es dans la ligne courante pour un annonceur et par m�dia (vehicle)
						if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserInvestByVeh.Trim()))
							tab = FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,"","","","",currentAdvertRow["id_advertiser"].ToString(),currentAdvertRow["advertiser"].ToString(),false,AdvertiserInvestByVeh,tempPDM,tempEvol,WebConstantes.CustomerSessions.ComparisonCriterion.universTotal,ConstResults.MediaStrategy.InvestmentType.advertiser,CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle);
						if(hPdmVehAdvert[idAdvertiser]==null)hPdmVehAdvert.Add(idAdvertiser,AdvertiserInvestByVeh);
						if(!inAdvertVehicleAlreadyUsedArr.Contains(idAdvertiser))inAdvertVehicleAlreadyUsedArr.Add(idAdvertiser);
						indexTabRow++;	
					}
																		
				}
				localFilter="";
				if(isPluri)hasAdvertiser=true;
			}
			
			return tab;
		}
		#endregion

		#region Investissement,PDM,Evolution

		/// <summary>
		/// Calcule les investissements,PDM et l'evolution pour chaque m�dia.
		/// </summary>
		/// <param name="dt">Table de donn�es</param>
		/// <param name="totalInvestParent">Investissement du m�dia parent</param>
		/// <param name="totalInvestChild">Investissement du m�dia fils</param>
		/// <param name="totalInvestChildPreviousYear">Investissement du m�dia fils ann�e pr�c�dente</param>
		/// <param name="mediaPDM">PDM m�dia</param>	
		/// <param name="evolution">evolution</param>
		/// <param name="expression">expression selection lignes tables de donn�es</param>
		/// <param name="expressionPreviuousYear">expression selection lignes tables de donn�es ann�e pr�c�dente</param>
		/// <param name="filter">expression filtrage des donn�es</param>	
		/// <param name="comparativeStudy">vrai si �tude comparative</param>
		private static void ComputeInvestPdmEvol(DataTable dt,ref string totalInvestParent,ref string totalInvestChild,ref string totalInvestChildPreviousYear,ref string mediaPDM,ref string evolution,string expression,string expressionPreviuousYear,string filter,bool comparativeStudy){
			
			double tempPDM = 0;
			double tempEvol = 0;

			//Investissement total univers pour m�dia (vehicle)
			totalInvestChild=dt.Compute(expression,filter).ToString();																									
//			if(WebFunctions.CheckedText.IsStringEmpty(totalInvestChild.ToString().Trim())){
//				totalInvestChild=String.Format("{0:### ### ### ### ##0.##}",(double.Parse(totalInvestChild)/(double)1000));
//			}

			//PDM pour m�dia 				
			if(WebFunctions.CheckedText.IsStringEmpty(totalInvestParent) && WebFunctions.CheckedText.IsStringEmpty(totalInvestChild.ToString())){
				tempPDM =(double)0.0;
				if(WebFunctions.CheckedText.IsStringEmpty(totalInvestParent) && double.Parse(totalInvestParent)>(double)0.0){
					tempPDM = (double.Parse(totalInvestChild.ToString())*(double)100.0)/double.Parse(totalInvestParent);											
					mediaPDM  = tempPDM.ToString();																				
				}else mediaPDM="";
			}else mediaPDM="";
																	
			//Evolution pour m�dia 
			if(comparativeStudy && dt.Columns.Contains("total_N") && dt.Columns.Contains("total_N1")){
				tempEvol=(double)0.0;
				totalInvestChildPreviousYear = dt.Compute(expressionPreviuousYear,filter).ToString();
//				if(WebFunctions.CheckedText.IsStringEmpty(totalInvestChildPreviousYear))totalInvestChildPreviousYear =String.Format("{0:### ### ### ### ##0.##}",(double.Parse(totalInvestChildPreviousYear)/(double)1000));
				//anne� N par rapport N-1 = ((N-(N-1))*100)/N-1 
				if(WebFunctions.CheckedText.IsStringEmpty(totalInvestChildPreviousYear) && WebFunctions.CheckedText.IsStringEmpty(totalInvestChild) && double.Parse(totalInvestChildPreviousYear)>(double)0.0){									
					tempEvol=((double.Parse(totalInvestChild)-double.Parse(totalInvestChildPreviousYear))*(double)100.0)/double.Parse(totalInvestChildPreviousYear);
					evolution=tempEvol.ToString();										
				}					
			}
		}
		
		
		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats avec
		/// Investissement pour un total univers ou march� ou famille, ou un annonceur de r�f�rence ou concurrent,
		/// par m�dia ou cat�gorie ou support
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="indexTabRow">index ligne du tableau</param>
		/// <param name="idVehicle">identifiant m�dia</param>
		/// <param name="Vehicle">lib�ll� m�dia</param>		
		/// <param name="plurimedia">vrai si s�lection plurim�dia</param>
		/// <param name="Invest">investissement en Keuros soit pour un total univers ou march� ou famille, 
		/// ou un annonceur de r�f�rence ou concurrent</param>
		/// <param name="PDM">Part de march�</param>
		/// <param name="Evolution">Evolution p�riode N par rapport � N-1</param>
		/// <param name="comparisonCriterion">total march� ou famille ou univers</param>
		/// <param name="investmentType">investissement total famille ou march� ou univers, ou pour un annonceur de r�f�rence ou concurrent</param>
		/// <param name="preformatedMediaDetail">niveau de d�tail m�dia</param>
		/// <returns>tableau de r�sultats</returns>		
		private static object[,] FillTabInvestPdmEvol(WebSession webSession,object[,] tab,int indexTabRow,string idVehicle,string Vehicle,bool plurimedia,string Invest,string PDM,string Evolution,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType investmentType,CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			return FillTabInvestPdmEvol(webSession,tab,indexTabRow,idVehicle,Vehicle,"","","","","","",plurimedia,Invest,PDM,Evolution,comparisonCriterion,investmentType,preformatedMediaDetail);
		}
		
		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats avec
		/// Investissement pour un total univers ou march� ou famille, ou un annonceur de r�f�rence ou concurrent,
		/// par m�dia ou cat�gorie ou support
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="indexTabRow">index ligne du tableau</param>
		/// <param name="idVehicle">identifiant m�dia</param>
		/// <param name="Vehicle">lib�ll� m�dia</param>
		/// <param name="idCategory">identifiant cat�gorie</param>
		/// <param name="Category">lib�ll� cat�gorie</param>
		/// <param name="idMedia">identifiant support</param>
		/// <param name="Media">lib�ll� support</param>
		/// <param name="idRefOrCompetAdvertiser">identifiant annonceur de r�f�rence ou concurrent</param>
		/// <param name="RefOrCompetAdvertiser">lib�ll� annonceur de r�f�rence ou concurrent</param>
		/// <param name="plurimedia">vrai si s�lection plurim�dia</param>
		/// <param name="Invest">investissement en Keuros soit pour un total univers ou march� ou famille, 
		/// ou un annonceur de r�f�rence ou concurrent</param>
		/// <param name="PDM">Part de march�</param>
		/// <param name="Evolution">Evolution p�riode N par rapport � N-1</param>
		/// <param name="comparisonCriterion">total march� ou famille ou univers</param>
		/// <param name="investmentType">investissement total famille ou march� ou univers, ou pour un annonceur de r�f�rence ou concurrent</param>
		/// <param name="preformatedMediaDetail">niveau de d�tail m�dia</param>
		/// <returns>tableau de r�sultats</returns>		
		private static object[,] FillTabInvestPdmEvol(WebSession webSession,object[,] tab,int indexTabRow,string idVehicle,string Vehicle,string idCategory,string Category,string idMedia,string Media,string idRefOrCompetAdvertiser,string RefOrCompetAdvertiser,bool plurimedia,string Invest,string PDM,string Evolution,TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparisonCriterion,TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType investmentType,CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			/*Remplit chaqe colonne de la ligne courante du tableau*/
			
			//colonne ID media 
			if(WebFunctions.CheckedText.IsStringEmpty(idVehicle.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.ID_VEHICLE_COLUMN_INDEX] = idVehicle;
			//colonne lib�ll� media 
			if(WebFunctions.CheckedText.IsStringEmpty(Vehicle.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]=Vehicle;	
			//colonne ID categorie 
			if(WebFunctions.CheckedText.IsStringEmpty(idCategory.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.ID_CATEGORY_COLUMN_INDEX] = idCategory;
			//colonne lib�ll� cat�gorie 
			if(WebFunctions.CheckedText.IsStringEmpty(Category.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]=Category;	
			//colonne ID support
			if(WebFunctions.CheckedText.IsStringEmpty(idMedia.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.ID_MEDIA_COLUMN_INDEX] = idMedia;
			//colonne lib�ll� support 
			if(WebFunctions.CheckedText.IsStringEmpty(Media.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]= Media;	
			//PDM 
			if(WebFunctions.CheckedText.IsStringEmpty(PDM.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]=PDM;	
			//EVOLUTION p�riode N/N-1
			if(WebFunctions.CheckedText.IsStringEmpty(Evolution.Trim()))tab[indexTabRow,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]=Evolution;

			if(investmentType==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType.total){
				//investissement ann�e N pour total univers ou march� ou famille
				if(WebFunctions.CheckedText.IsStringEmpty(Invest.Trim())){
					//Remplit les investissement totaux pour la famille ou l'ensemble du march�
					if(comparisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal || comparisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal){
						switch(preformatedMediaDetail){
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle :														
								if(comparisonCriterion==CustomerSessions.ComparisonCriterion.marketTotal){
									if(plurimedia)tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]= Invest;
									else tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]= Invest;
								}
								else {
									if(plurimedia)tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]= Invest;
									else tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]= Invest;
								}
								break;													
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory :															
								if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal)											
									tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]= Invest;										
								else tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]= Invest;																	
								break;
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia :
								if(webSession.ComparaisonCriterion==WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal)											
									tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]= Invest;										
								else tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]= Invest;																					
								break;
							default :
								throw (new WebExceptions.IndicatorMediaStrategyException(TNS.AdExpress.Web.Core.Translation.GestionWeb.GetWebWord(1237,webSession.SiteLanguage)));		
						}													
					//Remplit les investissement totaux pour l'univers
					} else if(comparisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
						switch(preformatedMediaDetail){
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:							
								if(plurimedia) {
									tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]= Invest;
								}
								else tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]= Invest;
								break;							
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
								tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]= Invest;
							break;		
							case CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
								tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]= Invest;														
								break;		
							default :
								throw (new WebExceptions.IndicatorMediaStrategyException(TNS.AdExpress.Web.Core.Translation.GestionWeb.GetWebWord(1237,webSession.SiteLanguage)));		
						}
					}
				}
			}
			else if (investmentType==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType.advertiser){
				//investissement ann�e N pour annonceur de r�f�rence ou concurrent
				if(WebFunctions.CheckedText.IsStringEmpty(Invest.ToString().Trim())){
					if(WebFunctions.CheckedText.IsStringEmpty(RefOrCompetAdvertiser.Trim()))
					tab[indexTabRow,ConstResults.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = RefOrCompetAdvertiser.ToString();
					if(WebFunctions.CheckedText.IsStringEmpty(idRefOrCompetAdvertiser.ToString().Trim()))
					tab[indexTabRow,ConstResults.MediaStrategy.ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = idRefOrCompetAdvertiser.ToString();																													
					if(plurimedia) {
						tab[indexTabRow,ConstResults.MediaStrategy.TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]= Invest;
					}
					else tab[indexTabRow,ConstResults.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]= Invest;							
				}
			}

			return tab;
		}
		#endregion	

		#region Traitement premier annonceur ou r�f�rence
		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats avec
		/// Investissement les donn�es du premier annonceur ou r�f�rence de chauqe m�dia.
		/// </summary>
		/// <param name="webSession">session</param>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="comparisonCriterion">crit�re de comparaison</param>
		/// <param name="TotalMarketOrSectorInvest">investissement total</param>
		/// <param name="indexTabRow">index des lignes du tableau r�sultas</param>
		/// <param name="increment">vrai si change de ligne</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillTabFisrtElmt(WebSession webSession, object[,] tab,CustomerSessions.ComparisonCriterion comparisonCriterion,string TotalMarketOrSectorInvest,ref int indexTabRow,bool increment ){

			//Remplit la ligne courante avec le lib�ll� et l'investissment du premier annonceur,	pour le total march� et plurim�dia 					
			DataSet dsPluri = IndicatorDataAccess.GetPluriMediaStrategy1stElmntData(webSession, FrameWorkResultConstantes.MotherRecap.ElementType.advertiser,comparisonCriterion);
			if(dsPluri!=null && dsPluri.Tables[0] !=null && dsPluri.Tables[0].Rows.Count==1 && WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorInvest) && (double.Parse(TotalMarketOrSectorInvest)>(double)0.0))
				tab=FillTabFisrtElmt(tab,indexTabRow,ConstResults.MotherRecap.ElementType.advertiser,"","",dsPluri.Tables[0],true);												
			//Remplit la ligne courante avec le lib�ll� et l'investissment de la  premiere r�f�rence,	pour le total march� et plurim�dia 						
			dsPluri = IndicatorDataAccess.GetPluriMediaStrategy1stElmntData(webSession, FrameWorkResultConstantes.MotherRecap.ElementType.product,comparisonCriterion);
			if(dsPluri!=null && dsPluri.Tables[0] !=null && dsPluri.Tables[0].Rows.Count==1 && WebFunctions.CheckedText.IsStringEmpty(TotalMarketOrSectorInvest) && (double.Parse(TotalMarketOrSectorInvest)>(double)0.0))
			tab=FillTabFisrtElmt(tab,indexTabRow,ConstResults.MotherRecap.ElementType.product,"","",dsPluri.Tables[0],true);						
			if(increment)indexTabRow++; //ligne suivante

			return tab;
		}

		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats avec
		/// Investissement les donn�es du premier annonceur ou r�f�rence de chauqe m�dia.
		/// </summary>	
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="totalInvestChild">Investissement du m�dia fils</param>		
		/// <param name="dt1stAdvertiser">Table de donn�es premier annonceur</param>
		/// <param name="dt1stProduct">Table de donn�es premiere r�f�rence</param>
		/// <param name="idElement">identifiant du m�dia</param>
		/// <param name="LabelElement">libell� du m�dia</param>
		/// <param name="inMediaAlreadyUsedArr">M�dias d�j� trait�s</param>
		/// <param name="indexTabRow">index des lignes du tableau r�sultas</param>
		/// <param name="increment">vrai si la ligne doit �tre incr�ment�e</param>
		/// <param name="isPluri">vrai si plurim�dia</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillTabFisrtElmt(object[,] tab,ref string totalInvestChild,DataTable dt1stAdvertiser,DataTable dt1stProduct,string LabelElement,string idElement,ref ArrayList inMediaAlreadyUsedArr,ref int indexTabRow,bool increment,bool isPluri){
			//Remplit la ligne courante avec le lib�ll� et l'investissment du 1er annonceur, pour le total, et par m�dia 						
			if(WebFunctions.CheckedText.IsStringEmpty(totalInvestChild) && (double.Parse(totalInvestChild)>(double)0.0) && dt1stAdvertiser!=null && dt1stAdvertiser.Rows.Count>0)				
				tab=FillTabFisrtElmt(tab,indexTabRow,ConstResults.MotherRecap.ElementType.advertiser,LabelElement,idElement,dt1stAdvertiser,isPluri);
			//Remplit la ligne courante avec le lib�ll� et l'investissment du 1ere r�f�rence, pour le total, et par m�dia 							
			if(WebFunctions.CheckedText.IsStringEmpty(totalInvestChild) && (double.Parse(totalInvestChild)>(double)0.0) && dt1stProduct!=null && dt1stProduct.Rows.Count>0)
				tab=FillTabFisrtElmt(tab,indexTabRow,ConstResults.MotherRecap.ElementType.product,LabelElement,idElement,dt1stProduct,isPluri);

			if(!inMediaAlreadyUsedArr.Contains(idElement)) inMediaAlreadyUsedArr.Add(idElement);
			if(increment)indexTabRow++; //ligne suivante

			return tab;
		}	
		
		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats concernant les totaux
		/// univers ou famille ou march� par les lib�ll�s des annonceurs de r�f�rence
		/// ou concurrentsainsi que leurs investissements en Keuros.
		/// </summary>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="indexTabRow">index ligne du tableau</param>		
		/// <param name="elementType">r�f�rence ou annonceur</param>
		/// <param name="dtFirstElmt">table de donn�es</param>
		/// <param name="idMedia">identifiant du m�dia (m�dia ou cat�gorie ou support)</param>
		/// <param name="idMediaName">libell� du m�dia</param>
		/// <param name="pluri">bool�en pour pr�ciser s'il s'agit d'une s�lection mono ou plurim�dia</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillTabFisrtElmt(object[,] tab,int indexTabRow,ConstResults.Novelty.ElementType elementType,string idMediaName,string idMedia,DataTable dtFirstElmt,bool pluri){
			#region variables locales
			string strExpr="";
			string strSort ="";
			DataRow[] foundRows=null;
			string tempInvest="";		
			#endregion
			//Remplit la ligne courante avec le lib�ll� et l'investissment du 1er annonceur ou r�f�rence, pour le total march� ou famille ou univers, et par m�dia (m�dia ou cat�gorie ou support)
			if(dtFirstElmt!=null && dtFirstElmt.Rows.Count>0 && dtFirstElmt.Columns.Contains("total_N") ){								
				//Plurimedia
				if(pluri)foundRows = dtFirstElmt.Select(); 				
				else{
					//MonoMedia
					strExpr = " "+idMediaName+"="+idMedia;
					strSort = "total_N  ASC";	
					foundRows = dtFirstElmt.Select( strExpr, strSort, DataViewRowState.OriginalRows);
				}						
				if(foundRows!=null && foundRows.Length>0 && foundRows[0]!=null ){													
//					tempInvest = String.Format("{0:### ### ### ### ##0.##}",(double.Parse(foundRows[0]["total_N"].ToString())/(double)1000));
					tempInvest = foundRows[0]["total_N"].ToString();
					//Insertion des donn�es dans la ligne courante
					if(WebFunctions.CheckedText.IsStringEmpty(tempInvest.ToString().Trim())){
						if(dtFirstElmt.Columns.Contains("advertiser")){
							if(pluri)tab = FillTabFirstElmt(tab,indexTabRow,"",foundRows[0]["advertiser"].ToString(),tempInvest,ConstResults.MotherRecap.ElementType.advertiser);							
							else tab = FillTabFirstElmt(tab,indexTabRow,foundRows[0]["id_advertiser"].ToString(),foundRows[0]["advertiser"].ToString(),tempInvest,ConstResults.MotherRecap.ElementType.advertiser);							
						}else if(dtFirstElmt.Columns.Contains("product")){
							if(pluri)tab = FillTabFirstElmt(tab,indexTabRow,"",foundRows[0]["product"].ToString(),tempInvest,ConstResults.MotherRecap.ElementType.product);
							else tab = FillTabFirstElmt(tab,indexTabRow,foundRows[0]["id_product"].ToString(),foundRows[0]["product"].ToString(),tempInvest,ConstResults.MotherRecap.ElementType.product);
						}
					}
				}			
			}			
			return tab;
		}

		/// <summary>
		/// Remplit chaque ligne du tableau de r�sultats concernant les totaux
		/// univers ou famille ou march� par les lib�ll�s des annonceurs de r�f�rence
		/// ou concurrentsainsi que leurs investissements en Keuros.
		/// </summary>
		/// <param name="tab">tableau de r�sultats</param>
		/// <param name="indexTabRow">index ligne du tableau</param>
		/// <param name="idElement">identifiant annonceur de r�f�rence ou concurrent</param>
		/// <param name="LabelElement">lib�ll� annonceur de r�f�rence ou concurrent</param>
		/// <param name="InvestElement">investissement annonceur de r�f�rence ou concurrent</param>
		/// <param name="elementType">r�f�rence ou annonceur</param>
		/// <returns>tableau de r�sultats</returns>
		private static object[,] FillTabFirstElmt(object[,] tab,int indexTabRow, string idElement,string LabelElement, string InvestElement,ConstResults.Novelty.ElementType elementType){
			if(ConstResults.Novelty.ElementType.advertiser==elementType){
				//ID premier annonceur
				if(WebFunctions.CheckedText.IsStringEmpty(idElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.ID_FIRST_ADVERT_COLUMN_INDEX] = idElement;
				//Lib�ll� premier annonceur
				if(WebFunctions.CheckedText.IsStringEmpty(LabelElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX] = LabelElement;
				//investissement premiere annonceur
				if(WebFunctions.CheckedText.IsStringEmpty(InvestElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX] = InvestElement;
			}else if(ConstResults.Novelty.ElementType.product==elementType){
				//ID premier r�f�rence
				if(WebFunctions.CheckedText.IsStringEmpty(idElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.ID_FIRST_REF_COLUMN_INDEX] = idElement;
				//Lib�ll� premier r�f�rence
				if(WebFunctions.CheckedText.IsStringEmpty(LabelElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX] = LabelElement;
				//investissement premiere r�f�rence
				if(WebFunctions.CheckedText.IsStringEmpty(InvestElement.Trim()))
				tab[indexTabRow,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX] = InvestElement;
			}
			else {
				throw (new Exceptions.IndicateurNoveltyRulesException("Impossible d'identifier le type d'�l�ments (produits ou annonceurs) � afficher." ));
			}

			return tab;
		}

		

		/// <summary>
		/// R�cup�re les tables de donn�es pour les 1ers annonceurs ou r�f�rences par m�dia
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dt1stProductByVeh">table de donn�es 1ere r�f�rence par m�dia </param>
		/// <param name="dt1stAdvertiserByVeh">table de donn�es 1er annonceur par m�dia</param>
		/// <param name="dt1stProductByCat">table de donn�es 1ere r�f�rence par cat�gorie</param>
		/// <param name="dt1stAdvertiserByCat">table de donn�es 1er annonceur par cat�gorie</param>
		/// <param name="dt1stProductByMed">table de donn�es 1ere r�f�rence par support</param>
		/// <param name="dt1stAdvertiserByMed">table de donn�es 1ere r�f�rence par annonceur</param>
		/// <param name="comparisonCriterion">crit�re de comparaisonn (univers,march�,famille)</param>
		/// <param name="mediaLevel">niveau m�dia</param>
		private static void Get1stElmtDataTbleByMedia(WebSession webSession, ref DataTable dt1stProductByVeh,ref DataTable dt1stAdvertiserByVeh, ref DataTable dt1stProductByCat,ref DataTable dt1stAdvertiserByCat, ref DataTable dt1stProductByMed,ref DataTable dt1stAdvertiserByMed,CustomerSessions.ComparisonCriterion comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel mediaLevel){

			#region variables locales
			//groupe de donn�es pour premiere r�f�rence 
			DataSet dsTotalFirstProduct = null;			
			//groupe de donn�es pour premier annonceur 
			DataSet dsTotalFirstAdvertiser = null;				
			#endregion
						
			//Donn�es pour premi�re r�f�rence par m�dia (vehicle)
			dsTotalFirstProduct = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.vehicleLevel);
			if(dsTotalFirstProduct!=null && dsTotalFirstProduct.Tables[0]!=null && dsTotalFirstProduct.Tables[0].Rows.Count>0)
				dt1stProductByVeh = dsTotalFirstProduct.Tables[0];

			//Donn�es pour premier annonceur par m�dia (vehicle)
			dsTotalFirstAdvertiser = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.vehicleLevel);
			if(dsTotalFirstAdvertiser!=null && dsTotalFirstAdvertiser.Tables[0]!=null && dsTotalFirstAdvertiser.Tables[0].Rows.Count>0)
				dt1stAdvertiserByVeh = dsTotalFirstAdvertiser.Tables[0];
			if( (FrameWorkResultConstantes.MediaStrategy.MediaLevel.categoryLevel==mediaLevel) || (FrameWorkResultConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel)){
				//Donn�es pour premi�re r�f�rence par m�dia (cat�gorie)
				dsTotalFirstProduct = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.categoryLevel);
				if(dsTotalFirstProduct!=null && dsTotalFirstProduct.Tables[0]!=null && dsTotalFirstProduct.Tables[0].Rows.Count>0)
					dt1stProductByCat = dsTotalFirstProduct.Tables[0];

				//Donn�es pour premier annonceur par m�dia (cat�gorie)
				dsTotalFirstAdvertiser = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.categoryLevel);
				if(dsTotalFirstAdvertiser!=null && dsTotalFirstAdvertiser.Tables[0]!=null && dsTotalFirstAdvertiser.Tables[0].Rows.Count>0)
					dt1stAdvertiserByCat = dsTotalFirstAdvertiser.Tables[0];
			}		
			if(FrameWorkResultConstantes.MediaStrategy.MediaLevel.mediaLevel==mediaLevel){
				//Donn�es pour premi�re r�f�rence par support
				dsTotalFirstProduct = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.mediaLevel);
				if(dsTotalFirstProduct!=null && dsTotalFirstProduct.Tables[0]!=null && dsTotalFirstProduct.Tables[0].Rows.Count>0)
					dt1stProductByMed = dsTotalFirstProduct.Tables[0];
			
				//Donn�es pour premier annonceur par support
				dsTotalFirstAdvertiser = IndicatorDataAccess.GetMediaStrategy1stElmntData(webSession,TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.advertiser,comparisonCriterion,FrameWorkResultConstantes.MediaStrategy.MediaLevel.mediaLevel);
				if(dsTotalFirstAdvertiser!=null && dsTotalFirstAdvertiser.Tables[0]!=null && dsTotalFirstAdvertiser.Tables[0].Rows.Count>0)
					dt1stAdvertiserByMed = dsTotalFirstAdvertiser.Tables[0];
			}									
		}
		#endregion

		#region Initialisation tableau r�sultats
		/// <summary>
		/// Initialise le tableau de r�sultas des r�partion m�dia en investissments.
		/// </summary>
		/// <param name="webSession">session du client </param>
		/// <param name="dtTotal">table de donn�es total famille ou march�</param>
		/// <param name="dtAdvertiser">table de donn�es annonceurs de r�f�rences ou concurrents</param>		
		/// <param name="VehicleAccessList">m�dia en acc�s</param>
		/// <param name="NB_MAX_COLUMNS">nombre max de colonne dans tableau</param>
		/// <returns>instance tableau de r�sultats</returns>
		public static object[,] TabInstance(WebSession webSession, System.Data.DataTable dtTotal,System.Data.DataTable dtAdvertiser,string VehicleAccessList,int NB_MAX_COLUMNS){
			
			#region variables
			//tableau de r�sultats
			object[,] tab =null;					
			//identifiant m�dia pr�c�dent
			string OldIdVehicle="0";			
			//nombre de m�dia (vehicle)
			int nbVehicle=0;					
			//collection identifiant pr�c�dentes cat�gories
			ArrayList OldIdCategoryArr = new ArrayList();			
			//nombre de cat�gories
			int nbCategory=0;								
			//collection identifiant pr�c�dents supports
			ArrayList OldIdMediaArr = new ArrayList();
			//nombre de supports
			int nbMedia=0;	
			//nombre  annonceurs de r�f�rences ou concurrents
			int nbAdvertiser=0;	
			//collection des annonceurs d�j� trait�s
			ArrayList OldIdAdvertiserArr = new ArrayList();		
			//nombre maximal de lignes
			int nbMaxLines = 0;	
			#endregion
			
			#region instanciation du tableau de r�sultats
			/*On instancie le tableau de r�sultats pour strat�gie m�dia
			 */
			if(dtTotal!=null &&  dtTotal.Rows.Count>0){				 			
				//calcule nombre maximal de lignes
				switch(webSession.PreformatedMediaDetail){
						//s�lection plurimedia
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle: 
						//nombre de m�dia (vehicle)
						foreach(DataRow curRow in  dtTotal.Rows){							
							if(dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle)!=int.Parse(curRow["id_vehicle"].ToString())) ){
								nbVehicle++;
							}
							OldIdVehicle=curRow["id_vehicle"].ToString();
						}	
						if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0){
							//nombre d'annonceurs
							foreach(DataRow currRow in dtAdvertiser.Rows){
								if(dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString())){
									nbAdvertiser++;
									OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
								}								
							}
						}
						//calcule du nombre maximal de lignes
						if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList) && DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList))
							nbMaxLines=2+nbAdvertiser+(2+nbAdvertiser)*nbVehicle;
						else nbMaxLines=(2+nbAdvertiser)*nbVehicle;
						break;
						//s�lection monomedia et cat�gories
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
						//nombre de m�dia (vehicle)
						foreach(DataRow curRow in  dtTotal.Rows){
							if(dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle)!=int.Parse(curRow["id_vehicle"].ToString())) ){
								nbVehicle++;
							}
							//nombre de cat�gories
							if(dtTotal.Columns.Contains("id_category") && !OldIdCategoryArr.Contains(curRow["id_category"].ToString())){
								nbCategory++;
								OldIdCategoryArr.Add(curRow["id_category"].ToString());
							}
							OldIdVehicle=curRow["id_vehicle"].ToString();
						}	
						if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0){
							//nombre d'annonceurs
							foreach(DataRow currRow in dtAdvertiser.Rows){
								if(dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString())){
									nbAdvertiser++;
									OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
								}								
							}
						}
						//calcule du nombre maximal de lignes
						if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList) && DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList))
							nbMaxLines=2+nbAdvertiser+((2+nbAdvertiser)*nbCategory)*nbVehicle;
						else nbMaxLines=2+nbAdvertiser+ (2+nbAdvertiser)*nbCategory;
						break;
						//s�lection monomedia et cat�gories et supports
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
						//nombre de m�dia (vehicle)
						foreach(DataRow curRow in  dtTotal.Rows){
							if(dtTotal.Columns.Contains("id_vehicle") && (int.Parse(OldIdVehicle)!=int.Parse(curRow["id_vehicle"].ToString()))){
								nbVehicle++;
							}
							//nombre de cat�gories
							if(dtTotal.Columns.Contains("id_category") && !OldIdCategoryArr.Contains(curRow["id_category"].ToString())){
								nbCategory++;
								OldIdCategoryArr.Add(curRow["id_category"].ToString());
							}
							//nombre de supports
							if(dtTotal.Columns.Contains("id_media") && !OldIdMediaArr.Contains(curRow["id_media"].ToString())){
								nbMedia++;
								OldIdMediaArr.Add(curRow["id_media"].ToString());
							}
							OldIdVehicle=curRow["id_vehicle"].ToString();
						}	
						if(dtAdvertiser!=null && dtAdvertiser.Rows.Count>0){
							//nombre d'annonceurs
							foreach(DataRow currRow in dtAdvertiser.Rows){
								if(dtAdvertiser.Columns.Contains("id_advertiser") && !OldIdAdvertiserArr.Contains(currRow["id_advertiser"].ToString())){
									nbAdvertiser++;
									OldIdAdvertiserArr.Add(currRow["id_advertiser"].ToString());
								}								
							}
						}
						//calcule du nombre maximal de lignes
						if(WebFunctions.CheckedText.IsStringEmpty(VehicleAccessList) && DBClassificationConstantes.Vehicles.names.plurimedia ==(DBClassificationConstantes.Vehicles.names)int.Parse(VehicleAccessList))
							nbMaxLines=2 + nbAdvertiser + (2+nbAdvertiser+ (2+nbAdvertiser +(2+nbAdvertiser)*nbMedia)*nbCategory)*nbVehicle;
						else nbMaxLines=2+nbAdvertiser+ (2+nbAdvertiser +(2+nbAdvertiser)*nbMedia)*nbCategory;
						break;
					default :
						throw (new WebExceptions.IndicatorMediaStrategyException(TNS.AdExpress.Web.Core.Translation.GestionWeb.GetWebWord(1237,webSession.SiteLanguage)));		
				}
				OldIdVehicle="0";				
			}			
			//cr�ation du tableau 
			tab = new object[nbMaxLines,NB_MAX_COLUMNS];				
			#endregion

			return tab;
		}
		#endregion

		#region Niveau M�dia s�lectionn�

		#region D�termine le niveau support s�lectionn�
		/// <summary>
		/// D�termine le niveau M�dia (M�dia ou cat�gorie ou support)
		/// </summary>
		/// <param name="webSession">session client</param>		
		/// <returns>niveau m�dia</returns>
		private static FrameWorkResultConstantes.MediaStrategy.MediaLevel SwitchMedia(WebSession webSession){						
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
					return FrameWorkResultConstantes.MediaStrategy.MediaLevel.mediaLevel;					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					 return FrameWorkResultConstantes.MediaStrategy.MediaLevel.categoryLevel;					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
					return  FrameWorkResultConstantes.MediaStrategy.MediaLevel.vehicleLevel;					
				default :
					 throw (new WebExceptions.IndicatorMediaStrategyException(TNS.AdExpress.Web.Core.Translation.GestionWeb.GetWebWord(1237,webSession.SiteLanguage)));		
			}						
		}
		/// <summary>
		/// Un traitement peut �tre affectu� si s�lection m�dia est au niveau cat�gorie
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si niveau cat�gorie</returns>
		internal static bool TreatCategory(WebSession webSession){
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:								
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
					return true;							
				default :
					return false;	
			}	
		}
		/// <summary>
		/// Un traitement peut �tre affectu� si on la session contient le niveau support
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si niveau cat�gorie</returns>
		internal static bool TreatMedia(WebSession webSession){
			switch(webSession.PreformatedMediaDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:												
					return true;							
				default :
					return false;	
			}	
		}
		
		#endregion
		
		#endregion

		#endregion
	}
}
