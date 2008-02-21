using System;
using TNS.AdExpress.Web.Core.Sessions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Rules.Customer;
using System.Collections;
using System.Data;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Chargement de liste : annonceur, famille, classe, groupe, marques
	/// </summary>
	public class AdvertiserListDataAccess{
			
		#region Variables
		/// <summary>
		/// 
		/// </summary>
		private DataSet _dsListAdvertiser;

		#endregion

		#region Constructeur
		/// <summary>
		/// Charge la liste des annonceurs qui peuvent être sélectionnés
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="newText">Texte</param>
		/// <param name="radioButtonHoldingCompany"></param>
		/// <param name="radiobuttonAdvertiser"></param>
		/// <param name="radiobuttonProduct"></param>
		/// <param name="radiobuttonAll"></param>
		/// <param name="listHoldingCompany"></param>
		/// <param name="listAdvertiser"></param>
		/// <param name="listProduct"></param>
		public AdvertiserListDataAccess(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, string newText,bool radioButtonHoldingCompany,
			bool radiobuttonAdvertiser,bool radiobuttonProduct,bool radiobuttonAll,string listHoldingCompany,string listAdvertiser,
			string listProduct) {
			
			Right right=webSession.CustomerLogin;
			string sql="";				
					
			#region requête
			if(radioButtonHoldingCompany){
				sql+=" select distinct hc.id_holding_company,holding_company,ad.id_advertiser,advertiser";
			}
			if(radiobuttonAdvertiser){
				sql+=" select ad.id_advertiser,advertiser, pr.id_product, product";
			}
			if(radiobuttonProduct){
				sql+="select id_product,product";
			}

			if(radiobuttonAll){
				sql+=" select hc.id_holding_company,holding_company,ad.id_advertiser,";
				sql+=" advertiser, pr.id_product, product";
			}
				
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".holding_company hc,"+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".advertiser ad,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector sc,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_ gr,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment sg,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".product pr";		
			sql+=" where hc.id_holding_company=ad.id_holding_company";
			sql+=" and pr.id_advertiser=ad.id_advertiser";	
			sql+=" and sc.id_subsector=gr.id_subsector";
			sql+=" and gr.id_group_=sg.id_group_";
			sql+=" and sg.id_segment=pr.id_segment";


			sql+=" and hc.id_language=ad.id_language";
			sql+=" and pr.id_language=ad.id_language";
			sql+=" and sc.id_language=gr.id_language";
			sql+=" and gr.id_language=sg.id_language";
			sql+=" and sg.id_language=pr.id_language";

			sql+=" and hc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";


			sql+=" and hc.id_language="+webSession.SiteLanguage+"";
			sql+=" and ad.id_language="+webSession.SiteLanguage+"";
			sql+=" and sc.id_language="+webSession.SiteLanguage+"";
			sql+=" and gr.id_language="+webSession.SiteLanguage+"";
			sql+=" and sg.id_language="+webSession.SiteLanguage+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";
					
			#region Droits clients
			//Droits clients partie produit
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) {
				sql+=" ";	 
			}

			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) {
				sql+=" and ( ";}
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				){
				sql+="( ";}
					
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)sql+="  sc.id_sector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess)+")";
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)
				sql+=" or sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 )
				sql+=" sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0))
				sql+=" or gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0)
				sql+="  gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";

					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ))
				sql+=" or sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0)
				sql+="  sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";


			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				){
				sql+=" ) ";}

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0)sql+=" and sc.id_sector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0)sql+=" and sc.id_subsector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0)sql+=" and gr.id_group_ not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0)sql+=" and sg.id_segment not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException)+")";
					

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) {
				sql+=" ";	 
			}
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) {
				sql+=" ) ";}

						
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==00) {
				sql+=" ";
			}
				//Droits clients partie advertiser
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) {
				sql+=" and ( ";}
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				){
				sql+="( ";}

					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
				sql+="  hc.id_holding_company in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess)+")";

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
				sql+=" or ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0)
				sql+=" ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				){
				sql+=" ) ";}
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0)sql+=" and hc.id_holding_company not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0)sql+=" and ad.id_advertiser not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException)+")";
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==0) {
				sql+=" ";
			}
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) {
				sql+=" ) ";}
			#endregion

			
			// Cas Holding Company
			if(radioButtonHoldingCompany){	
				if(newText.Length>0){
					sql+=" and ( holding_company like upper('%"+newText+"%')";
					if(listHoldingCompany.Length!=0) {
						sql+=" or hc.id_holding_company in("+listHoldingCompany+")";
					}
				}
				else{
					if(listHoldingCompany.Length!=0) {
						sql+="and ( hc.id_holding_company in("+listHoldingCompany+")";
					}
				}
				
				if(listAdvertiser.Length!=0 && (listHoldingCompany.Length!=0 || newText.Length>0)){
					sql+=" or ad.id_advertiser in ("+listAdvertiser+") ";
				}
				else if(listAdvertiser.Length!=0){
					sql+=" and ( ad.id_advertiser in ("+listAdvertiser+") ";
				}
				sql+=")";
				sql+=" order by  holding_company, advertiser";
			}
			// Cas Advertiser
			if(radiobuttonAdvertiser){		
				
				if(newText.Length>0){		
					sql+=" and ( advertiser like upper('%"+newText+"%')";
					if(listAdvertiser.Length!=0) {
						sql+=" or ad.id_advertiser in ("+listAdvertiser+")";
					}
				}else{
					if(listAdvertiser.Length!=0) {
						sql+=" and ( ad.id_advertiser in ("+listAdvertiser+")";
					}
				}


				if(listProduct.Length!=0 && (listAdvertiser.Length!=0 || newText.Length>0)){
					sql+=" or pr.id_product in("+listProduct+")";
				}
				else if(listProduct.Length!=0){
					sql+=" and ( pr.id_product in("+listProduct+")";
				}
				
					
			
				sql+=" ) ";
				sql+=" order by advertiser,product";
			}
			// Cas références
			if(radiobuttonProduct){

				
				if(newText.Length>0){
					sql+=" and ( product like upper('%"+newText+"%')";
					if(listProduct.Length!=0){
						sql+=" or pr.id_product in("+listProduct+")";
					}
				}
				else{
					if(listProduct.Length!=0){
						sql+=" and ( pr.id_product in("+listProduct+")";
					}
				}
				
				sql+=" ) ";
				sql+=" order by product";
			}
			// Cas tous
			if(radiobuttonAll){
				sql+="  and ( holding_company like upper ('%"+newText+"%')";
				sql+=" or product like upper('%"+newText+"%')";
				sql+=" or advertiser like upper('%"+newText+"%'))";
				
				if(listHoldingCompany.Length!=0) {
					sql+=" or hc.id_holding_company in("+listHoldingCompany+")";
				}
				if(listAdvertiser.Length!=0) {
					sql+=" or ad.id_advertiser in ("+listAdvertiser+")";
				}
				if(listProduct.Length!=0){
					sql+=" or pr.id_product in("+listProduct+")";
				}
				sql+=" ) ";
				sql+=" order by holding_company, advertiser, product";
			}	
		
			#endregion

			#region Execution de la requête
			try{
				LoadDataSet(webSession,sql);
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.AdvertiserListException("Impossible de charger le dataset",err));
			}
			#endregion

		}
		
		/// <summary>
		/// Charge la liste des famille classe groupe
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="newText">Texte</param>
		/// <param name="radioButtonSector">bool pour dire si l'on a sélectionné une famille</param>
		/// <param name="radioButtonSubSector">bool pour dire si l'on a sélectionné une classe</param>
		/// <param name="radioButtonGroup">bool pour dire si l'on a sélectionné un group</param>
		/// <param name="radioButtonSegment"></param>
		/// <param name="listSector"></param>
		/// <param name="listSubSector"></param>
		/// <param name="listGroup"></param>
		/// <param name="listSegment"></param>
		public AdvertiserListDataAccess(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, string newText,bool radioButtonSector,
			bool radioButtonSubSector,bool radioButtonGroup,bool radioButtonSegment,string listSector,string listSubSector,string listGroup,string listSegment){
		
			Right right=webSession.CustomerLogin;
			string sql="";				
					
			#region requête
			if(radioButtonSector){
				sql+=" select distinct sec.id_sector,sector,sc.id_subsector,subsector";
			}
			if(radioButtonSubSector){
				sql+=" select distinct sc.id_subsector,subsector, gr.id_group_, group_";
			}
			if(radioButtonGroup){
				sql+="select distinct gr.id_group_,group_,sg.id_segment,segment";
			}
			if (radioButtonSegment) {
				sql += "select distinct sg.id_segment,segment";
			}


							
			//	sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".holding_company hc,"+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".advertiser ad,";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector sc,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".sector sec,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_ gr,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment sg,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".product pr";		
			//	sql+=" where hc.id_holding_company=ad.id_holding_company";		
			sql+=" where sec.id_sector=sc.id_sector";	
			sql+=" and sc.id_subsector=gr.id_subsector";
			sql+=" and gr.id_group_=sg.id_group_";
			sql+=" and sg.id_segment=pr.id_segment";


			//sql+=" and hc.id_language=ad.id_language";
			sql+=" and sec.id_language=sc.id_language";
			sql+=" and sc.id_language=gr.id_language";
			sql+=" and gr.id_language=sg.id_language";
			sql+=" and sg.id_language=pr.id_language";

			//	sql+=" and hc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sec.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";


			//	sql+=" and hc.id_language="+webSession.SiteLanguage+"";
			sql+=" and sec.id_language="+webSession.SiteLanguage+"";
			sql+=" and sc.id_language="+webSession.SiteLanguage+"";
			sql+=" and gr.id_language="+webSession.SiteLanguage+"";
			sql+=" and sg.id_language="+webSession.SiteLanguage+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";
					
			#region Droits clients
			//Droits clients partie produit
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) {
				sql+=" ";	 
			}

			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) {
				sql+=" and ( ";}
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				){
				sql+="( ";}
					
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)sql+="  sc.id_sector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess)+")";
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)
				sql+=" or sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 )
				sql+=" sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0))
				sql+=" or gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0)
				sql+="  gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";

					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ))
				sql+=" or sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0)
				sql+="  sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";


			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				){
				sql+=" ) ";}

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0)sql+=" and sc.id_sector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0)sql+=" and sc.id_subsector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0)sql+=" and gr.id_group_ not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0)sql+=" and sg.id_segment not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException)+")";
					

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) {
				sql+=" ";	 
			}
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) {
				sql+=" ) ";}

						
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
			//				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==0) {
			//				sql+=" ";
			//			}
			//				//Droits clients partie advertiser
			//			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) {
			//				sql+=" and ( ";}
			//					
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
			//				){
			//				sql+="( ";}
			//
			//					
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
			//				sql+="  hc.id_holding_company in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess)+")";
			//
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
			//				sql+=" or ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			//			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0)
			//				sql+=" ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
			//				){
			//				sql+=" ) ";}
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0)sql+=" and hc.id_holding_company not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException)+")";
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0)sql+=" and ad.id_advertiser not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException)+")";
			//					
			//			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
			//				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==0) {
			//				sql+=" ";
			//			}
			//			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
			//				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) {
			//				sql+=" ) ";}
			#endregion

			
			// Cas sector
			if(radioButtonSector){		
		
				if(newText.Length>0){
					sql+=" and ( sector like upper('%"+newText+"%')";
					if(listSector.Length!=0) {
						sql+=" or sec.id_sector in("+listSector+")";
					}
				}
				else{
					if(listSector.Length!=0) {
						sql+=" and ( sec.id_sector in("+listSector+")";
					}
				}
				if(listSubSector.Length!=0 && (listSector.Length!=0 || newText.Length>0)){
					sql+=" or sc.id_subsector in ("+listSubSector+") ";
				}
				else if(listSubSector.Length!=0){
					sql+=" and ( sc.id_subsector in ("+listSubSector+") ";
				}
				sql+=")";
				sql+=" order by  sector, subsector";
			}
			// Cas subsector
			if(radioButtonSubSector){		
				
				if(newText.Length>0){	
					sql+=" and ( subsector like upper('%"+newText+"%')";
					if(listSubSector.Length!=0) {
						sql+=" or sc.id_subsector in ("+listSubSector+")";
					}
				}else{
					if(listSubSector.Length!=0) {
						sql+=" and ( sc.id_subsector in ("+listSubSector+")";
					}
				}


				if(listGroup.Length!=0 && (listSubSector.Length!=0 || newText.Length>0)){
					sql+=" or gr.id_group_ in("+listGroup+")";
				}
				else if(listGroup.Length!=0){
					sql+=" and ( gr.id_group_ in("+listGroup+")";
				}
				sql+=" ) ";
				sql+=" order by subsector,group_";
			}
			// Cas groupe
			if(radioButtonGroup){
				if(newText.Length>0){
					sql+=" and ( group_ like upper('%"+newText+"%')";
					if(listGroup.Length!=0){
						sql+=" or gr.id_group_ in("+listGroup+")";
					}
				}else{
					if (listGroup.Length != 0)
					sql+=" and ( gr.id_group_ in("+listGroup+")";
				}

				if (listSegment.Length != 0 && (listGroup.Length != 0 || newText.Length > 0)) {
					sql += " or sg.id_segment in (" + listSegment + ") ";
				}
				else if (listSegment.Length != 0) {
					sql += " and (  sg.id_segment in (" + listSegment + ") ";
				}

				sql+=" ) ";
				sql+=" order by group_,segment";
			}

			// Cas variété
			if (radioButtonSegment) {
				if (newText.Length > 0) {
					sql += " and ( segment like upper('%" + newText + "%')";
					if (listSegment.Length != 0) {
						sql += " or sg.id_segment in(" + listSegment + ")";
					}
				}
				else {
					if (listSegment.Length != 0)
						sql += " and ( sg.id_segment in(" + listSegment + ")";
				}
				sql += " ) ";
				sql += " order by segment";
			}
			#endregion

			#region Execution de la requête
			try{
				LoadDataSet(webSession,sql);
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.AdvertiserListException("Impossible de charger le dataset",err));
			}
			#endregion
 			
		}

		/// <summary>
		///  Charge la liste des Marques qui peuvent être sélectionnés
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="newText">texte</param>
		/// <param name="radioButtonBrand">bouton radio marque</param>
		/// <param name="listBrand">liste des marques</param>
		public AdvertiserListDataAccess(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, string newText,bool radioButtonBrand,string listBrand){
			
			Right right=webSession.CustomerLogin;
			string sql="";				
					
			#region requête
			
			sql+=" select br.id_brand,brand, pr.id_product, product";			
				
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".holding_company hc,"+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".advertiser ad,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector sc,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_ gr,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment sg,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".brand br,";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".product pr";		
			sql+=" where hc.id_holding_company=ad.id_holding_company";
			sql+=" and pr.id_advertiser=ad.id_advertiser";
			sql+=" and pr.id_brand=br.id_brand";
			sql+=" and sc.id_subsector=gr.id_subsector";
			sql+=" and gr.id_group_=sg.id_group_";
			sql+=" and sg.id_segment=pr.id_segment";


			sql+=" and hc.id_language=ad.id_language";
			sql+=" and pr.id_language=ad.id_language";
			sql+=" and pr.id_language=br.id_language";
			sql+=" and sc.id_language=gr.id_language";
			sql+=" and gr.id_language=sg.id_language";
			sql+=" and sg.id_language=pr.id_language";

			sql+=" and hc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and br.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and sg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";


			sql+=" and hc.id_language="+webSession.SiteLanguage+"";
			sql+=" and ad.id_language="+webSession.SiteLanguage+"";
			sql+=" and br.id_language="+webSession.SiteLanguage+"";
			sql+=" and sc.id_language="+webSession.SiteLanguage+"";
			sql+=" and gr.id_language="+webSession.SiteLanguage+"";
			sql+=" and sg.id_language="+webSession.SiteLanguage+"";
			sql+=" and pr.id_language="+webSession.SiteLanguage+"";
					
			#region Droits clients
			//Droits clients partie produit
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) 
			{
				sql+=" ";	 
			}

			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) 
			{
				sql+=" and ( ";}
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				)
			{
				sql+="( ";}
					
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)sql+="  sc.id_sector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess)+")";
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0)
				sql+=" or sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 )
				sql+=" sc.id_subsector in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)+")";

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0))
				sql+=" or gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0)
				sql+="  gr.id_group_ in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess)+")";

					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0 && (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 || right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ))
				sql+=" or sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0)
				sql+="  sg.id_segment in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess)+")";


			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				)
			{
				sql+=" ) ";}

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0)sql+=" and sc.id_sector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0)sql+=" and sc.id_subsector not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0)sql+=" and gr.id_group_ not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0)sql+=" and sg.id_segment not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException)+")";
					

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0) 
			{
				sql+=" ";	 
			}
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.sectorException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.groupException).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.segmentException).Length>0
				) 
			{
				sql+=" ) ";}

						
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==00) 
			{
				sql+=" ";
			}
				//Droits clients partie advertiser
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) 
			{
				sql+=" and ( ";}
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				)
			{
				sql+="( ";}

					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
				sql+="  hc.id_holding_company in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess)+")";

			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0 && right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0)
				sql+=" or ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			else if (right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0)
				sql+=" ad.id_advertiser in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				)
			{
				sql+=" ) ";}
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0)sql+=" and hc.id_holding_company not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException)+")";
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0)sql+=" and ad.id_advertiser not in ("+right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException)+")";
					
			if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 
				&& right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length==0) 
			{
				sql+=" ";
			}
			else if(right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0 
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException).Length>0
				|| right.rightElementString(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException).Length>0) 
			{
				sql+=" ) ";}
			#endregion		
					
				
			if(newText.Length>0)
			{		
				sql+=" and ( brand like upper('%"+newText+"%')";
				if(listBrand.Length!=0) 
				{
					sql+=" or br.id_brand in ("+listBrand+")";
				}
			}
			else
			{
				if(listBrand.Length!=0) 
				{
					sql+=" and ( br.id_brand in ("+listBrand+")";
				}
			}


			if(listBrand.Length!=0 && (listBrand.Length!=0 || newText.Length>0))
			{
				sql+=" or pr.id_product in("+listBrand+")";
			}
			else if(listBrand.Length!=0)
			{
				sql+=" and ( pr.id_product in("+listBrand+")";
			}					
			
			sql+=" ) ";
			sql+=" order by brand,product";			
		
			#endregion

			#region Execution de la requête
			try{
				LoadDataSet(webSession,sql);
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.AdvertiserListException("Impossible de charger le dataset",err));
			}
			#endregion

		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient la liste des vehicles visible par le client
		/// </summary>
		public DataSet DsListAdvertiser{
			get{return(_dsListAdvertiser);}
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Chargement du DataSet
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="sql">SQL</param>
		private void LoadDataSet(WebSession webSession,string sql){

			#region Execution de la requête
			try{
				_dsListAdvertiser=webSession.Source.Fill(sql);
				_dsListAdvertiser.Tables[0].TableName="dsListAdvertiser";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.AdvertiserListException("Impossible de charger le dataset",err));
			}
			#endregion
		
		}
		#endregion
	
	}
}
